namespace LEAP_dis_manager
{
    partial class MainForm
    {

        

            // 
            // MainForm
            // 


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            button1 = new Button();
            title_label = new Label();
            close_button = new Button();
            Start_button = new Button();
            label1 = new Label();
            label2 = new Label();
            siteIdUpDown = new NumericUpDown();
            comboBox1 = new ComboBox();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)siteIdUpDown).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(93, 185);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Settings";
            button1.UseVisualStyleBackColor = true;
            button1.Click += openSettings;
            // 
            // title_label
            // 
            title_label.AutoSize = true;
            title_label.Font = new Font("Arial Narrow", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            title_label.Location = new Point(12, 9);
            title_label.Name = "title_label";
            title_label.Size = new Size(115, 25);
            title_label.TabIndex = 1;
            title_label.Text = "DIS Listener";
            // 
            // close_button
            // 
            close_button.Location = new Point(12, 185);
            close_button.Name = "close_button";
            close_button.Size = new Size(75, 23);
            close_button.TabIndex = 2;
            close_button.Text = "Close";
            close_button.UseVisualStyleBackColor = true;
            close_button.Click += close_program;
            // 
            // Start_button
            // 
            Start_button.Location = new Point(447, 185);
            Start_button.Name = "Start_button";
            Start_button.Size = new Size(75, 23);
            Start_button.TabIndex = 3;
            Start_button.Text = "Start";
            Start_button.UseVisualStyleBackColor = true;
            Start_button.Click += run;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(264, 129);
            label1.Name = "label1";
            label1.Size = new Size(68, 15);
            label1.TabIndex = 37;
            label1.Text = "Section IDs:";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(402, 129);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 38;
            label2.Text = "Site ID:";
            // 
            // siteIdUpDown
            // 
            siteIdUpDown.Location = new Point(402, 149);
            siteIdUpDown.Name = "siteIdUpDown";
            siteIdUpDown.Size = new Size(120, 23);
            siteIdUpDown.TabIndex = 39;
            siteIdUpDown.ValueChanged += siteIdUpDown_ValueChanged;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(264, 149);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 40;
            comboBox1.DropDown += comboBox1_DropDown;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.Location = new Point(134, 129);
            button2.Name = "button2";
            button2.Size = new Size(115, 42);
            button2.TabIndex = 41;
            button2.Text = "Create New Section ID";
            button2.UseVisualStyleBackColor = true;
            button2.Click += create_new_scenario;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(534, 220);
            Controls.Add(button2);
            Controls.Add(comboBox1);
            Controls.Add(siteIdUpDown);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Start_button);
            Controls.Add(close_button);
            Controls.Add(title_label);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "LEAP Dis Listener";
            ((System.ComponentModel.ISupportInitialize)siteIdUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label title_label;
        private Button close_button;
        private Button Start_button;
        private Label label1;
        private Label label2;
        private NumericUpDown siteIdUpDown;
        private ComboBox comboBox1;
        private Button button2;
    }
}