using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using TCPLibrary;

namespace ServerProject
{
    // https://learn.microsoft.com/en-us/sysinternals/downloads/tcpview
    public class Server
    {
        // server must run a listener
        private TcpListener listener;
        // client is represented on server as a tcpclient
        List<TcpClient> clients = new();
        Dictionary<string, TcpClient> usernames = new(); // username => connection
        Dictionary<int?, List<TcpClient>> screenshares = new(); // roon number, connected clients 
        // list for disconnected clients
        List<TcpClient> disconnects = new();
        // is the server running?
        private bool running = false;
        // set up events to deal with packets
        public delegate void ServerMessage(Packet280 packet);
        public event ServerMessage? ServerMessageEvent;
        // for local server UI messages
        public delegate void LocalServerMessage(string msg);
        public event LocalServerMessage? LocalServerMessageEvent;

        // construct server and inform which port to start on
        public Server(int port)
        {
            this.listener = new(System.Net.IPAddress.Any, port);
        }

        public async Task Stop()
        {
            // set bool controlling start loop to end
            this.running = false;
            // stop the listener
            this.listener.Stop();
            // send a message to UI that server is stopped
            if (LocalServerMessageEvent != null)
                LocalServerMessageEvent("Server has stopped.");
        }

        public async Task Start()
        {
            // start the listener on machine
            this.listener.Start();
            // show server has started
            if (LocalServerMessageEvent != null)
                LocalServerMessageEvent("Server has started.");
            // keep track of if server is running
            this.running = true;
            while (this.running)
            {
                try
                {
                    // wait for client to connect
                    TcpClient client = await this.listener.AcceptTcpClientAsync();
                    // keep track of the client
                    clients.Add(client);
                    Task.Run(() => this.HandleClient(client));
                }
                catch (Exception ex)
                {
                    LocalServerMessageEvent(ex.Message);
                }
            }
        }

        private async Task BroadcastToAllClients(Packet280 packet)
        {
            // turn message into JSON string
            var msg = JsonConvert.SerializeObject(packet) + "<EOM>";
            byte[] buffer = Encoding.UTF8.GetBytes(msg);

            // loop through and send to each client
            foreach (var client in clients)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    // TODO -- inform everyone the current client has disconnected
                    disconnects.Add(client);
                }
            }
            foreach (var client in disconnects)
            {
                clients.Remove(client);
                // remove the client from username dictionary
                usernames.Remove(usernames.FirstOrDefault(x => x.Value == client).Key);
            }
        }

        private async Task BroadcastToSpecificClient(TcpClient specificClient, Packet280 packet)
        {
            // turn message into JSON string
            var msg = JsonConvert.SerializeObject(packet) + "<EOM>"; // if EOM added here, will break; clients need reader for EOM?
            byte[] buffer = Encoding.UTF8.GetBytes(msg);

            // loop through and send to each client
            foreach (var client in clients)
            {
                if (client == specificClient)
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        // TODO -- inform everyone the current client has disconnected
                        disconnects.Add(client);
                    }
            }
            foreach (var client in disconnects)
            {
                clients.Remove(client);
                // remove the client from username dictionary
                usernames.Remove(usernames.FirstOrDefault(x => x.Value == client).Key);
            }
        }

        public async void HandleClient(TcpClient client)
        {
            // spool up bytes
            string entireMessage = string.Empty;

            try
            {
                // establish networked stream
                NetworkStream stream = client.GetStream();
                int counter = 0;

                // establish buffer to read bytes off the stream
                byte[] buffer = new byte[4096];
                int bytesRead;
                Packet280 tmpMessage = null;

                // loop and wait for bytes to arrive from client
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // read the message as a string - large files will have issues here
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // keeps an iterative string which is added to as data comes off the stream
                    entireMessage += message;

                    // try to turn the mesasge into packet
                    try
                    {
                        if (entireMessage.Contains("<EOM>"))
                        {
                            string[] smallMsg = entireMessage.Split("<EOM>");
                            // deserialize a json string back into packet280 -- things get weird for screen shares
                            tmpMessage = JsonConvert.DeserializeObject<Packet280>(smallMsg[0]);

                            // clear message string
                            entireMessage = string.Empty;
                            // save everything else, complete or incomplete
                            for (int i = 1; i < smallMsg.Length; i++)
                            {
                                entireMessage += smallMsg[i];
                            }
                        }
                    }
                    catch
                    {
                        // failed if has not recieved all the bytes
                    }

                    if (tmpMessage != null)
                    {
                        switch (tmpMessage.ContentType)
                        {
                            case MessageType.Connected:
                                // add new client to username dictionary
                                usernames.Add(tmpMessage.Username, client);
                                LocalServerMessageEvent($"{tmpMessage.Username}: {tmpMessage.Payload}");
                                await BroadcastToAllClients(tmpMessage);
                                break;

                            case MessageType.Disconnected:
                                // remove disconnected client from username disctionary
                                usernames.Remove(tmpMessage.Username);
                                LocalServerMessageEvent($"{tmpMessage.Username}: {tmpMessage.Payload}");
                                // client gracefully disconnected
                                await BroadcastToAllClients(tmpMessage);
                                break;

                            // this should be sent to everyone
                            case MessageType.Broadcast:
                                LocalServerMessageEvent($"{tmpMessage.Username}: {tmpMessage.Payload}");
                                await BroadcastToAllClients(tmpMessage);
                                break;

                            case MessageType.File:
                                FileTransferObject file = JsonConvert.DeserializeObject<FileTransferObject>(tmpMessage.Payload);
                                // to save at other locations, edit file.FileName
                                await File.WriteAllBytesAsync(file.FileName, file.FileBytes);
                                LocalServerMessageEvent($"A file was saved: {file.FileName}");
                                break;

                            case MessageType.UpdateUsername:
                                if (UpdateUsername(client, tmpMessage))
                                {
                                    // true, change allowed and accepted, send UpdateUsername to that client to update their ClientForm
                                    Packet280 updateName = new()
                                    {
                                        ContentType = MessageType.UpdateUsername,
                                        Username = tmpMessage.Payload,
                                        Payload = $"{tmpMessage.Username},{tmpMessage.Payload}"
                                    };
                                    // send confrimation back to requesting client
                                    await BroadcastToSpecificClient(client, updateName);
                                    Packet280 tmpConf = new()
                                    {
                                        ContentType = MessageType.Broadcast,
                                        Username = tmpMessage.Payload,
                                        Payload = $"{tmpMessage.Username} has changed their name to {tmpMessage.Payload}"
                                    };
                                    // then udpate the rest of the server
                                    LocalServerMessageEvent($"{tmpMessage.Username} has changed their name to {tmpMessage.Payload}");
                                    await BroadcastToAllClients(tmpConf);
                                }
                                else
                                {
                                    // false, change not allowed, inform that client only
                                    Packet280 updateName = new()
                                    {
                                        ContentType = MessageType.UpdateUsername,
                                        Username = tmpMessage.Payload,
                                        Payload = $"failed, {tmpMessage.Username},{tmpMessage.Payload}"
                                    };
                                    // send confrimation back to requesting client
                                    await BroadcastToSpecificClient(client, updateName);
                                };
                                break;

                            case MessageType.ScreenShare:
                                if (tmpMessage.Room != null && screenshares.ContainsKey(tmpMessage.Room))
                                {
                                    List<TcpClient> clients = new(screenshares.FirstOrDefault(x => x.Key == tmpMessage.Room).Value);
                                    if (!clients.Contains(client))
                                    {
                                        // add the joining client
                                        clients.Add(client);
                                        screenshares.Remove(tmpMessage.Room);
                                        screenshares.Add(tmpMessage.Room, clients);
                                    }
                                    else
                                    {
                                        // forward object to all clients (exclude [0]/host)
                                        if (clients.Count > 1)
                                            for (int i = 0; i < clients.Count; i++)
                                            {
                                                if (i != 0)
                                                {
                                                    // forward host image to client (host always in first element)
                                                    await BroadcastToSpecificClient(clients[i], tmpMessage);
                                                }
                                            }
                                    }
                                }
                                else if (tmpMessage.Room != null)
                                {
                                    CreateScreenshare(client, tmpMessage);
                                }
                                break;

                            case MessageType.MouseEvent:
                                if (tmpMessage.Room != null && screenshares.ContainsKey(tmpMessage.Room))
                                {
                                    List<TcpClient> clients = new(screenshares.FirstOrDefault(x => x.Key == tmpMessage.Room).Value);
                                    if (clients.Count > 1)
                                        for (int i = 0; i < clients.Count; i++)
                                        {
                                            if (i == 0)
                                            {
                                                // forward joined click events to host
                                                await BroadcastToSpecificClient(clients[i], tmpMessage);
                                            }
                                        }
                                }
                                break;
                        }
                        tmpMessage = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Packet280 packet = new()
                {
                    ContentType = MessageType.Broadcast,
                    Payload = $"A client has disconnected from {client.Client.RemoteEndPoint} :: {ex.Message}"
                };
                // TODO -- event required to invoke message
            }
        }

        private bool UpdateUsername(TcpClient client, Packet280 message)
        {
            if (!usernames.ContainsKey(message.Payload))
            {
                usernames.Remove(message.Username);
                usernames.Add(message.Payload, client);
                return true;
            }
            // username already taken
            return false;
        }

        private void CreateScreenshare(TcpClient client, Packet280 message)
        { // create new list
            List<TcpClient> clients = new();
            // add the user who create the room (aka host)
            var newUser = usernames.FirstOrDefault(x => x.Value == client).Value;
            // add to list
            clients.Add(newUser);
            //add to screen share dictionary
            screenshares.Add(message.Room, clients);
        }
    }
}
