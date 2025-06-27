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
            Start_button = new Button();
            label1 = new Label();
            label2 = new Label();
            siteIdUpDown = new NumericUpDown();
            sectionIdDropdown = new ComboBox();
            button2 = new Button();
            EntityUpdateLabel = new Label();
            dataGridView = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)siteIdUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 270);
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
            // Start_button
            // 
            Start_button.Location = new Point(586, 299);
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
            label1.Location = new Point(403, 252);
            label1.Name = "label1";
            label1.Size = new Size(68, 15);
            label1.TabIndex = 37;
            label1.Text = "Section IDs:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(541, 252);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 38;
            label2.Text = "Site ID:";
            // 
            // siteIdUpDown
            // 
            siteIdUpDown.Location = new Point(541, 270);
            siteIdUpDown.Name = "siteIdUpDown";
            siteIdUpDown.Size = new Size(120, 23);
            siteIdUpDown.TabIndex = 39;
            siteIdUpDown.ValueChanged += siteIdUpDown_ValueChanged;
            // 
            // sectionIdDropdown
            // 
            sectionIdDropdown.FormattingEnabled = true;
            sectionIdDropdown.Location = new Point(403, 270);
            sectionIdDropdown.Name = "sectionIdDropdown";
            sectionIdDropdown.Size = new Size(121, 23);
            sectionIdDropdown.TabIndex = 40;
            sectionIdDropdown.DropDown += comboBox1_DropDown;
            sectionIdDropdown.SelectedIndexChanged += OnSectionIdSelectionChanged;
            // 
            // button2
            // 
            button2.Location = new Point(275, 252);
            button2.Name = "button2";
            button2.Size = new Size(113, 41);
            button2.TabIndex = 41;
            button2.Text = "New Section ID";
            button2.UseVisualStyleBackColor = true;
            button2.Click += OpenNewScenarioDialog;
            // 
            // EntityUpdateLabel
            // 
            EntityUpdateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            EntityUpdateLabel.AutoSize = true;
            EntityUpdateLabel.Font = new Font("Segoe UI", 13F);
            EntityUpdateLabel.Location = new Point(198, 51);
            EntityUpdateLabel.Name = "EntityUpdateLabel";
            EntityUpdateLabel.Size = new Size(155, 25);
            EntityUpdateLabel.TabIndex = 42;
            EntityUpdateLabel.Text = "EntityUpdateLabel";
            EntityUpdateLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dataGridView
            // 
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dataGridView.Location = new Point(198, 79);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersVisible = false;
            dataGridView.Size = new Size(273, 128);
            dataGridView.TabIndex = 43;
            // 
            // Column1
            // 
            Column1.HeaderText = "Unit Name";
            Column1.Name = "Column1";
            // 
            // Column2
            // 
            Column2.HeaderText = "Time Stamp";
            Column2.Name = "Column2";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(690, 334);
            Controls.Add(dataGridView);
            Controls.Add(EntityUpdateLabel);
            Controls.Add(button2);
            Controls.Add(sectionIdDropdown);
            Controls.Add(siteIdUpDown);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Start_button);
            Controls.Add(title_label);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "LEAP Dis Listener";
            ((System.ComponentModel.ISupportInitialize)siteIdUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label title_label;
        private Button Start_button;
        private Label label1;
        private Label label2;
        private NumericUpDown siteIdUpDown;
        private ComboBox sectionIdDropdown;
        private Button button2;
        private Label EntityUpdateLabel;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
    }
}