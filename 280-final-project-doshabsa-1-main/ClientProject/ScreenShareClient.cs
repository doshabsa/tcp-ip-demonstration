using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpHook;
using SharpHook.Native;
using TCPLibrary;

namespace ClientProject
{
    public partial class ScreenShareClient : Form
    {
        private Client _client;
        private int _JoinCode;

        // sharphook
        EventSimulator eventSim = new();
        int X, Y = 0;
        private int RequireScreenFixX = 0;
        private int RequireScreenFixY = 0;

        public ScreenShareClient(Client user, int joinCode)
        {
            InitializeComponent();
            _client = user;
            _client.ClientRecieveScreenSharePacket += _client_RecieveScreenSharePacket;
            _JoinCode = joinCode;
            this.Text = _JoinCode.ToString() + "Client";

            Task.Run(() => ConnectToRoom());
        }

        private async void pictureBox1_Click(object sender, EventArgs e)
        {
            X = SystemInformation.VirtualScreen.Width;
            Y = SystemInformation.VirtualScreen.Height;
            //We do this to get mouse coordinates
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            //Set up ratio for x and y
            double xRatio = (double)coordinates.X / (double)pictureBox1.Width;
            double yRatio = (double)coordinates.Y / (double)pictureBox1.Height;
            //We want to know if the left or right mouse button was clicked
            if (me.Button == MouseButtons.Left)
            {
                //Get the points to click on the real screen
                var a = (X * xRatio) - RequireScreenFixX; // this was added for this computer specifically?
                var b = (Y * yRatio) - RequireScreenFixY;
                // does not alow for click and drag
                MouseEventObject leftClick = new()
                {
                    X = me.X,
                    Y = me.Y,
                    Button = "Left"
                };
                Packet280 mousePacket = new()
                {
                    ContentType = MessageType.MouseEvent,
                    Username = _client._username,
                    Room = _JoinCode,
                    Payload = leftClick.JsonSerialized()
                };
                await this._client.SendMessage(mousePacket);
            }
            else if (me.Button == MouseButtons.Right)
            {
                //Get the points to click on the real screen
                var a = (X * xRatio);
                var b = (Y * yRatio);
                MouseEventObject rightClick = new()
                {
                    X = (short)a,
                    Y = (short)b,
                    Button = "Right"
                };
                Packet280 mousePacket = new()
                {
                    ContentType = MessageType.MouseEvent,
                    Username = _client._username,
                    Room = _JoinCode,
                    Payload = rightClick.JsonSerialized()
                };
                await this._client.SendMessage(mousePacket);
            }
        }

        private void _client_RecieveScreenSharePacket(FileTransferObject file)
        {
            // deserialize FTO, turn byte load into image
            pictureBox1.Image = ByteArrayToBitmap(file.FileBytes);
        }

        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                // Create a Bitmap from the MemoryStream
                Bitmap bitmap = new Bitmap(ms);
                return bitmap;
            }
        }

        private async Task ConnectToRoom()
        {
            Packet280 packet = new()
            {
                ContentType = MessageType.ScreenShare,
                Username = _client._username,
                Room = _JoinCode
            };
            // send to server
            await this._client.SendMessage(packet);
        }

        private void ScreenShareHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            // leave the room?
        }
    }
}
