namespace ClientProject
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstClientMessages = new ListBox();
            lblIPAdress = new Label();
            lblPort = new Label();
            txtIPAddress = new TextBox();
            txtPort = new TextBox();
            btnConnect = new Button();
            txtMessage = new TextBox();
            btnSend = new Button();
            btnDisconnect = new Button();
            btnSendFile = new Button();
            lblUsername = new Label();
            txtUsername = new TextBox();
            btnChangeUsername = new Button();
            btnScreenShare = new Button();
            SuspendLayout();
            // 
            // lstClientMessages
            // 
            lstClientMessages.Dock = DockStyle.Top;
            lstClientMessages.FormattingEnabled = true;
            lstClientMessages.ItemHeight = 15;
            lstClientMessages.Location = new Point(0, 0);
            lstClientMessages.Name = "lstClientMessages";
            lstClientMessages.Size = new Size(800, 259);
            lstClientMessages.TabIndex = 6;
            // 
            // lblIPAdress
            // 
            lblIPAdress.AutoSize = true;
            lblIPAdress.Location = new Point(12, 285);
            lblIPAdress.Name = "lblIPAdress";
            lblIPAdress.Size = new Size(65, 15);
            lblIPAdress.TabIndex = 1;
            lblIPAdress.Text = "IP Address:";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(154, 285);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(32, 15);
            lblPort.TabIndex = 1;
            lblPort.Text = "Port:";
            // 
            // txtIPAddress
            // 
            txtIPAddress.Location = new Point(83, 282);
            txtIPAddress.Name = "txtIPAddress";
            txtIPAddress.PlaceholderText = "host";
            txtIPAddress.Size = new Size(65, 23);
            txtIPAddress.TabIndex = 0;
            txtIPAddress.Text = "localhost";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(192, 282);
            txtPort.Name = "txtPort";
            txtPort.PlaceholderText = "port";
            txtPort.Size = new Size(61, 23);
            txtPort.TabIndex = 1;
            txtPort.Text = "10000";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(710, 280);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(12, 340);
            txtMessage.Name = "txtMessage";
            txtMessage.PlaceholderText = "message...";
            txtMessage.Size = new Size(692, 23);
            txtMessage.TabIndex = 4;
            txtMessage.Text = "test message";
            txtMessage.KeyDown += txtMessage_KeyDown;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(713, 339);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 5;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(710, 309);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 23);
            btnDisconnect.TabIndex = 3;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnSendFile
            // 
            btnSendFile.Location = new Point(629, 309);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(75, 23);
            btnSendFile.TabIndex = 5;
            btnSendFile.Text = "Send File";
            btnSendFile.UseVisualStyleBackColor = true;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(14, 317);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(63, 15);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(83, 312);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "username";
            txtUsername.Size = new Size(80, 23);
            txtUsername.TabIndex = 1;
            // 
            // btnChangeUsername
            // 
            btnChangeUsername.Location = new Point(169, 311);
            btnChangeUsername.Name = "btnChangeUsername";
            btnChangeUsername.Size = new Size(131, 23);
            btnChangeUsername.TabIndex = 7;
            btnChangeUsername.Text = "Change Username";
            btnChangeUsername.UseVisualStyleBackColor = true;
            btnChangeUsername.Click += btnChangeUsername_Click;
            // 
            // btnScreenShare
            // 
            btnScreenShare.Location = new Point(629, 280);
            btnScreenShare.Name = "btnScreenShare";
            btnScreenShare.Size = new Size(75, 23);
            btnScreenShare.TabIndex = 5;
            btnScreenShare.Text = "Screen Share";
            btnScreenShare.UseVisualStyleBackColor = true;
            btnScreenShare.Click += btnScreenShare_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 373);
            Controls.Add(btnChangeUsername);
            Controls.Add(txtMessage);
            Controls.Add(btnScreenShare);
            Controls.Add(btnSendFile);
            Controls.Add(btnSend);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Controls.Add(txtUsername);
            Controls.Add(txtPort);
            Controls.Add(txtIPAddress);
            Controls.Add(lblUsername);
            Controls.Add(lblPort);
            Controls.Add(lblIPAdress);
            Controls.Add(lstClientMessages);
            Name = "ClientForm";
            Text = "Client";
            FormClosing += ClientForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstClientMessages;
        private Label lblIPAdress;
        private Label lblPort;
        private TextBox txtIPAddress;
        private TextBox txtPort;
        private Button btnConnect;
        private TextBox txtMessage;
        private Button btnSend;
        private Button btnDisconnect;
        private Button btnSendFile;
        private Label lblUsername;
        private TextBox txtUsername;
        private Button btnChangeUsername;
        private Button btnScreenShare;
    }
}