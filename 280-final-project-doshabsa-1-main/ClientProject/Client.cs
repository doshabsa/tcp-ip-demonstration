using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TCPLibrary;

namespace ClientProject
{
    public class Client
    {
        private TcpClient _client;
        public string _username { get; set; } = $"User{Random.Shared.Next(0, 50000)}";
        public delegate void RecievePacketMessage(Packet280 packet);
        public event RecievePacketMessage? RecievePacket;
        public delegate void RecieveFileTransferObject(FileTransferObject file);
        public event RecieveFileTransferObject? ClientRecieveScreenSharePacket;
        public delegate void RecievemouseEventObject(MouseEventObject mouse);
        public event RecievemouseEventObject? HostRecieveScreenSharePacket;

        public Client(string host, int port)
        {
            // makes connection and binds to ip address (hostname) and port
            this._client = new(host, port);
            // start receiving messages from the server
            Task.Run(() => Receive());
        }

        public async Task SendMessage(Packet280 packet)
        {
            NetworkStream stream = this._client.GetStream();
            // place marker at end of message to split on server side
            var tmp = JsonConvert.SerializeObject(packet) + "<EOM>"; // changed to prevent classmates breaking
            byte[] buffer = Encoding.UTF8.GetBytes(tmp);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<string> Receive()
        {
            NetworkStream stream = this._client.GetStream();
            while (true)
            {
                StringBuilder messageBuilder = new StringBuilder();
                byte[] buffer = new byte[4096];
                int bytesRead;

                do
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string part = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuilder.Append(part);
                } while (!messageBuilder.ToString().EndsWith("<EOM>")); // updated EOM message to prevent classmate breaking

                string fullMessage = messageBuilder.ToString().TrimEnd("<EOM>".ToCharArray());
                var message = JsonConvert.DeserializeObject<Packet280>(fullMessage);

                if (message.ContentType == MessageType.Broadcast ||
                    message.ContentType == MessageType.Connected ||
                    message.ContentType == MessageType.Disconnected)
                {
                    if (RecievePacket != null && message != null)
                    {
                        RecievePacket(message);
                    }
                }
                else if (message.ContentType == MessageType.UpdateUsername)
                {
                    // update username on client side
                    string[] confirmation = message.Payload.Split(',');
                    if (confirmation.Length == 2)
                    {
                        _username = confirmation[1];
                        Packet280 tmpConf = new()
                        {
                            ContentType = MessageType.Broadcast,
                            Username = _username,
                            Payload = $"Username changed to {_username}."
                        };
                        RecievePacket(tmpConf);
                    }
                    else
                    {
                        Packet280 tmpConf = new()
                        {
                            ContentType = MessageType.Broadcast,
                            Username = _username,
                            Payload = $"Please choose another username."
                        };
                        RecievePacket(tmpConf);
                    }
                }
                else if (message.ContentType == MessageType.ScreenShare)
                {
                    // deserialize FTO, turn byte load into image
                    FileTransferObject file = JsonConvert.DeserializeObject<FileTransferObject>(message.Payload);
                    // send image payload to screen share client
                    ClientRecieveScreenSharePacket(file);
                }
                else if (message.ContentType == MessageType.MouseEvent)
                {
                    // deserialize FTO, turn byte load into image
                    MouseEventObject mouse = JsonConvert.DeserializeObject<MouseEventObject>(message.Payload);
                    // send image payload to screen share client
                    HostRecieveScreenSharePacket(mouse);
                }
            }
        }

        public async Task DisconnectClient()
        {
            if (this._client != null && this._client.Connected)
            {
                // disconnecting, notify server
                Packet280 packet = new()
                {
                    ContentType = MessageType.Disconnected,
                    Username = _username,
                    Payload = $"{_username} has disconnected."
                };
                await SendMessage(packet);

                // close connection
                this._client.Client.Close();
            }
        }
    }
}
