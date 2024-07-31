namespace ServerProject
{
    public partial class ServerForm : Form
    {
        Server server = new(10000);

        public ServerForm()
        {
            InitializeComponent();
            // auto start server, cause I am too lazy to click it
            Task.Run(() => StartServer());
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if(btnStart.Enabled)
            {
                btnStart.Enabled = false;
                await StartServer();
            }
        }

        private async Task StartServer()
        {
            server.ServerMessageEvent += Server_ServerMessageEvent;
            server.LocalServerMessageEvent += Server_LocalServerMessageEvent;
            Task serverTask = server.Start();
            await serverTask;
        }

        private void Server_LocalServerMessageEvent(string msg)
        {
            this.Invoke(() => lstMessages.Items.Add(msg));
        }

        private void Server_ServerMessageEvent(TCPLibrary.Packet280 packet)
        {
            this.Invoke(() => lstMessages.Items.Add(packet.Payload));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            server.Stop();
            btnStart.Enabled = true;
        }
    }
}