namespace LEAP_dis_manager
{


    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            ipLabel = new Label();
            portLabel = new Label();
            Multicast = new CheckBox();
            recievingPortMTB = new MaskedTextBox();
            databasePortMTB = new MaskedTextBox();
            label1 = new Label();
            label2 = new Label();
            OK = new Button();
            cancelButton = new Button();
            receivingIp1 = new TextBox();
            receivingIp2 = new TextBox();
            receivingIp3 = new TextBox();
            receivingIp4 = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            databaseIp4 = new TextBox();
            databaseIp3 = new TextBox();
            databaseIp2 = new TextBox();
            databaseIp1 = new TextBox();
            label9 = new Label();
            exerciseIDTextBox = new MaskedTextBox();
            SuspendLayout();
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Location = new Point(24, 26);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(114, 15);
            ipLabel.TabIndex = 0;
            ipLabel.Text = "Receiving IP address";
            ipLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(23, 90);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(83, 15);
            portLabel.TabIndex = 2;
            portLabel.Text = "Recieving Port";
            portLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Multicast
            // 
            Multicast.AutoSize = true;
            Multicast.Location = new Point(24, 157);
            Multicast.Name = "Multicast";
            Multicast.Size = new Size(75, 19);
            Multicast.TabIndex = 4;
            Multicast.Text = "Multicast";
            Multicast.UseVisualStyleBackColor = true;
            Multicast.CheckedChanged += Multicast_CheckedChanged;
            // 
            // recievingPortMTB
            // 
            recievingPortMTB.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            recievingPortMTB.Location = new Point(24, 108);
            recievingPortMTB.Name = "recievingPortMTB";
            recievingPortMTB.Size = new Size(82, 29);
            recievingPortMTB.TabIndex = 6;
            recievingPortMTB.Text = "52";
            recievingPortMTB.Click += ipTextBox_click;
            // 
            // databasePortMTB
            // 
            databasePortMTB.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            databasePortMTB.Location = new Point(222, 108);
            databasePortMTB.Name = "databasePortMTB";
            databasePortMTB.Size = new Size(79, 29);
            databasePortMTB.TabIndex = 10;
            databasePortMTB.Text = "520";
            databasePortMTB.Click += ipTextBox_click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(221, 90);
            label1.Name = "label1";
            label1.Size = new Size(80, 15);
            label1.TabIndex = 8;
            label1.Text = "Database Port";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(222, 26);
            label2.Name = "label2";
            label2.Size = new Size(111, 15);
            label2.TabIndex = 7;
            label2.Text = "Database IP address";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // OK
            // 
            OK.Location = new Point(309, 255);
            OK.Name = "OK";
            OK.Size = new Size(75, 23);
            OK.TabIndex = 11;
            OK.Text = "OK";
            OK.UseVisualStyleBackColor = true;
            OK.Click += OK_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(228, 255);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancel_click;
            // 
            // receivingIp1
            // 
            receivingIp1.Location = new Point(24, 44);
            receivingIp1.MaxLength = 3;
            receivingIp1.Name = "receivingIp1";
            receivingIp1.Size = new Size(33, 23);
            receivingIp1.TabIndex = 16;
            receivingIp1.Text = "123";
            receivingIp1.Click += ipTextBox_click;
            receivingIp1.TextChanged += receivingIp1_TextChanged;
            // 
            // receivingIp2
            // 
            receivingIp2.Location = new Point(63, 44);
            receivingIp2.MaxLength = 3;
            receivingIp2.Name = "receivingIp2";
            receivingIp2.Size = new Size(33, 23);
            receivingIp2.TabIndex = 17;
            receivingIp2.Text = "52";
            receivingIp2.Click += ipTextBox_click;
            receivingIp2.TextChanged += receivingIp2_TextChanged;
            // 
            // receivingIp3
            // 
            receivingIp3.Location = new Point(102, 44);
            receivingIp3.MaxLength = 3;
            receivingIp3.Name = "receivingIp3";
            receivingIp3.Size = new Size(33, 23);
            receivingIp3.TabIndex = 18;
            receivingIp3.Text = "78";
            receivingIp3.Click += ipTextBox_click;
            receivingIp3.TextChanged += receivingIp3_TextChanged;
            // 
            // receivingIp4
            // 
            receivingIp4.Location = new Point(141, 44);
            receivingIp4.MaxLength = 3;
            receivingIp4.Name = "receivingIp4";
            receivingIp4.Size = new Size(33, 23);
            receivingIp4.TabIndex = 19;
            receivingIp4.Text = "988";
            receivingIp4.Click += ipTextBox_click;
            receivingIp4.TextChanged += receivingIp4_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(55, 52);
            label3.Name = "label3";
            label3.Size = new Size(10, 15);
            label3.TabIndex = 20;
            label3.Text = ".";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(94, 52);
            label4.Name = "label4";
            label4.Size = new Size(10, 15);
            label4.TabIndex = 21;
            label4.Text = ".";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(133, 52);
            label5.Name = "label5";
            label5.Size = new Size(10, 15);
            label5.TabIndex = 22;
            label5.Text = ".";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(333, 52);
            label6.Name = "label6";
            label6.Size = new Size(10, 15);
            label6.TabIndex = 29;
            label6.Text = ".";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(294, 52);
            label7.Name = "label7";
            label7.Size = new Size(10, 15);
            label7.TabIndex = 28;
            label7.Text = ".";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(255, 52);
            label8.Name = "label8";
            label8.Size = new Size(10, 15);
            label8.TabIndex = 27;
            label8.Text = ".";
            // 
            // databaseIp4
            // 
            databaseIp4.Location = new Point(341, 44);
            databaseIp4.MaxLength = 3;
            databaseIp4.Name = "databaseIp4";
            databaseIp4.Size = new Size(33, 23);
            databaseIp4.TabIndex = 26;
            databaseIp4.Text = "96";
            databaseIp4.Click += ipTextBox_click;
            databaseIp4.TextChanged += databaseIp4_TextChanged;
            // 
            // databaseIp3
            // 
            databaseIp3.Location = new Point(302, 44);
            databaseIp3.MaxLength = 3;
            databaseIp3.Name = "databaseIp3";
            databaseIp3.Size = new Size(33, 23);
            databaseIp3.TabIndex = 25;
            databaseIp3.Text = "20";
            databaseIp3.Click += ipTextBox_click;
            databaseIp3.TextChanged += databaseIp3_TextChanged;
            // 
            // databaseIp2
            // 
            databaseIp2.Location = new Point(263, 44);
            databaseIp2.MaxLength = 3;
            databaseIp2.Name = "databaseIp2";
            databaseIp2.Size = new Size(33, 23);
            databaseIp2.TabIndex = 24;
            databaseIp2.Text = "58";
            databaseIp2.Click += ipTextBox_click;
            databaseIp2.TextChanged += databaseIp2_TextChanged;
            // 
            // databaseIp1
            // 
            databaseIp1.Location = new Point(224, 44);
            databaseIp1.MaxLength = 3;
            databaseIp1.Name = "databaseIp1";
            databaseIp1.Size = new Size(33, 23);
            databaseIp1.TabIndex = 23;
            databaseIp1.Text = "58";
            databaseIp1.Click += ipTextBox_click;
            databaseIp1.TextChanged += databaseIp1_TextChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(221, 157);
            label9.Name = "label9";
            label9.Size = new Size(63, 15);
            label9.TabIndex = 30;
            label9.Text = "Exercise ID";
            // 
            // exerciseIDTextBox
            // 
            exerciseIDTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exerciseIDTextBox.Location = new Point(222, 175);
            exerciseIDTextBox.Name = "exerciseIDTextBox";
            exerciseIDTextBox.Size = new Size(79, 29);
            exerciseIDTextBox.TabIndex = 31;
            exerciseIDTextBox.Text = "222";
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(396, 290);
            Controls.Add(exerciseIDTextBox);
            Controls.Add(label9);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(databaseIp4);
            Controls.Add(databaseIp3);
            Controls.Add(databaseIp2);
            Controls.Add(databaseIp1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(receivingIp4);
            Controls.Add(receivingIp3);
            Controls.Add(receivingIp2);
            Controls.Add(receivingIp1);
            Controls.Add(cancelButton);
            Controls.Add(OK);
            Controls.Add(databasePortMTB);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(recievingPortMTB);
            Controls.Add(Multicast);
            Controls.Add(portLabel);
            Controls.Add(ipLabel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Settings";
            Text = "Settings";
            Load += Settings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ipLabel;
        private Label portLabel;
        private CheckBox Multicast;
        private MaskedTextBox recievingPortMTB;
        private MaskedTextBox databasePortMTB;
        private Label label1;
        private Label label2;
        private Button OK;
        private Button cancelButton;
        private TextBox receivingIp1;
        private TextBox receivingIp2;
        private TextBox receivingIp3;
        private TextBox receivingIp4;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox databaseIp4;
        private TextBox databaseIp3;
        private TextBox databaseIp2;
        private TextBox databaseIp1;
        private Label label9;
        private MaskedTextBox exerciseIDTextBox;
    }
}
