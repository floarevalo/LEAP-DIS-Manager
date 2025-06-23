namespace LEAP_dis_manager
{
    partial class ScenarioInputForm
    {
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
            newScenarioNameTextBox = new TextBox();
            label2 = new Label();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // newScenarioNameTextBox
            // 
            newScenarioNameTextBox.Location = new Point(165, 112);
            newScenarioNameTextBox.Name = "newScenarioNameTextBox";
            newScenarioNameTextBox.Size = new Size(188, 23);
            newScenarioNameTextBox.TabIndex = 0;
            newScenarioNameTextBox.Click += scenarioNameTextBox_Click;
            newScenarioNameTextBox.TextChanged += scenarioNameTextBox_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(165, 94);
            label2.Name = "label2";
            label2.Size = new Size(150, 15);
            label2.TabIndex = 2;
            label2.Text = "Enter a New Senario Name:";
            // 
            // button1
            // 
            button1.Location = new Point(548, 220);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OK_Click;
            // 
            // button2
            // 
            button2.Location = new Point(467, 220);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 4;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Cancel_Click;
            // 
            // ScenarioInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(639, 260);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(newScenarioNameTextBox);
            Name = "ScenarioInputForm";
            Text = "ScenarioInputForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox newScenarioNameTextBox;
        private Label label2;
        private Button button1;
        private Button button2;
    }
}