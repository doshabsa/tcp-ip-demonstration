using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLibrary;

namespace ClientProject
{
    public partial class ShareLobby : Form
    {
        private Client _client;
        private Button btnHost;
        private Button btnJoin;
        private TextBox txtJoinCode;

        private void InitializeComponent()
        {
            btnHost = new Button();
            btnJoin = new Button();
            txtJoinCode = new TextBox();
            SuspendLayout();
            // 
            // btnHost
            // 
            btnHost.Location = new Point(12, 12);
            btnHost.Name = "btnHost";
            btnHost.Size = new Size(96, 23);
            btnHost.TabIndex = 0;
            btnHost.Text = "Host";
            btnHost.UseVisualStyleBackColor = true;
            btnHost.Click += btnHost_Click;
            // 
            // btnJoin
            // 
            btnJoin.Location = new Point(12, 41);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(96, 23);
            btnJoin.TabIndex = 0;
            btnJoin.Text = "Join";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // txtJoinCode
            // 
            txtJoinCode.Location = new Point(12, 70);
            txtJoinCode.Name = "txtJoinCode";
            txtJoinCode.PlaceholderText = "join code";
            txtJoinCode.Size = new Size(96, 23);
            txtJoinCode.TabIndex = 1;
            // 
            // ShareLobby
            // 
            AutoValidate = AutoValidate.Disable;
            ClientSize = new Size(120, 112);
            Controls.Add(txtJoinCode);
            Controls.Add(btnJoin);
            Controls.Add(btnHost);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShareLobby";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }

        public ShareLobby(Client user)
        {
            InitializeComponent();
            _client = user;
        }

        private void btnHost_Click(object sender, EventArgs e)
        {
            ScreenShareHost host = new(_client);
            host.Show();
        }

        private async void btnJoin_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtJoinCode.Text, out int code))
            {
                // use join code to find and join the screen share
                ScreenShareClient ssClient = new(_client, code);
                ssClient.Show();
            }
            else
                MessageBox.Show("Please enter a valid room number to join.");
        }
    }
}
