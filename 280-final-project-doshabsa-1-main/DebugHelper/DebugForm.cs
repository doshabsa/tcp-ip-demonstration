namespace DebugHelper
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServerProject.ServerForm s1 = new();
            s1.Show();

            ClientProject.ClientForm c1 = new();
            c1.Show();

            ClientProject.ClientForm c2 = new();
            c2.Show();
        }
    }
}