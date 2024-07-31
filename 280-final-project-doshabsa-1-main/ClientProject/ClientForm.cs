using System.Net;
using TCPLibrary;

namespace ClientProject
{
    public partial class ClientForm : Form
    {
        private Client client;

        public ClientForm()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPort.Text, out int port))
            {
                client = new(txtIPAddress.Text, port);
                client.RecievePacket += Client_RecievePacket;


                // connection sucess, notify server
                Packet280 packet = new()
                {
                    ContentType = MessageType.Connected,
                    Username = client._username,
                    Payload = $"{client._username} has connected."
                };

                txtUsername.Text = client._username;
                await this.client.SendMessage(packet);
            }
        }

        private void Client_RecievePacket(Packet280 packet)
        {
            this.Invoke(() => lstClientMessages.Items.Add($"{packet.Username}: {packet.Payload}"));
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            Packet280 packet = new()
            {
                ContentType = MessageType.Broadcast,
                Username = client._username,
                Payload = $"{txtMessage.Text}"
            };
            txtMessage.Text = string.Empty;
            await this.client.SendMessage(packet);
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            await this.client.DisconnectClient();
        }

        private async void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            await this.client.DisconnectClient();
        }

        private async void btnSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileTransferObject obj = new()
                {
                    FileName = openFileDialog.SafeFileName,
                    FileBytes = File.ReadAllBytes(openFileDialog.FileName)
                };
                Packet280 packet = new()
                {
                    ContentType = MessageType.File,
                    Username = client._username,
                    Payload = obj.JsonSerialized()
                };
                await client.SendMessage(packet);
            }
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }
        }

        private async void btnChangeUsername_Click(object sender, EventArgs e)
        {
            // code to update name with server, await feedback to confirm name is allowed/available then inform/update the requesting client
            Packet280 packet = new()
            {
                ContentType = MessageType.UpdateUsername,
                Username = client._username,
                Payload = txtUsername.Text
            };
            await client.SendMessage(packet);
        }

        private void btnScreenShare_Click(object sender, EventArgs e)
        {
            ShareLobby startShare = new(client);
            startShare.Show();
        }
    }
}