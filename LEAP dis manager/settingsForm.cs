using System.Net;

namespace LEAP_dis_manager
{
    //Generic UI TODOs
    //TODO: Don't let the user change settings while the UDP client is connected.
    //Might also want to display a message box notifying them that they can't change settings while connected.

    //TODO: Think about filtering out unwanted characters in each text box entry  (ex: only allow numbers to be input for ports, IPs, etc.)

    //TODO: Restrict value inputs for text boxes (ex: Port can only be between 0-65535, IP octets can only be 0-255, etc.)

    //TODO: Add helper text by text boxes, notifying user of special cases that may not be apparent (ex: Excercise ID of -1 represents accepting all Exercise IDs, receive IP of 0.0.0.0 means to accept packets from any IP address, etc..)

    //TODO: Think about adding a checkbox for receiving loopback traffic. Meaning, should my computer receive its own packets or not.
    //Code already exists in the MainForm that should handle this, but is commented out as there is no checkbox to enable/disable it yet.

    public partial class Settings : Form
    {
        private MainForm mainForm;
        public string receivingIpAddress;
        public int receivingPort;
        public string databaseIpAddress;
        public int databasePort;
        public bool isMulticast;
        public int exerciseID;

        public Settings(MainForm mainForm, string receivingIpAddress, int receivingPort, string databaseIpAddress, int databasePort, bool isMulticast, int exerciseID)
        {
            InitializeComponent();

            this.mainForm = mainForm;
            this.receivingIpAddress = receivingIpAddress;
            this.receivingPort = receivingPort;
            this.databaseIpAddress = databaseIpAddress;
            this.databasePort = databasePort;
            this.isMulticast = isMulticast;
            this.exerciseID = exerciseID;
        }

        private void OK_Click(object sender, EventArgs e)

        {
            string tryReceivingAddress = $"{receivingIp1.Text}.{receivingIp2.Text}.{receivingIp3.Text}.{receivingIp4.Text}";

            string tryDatabaseAddress = $"{databaseIp1.Text}.{databaseIp2.Text}.{databaseIp3.Text}.{databaseIp4.Text}";


            if (IPAddress.TryParse(tryReceivingAddress, out _))
            {
                receivingIpAddress = tryReceivingAddress;
            }
            else
            {
                MessageBox.Show("Invalid receiving IP address: ", tryReceivingAddress);
                return;
            }

            if (int.TryParse(recievingPortMTB.Text, out int port1))
            {
                receivingPort = port1;
            }
            else
            {
                MessageBox.Show("Invalid receiving port");
                return;
            }

            if (IPAddress.TryParse(tryDatabaseAddress, out _))
            {
                databaseIpAddress = tryDatabaseAddress;
            }
            else
            {
                MessageBox.Show("Invalid database IP address");
                return;
            }

            if (int.TryParse(databasePortMTB.Text, out int port2))
            {
                databasePort = port2;
            }
            else
            {
                MessageBox.Show("Invalid database port");
                return;
            }

            isMulticast = Multicast.Checked;

            if (int.TryParse(exerciseIDTextBox.Text, out int disExerciseID))
            {
                exerciseID = disExerciseID;
            }
            else
            {
                MessageBox.Show("Invalid exercise ID");
                return;
            }

            // Save settings to MainForm and Properties.Settings
            mainForm.SaveSettings(receivingIpAddress, receivingPort, databaseIpAddress, databasePort, isMulticast, exerciseID);

            Properties.Settings.Default.isMulticast = isMulticast;
            Properties.Settings.Default.receivingIpAddress = receivingIpAddress;
            Properties.Settings.Default.receivingPort = receivingPort;
            Properties.Settings.Default.databaseIpAddress = databaseIpAddress;
            Properties.Settings.Default.databasePort = databasePort;
            Properties.Settings.Default.exerciseID = exerciseID;
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // Load current settings into the form controls
            string[] receivingParts = receivingIpAddress.Split('.');
            if (receivingParts.Length == 4)
            {
                receivingIp1.Text = receivingParts[0];
                receivingIp2.Text = receivingParts[1];
                receivingIp3.Text = receivingParts[2];
                receivingIp4.Text = receivingParts[3];
            }

            recievingPortMTB.Text = receivingPort.ToString();

            string[] databaseParts = databaseIpAddress.Split('.');
            if (databaseParts.Length == 4)
            {
                databaseIp1.Text = databaseParts[0];
                databaseIp2.Text = databaseParts[1];
                databaseIp3.Text = databaseParts[2];
                databaseIp4.Text = databaseParts[3];
            }

            databasePortMTB.Text = databasePort.ToString();

            Multicast.Checked = isMulticast;

            exerciseIDTextBox.Text = exerciseID.ToString();
        }
        private void ipTextBox_click(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
        private void cancel_click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void AdjustIpInput(TextBox textBox)
        {
            var text = textBox.Text;
            if (text.Length > 3)
            {
                textBox.Text = text.Substring(0, 3);
            }
        }

        private void receivingIp1_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(receivingIp1);
        }

        private void receivingIp2_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(receivingIp2);
        }

        private void receivingIp3_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(receivingIp3);
        }

        private void receivingIp4_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(receivingIp4);
        }

        private void databaseIp1_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(databaseIp1);
        }

        private void databaseIp2_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(databaseIp2);
        }

        private void databaseIp3_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(databaseIp3);
        }

        private void databaseIp4_TextChanged(object sender, EventArgs e)
        {
            AdjustIpInput(databaseIp4);
        }

        private void Multicast_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
