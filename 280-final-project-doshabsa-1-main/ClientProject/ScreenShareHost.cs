using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Windows.Forms;
using SharpHook;
using SharpHook.Native;
using TCPLibrary;

namespace ClientProject
{
    public partial class ScreenShareHost : Form
    {
        private Client _client;
        private int _JoinCode = Random.Shared.Next(6000, 8000);
        private int RequireScreenFixX = 0;
        private int RequireScreenFixY = 0;

        public ScreenShareHost(Client user)
        {
            InitializeComponent();
            _client = user;
            _client.HostRecieveScreenSharePacket += _client_HostRecieveScreenSharePacket;
            // now that screenshare is on, enable timer
            timer1.Enabled = true;
            this.Text = _JoinCode.ToString() + "Host";
        }

        private void _client_HostRecieveScreenSharePacket(MouseEventObject mouse)
        {
            // update host events/mouse movement here

            X = SystemInformation.VirtualScreen.Width;
            Y = SystemInformation.VirtualScreen.Height;
            //We do this to get mouse coordinates


            //Set up ratio for x and y
            double xRatio = (double)mouse.X / (double)pictureBox1.Width;
            double yRatio = (double)mouse.Y / (double)pictureBox1.Height;
            //We want to know if the left or right mouse button was clicked
            if (mouse.Button == "Left")
            {
                //Get the points to click on the real screen
                var a = (X * xRatio) - RequireScreenFixX; // this was added for this computer specifically?
                var b = (Y * yRatio) - RequireScreenFixY;
                //move mouse
                eventSim.SimulateMouseMovement((short)a, (short)b);
                // does not alow for click and drag
                eventSim.SimulateMousePress((short)a, (short)b, MouseButton.Button1);
                eventSim.SimulateMouseRelease((short)a, (short)b, MouseButton.Button1);
            }
            else if (mouse.Button == "Right")
            {
                //Get the points to click on the real screen
                var a = (X * xRatio);
                var b = (Y * yRatio);
                //There is no click code yet!
                eventSim.SimulateMouseMovement((short)a, (short)b);
                // does not alow for click and drag
                eventSim.SimulateMousePress((short)a, (short)b, MouseButton.Button2);
                eventSim.SimulateMouseRelease((short)a, (short)b, MouseButton.Button2);
            }
        }

        // not used
        public Bitmap CaptureScreen(string fileName, ImageFormat format)
        {
            Bitmap capturedBitap = new(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height);
            Graphics g = Graphics.FromImage(capturedBitap);
            g.CopyFromScreen(Screen.AllScreens[1].Bounds.X, Screen.AllScreens[1].Bounds.Y,
                0, 0, Screen.AllScreens[1].Bounds.Size, CopyPixelOperation.SourceCopy);
            capturedBitap.Save(fileName, format);
            return capturedBitap;
        }

        int X, Y = 0;
        Bitmap screenshot = null;
        Graphics screenGraphics = null;
        public void CaptureWholeScreen()
        {
            X = SystemInformation.VirtualScreen.Width;
            Y = SystemInformation.VirtualScreen.Height;

            screenshot = new(X, Y, PixelFormat.Format32bppPArgb);
            screenGraphics = Graphics.FromImage(screenshot);
            screenGraphics.CopyFromScreen(SystemInformation.VirtualScreen.X, SystemInformation.VirtualScreen.Y,
                0, 0, SystemInformation.VirtualScreen.Size, CopyPixelOperation.SourceCopy);
            screenshot.Save("screenshot.png");
            // important:
            pictureBox1.Image = screenshot;
            // if memory becomes an issue, call this expensive operation
            //GC.Collect();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            CaptureWholeScreen();

            FileTransferObject obj = new()
            {
                FileName = "screenshot.bmp",
                FileBytes = ImageToByte(screenshot)
            };
            Packet280 packet = new()
            {
                ContentType = MessageType.ScreenShare,
                Username = _client._username,
                Room = _JoinCode,
                Payload = obj.JsonSerialized()
            };
            // send to server
            await this._client.SendMessage(packet);
        }

        // https://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        // sharphook
        EventSimulator eventSim = new();
        private void pictureBox1_Click(object sender, EventArgs e)
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
                eventSim.SimulateMouseMovement((short)a, (short)b);
            }
            else if (me.Button == MouseButtons.Right)
            {
                //Get the points to click on the real screen
                var a = (X * xRatio);
                var b = (Y * yRatio);
                //There is no click code yet!
                eventSim.SimulateMouseMovement((short)a, (short)b);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // if doing remote help, construct packet and sent to client/server
            eventSim.SimulateTextEntry(e.KeyChar.ToString());
            if (e.KeyChar == (char)Keys.Escape)
            {
                timer1.Enabled = false;
                this.Close();
            }
            if (e.KeyChar == (char)Keys.F)
            {
                if (RequireScreenFixX == 0)
                {
                    RequireScreenFixX = Math.Abs(SystemInformation.VirtualScreen.Width);
                    RequireScreenFixX = Math.Abs(SystemInformation.VirtualScreen.Height);
                }
                else
                {
                    RequireScreenFixX = 0;
                    RequireScreenFixY = 0;
                }
            }
        }

        private void ScreenShareHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }
    }
}
