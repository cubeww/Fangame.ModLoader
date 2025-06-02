namespace Fangame.ModLoader.Gui
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ModsListBox = new CheckedListBox();
            label1 = new Label();
            OutputTextBox = new TextBox();
            label2 = new Label();
            OpenModsFolderButton = new Button();
            SuspendLayout();
            // 
            // ModsListBox
            // 
            ModsListBox.CheckOnClick = true;
            ModsListBox.FormattingEnabled = true;
            ModsListBox.Location = new Point(23, 63);
            ModsListBox.Name = "ModsListBox";
            ModsListBox.Size = new Size(261, 328);
            ModsListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 14);
            label1.Name = "label1";
            label1.Size = new Size(59, 24);
            label1.TabIndex = 1;
            label1.Text = "Mods";
            // 
            // OutputTextBox
            // 
            OutputTextBox.AllowDrop = true;
            OutputTextBox.Location = new Point(328, 63);
            OutputTextBox.Multiline = true;
            OutputTextBox.Name = "OutputTextBox";
            OutputTextBox.ScrollBars = ScrollBars.Both;
            OutputTextBox.Size = new Size(440, 328);
            OutputTextBox.TabIndex = 2;
            OutputTextBox.Text = "To mod a game, drag and drop the game exe here...\r\n";
            OutputTextBox.DragDrop += OutputTextBox_DragDrop;
            OutputTextBox.DragEnter += OutputTextBox_DragEnter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(328, 14);
            label2.Name = "label2";
            label2.Size = new Size(73, 24);
            label2.TabIndex = 3;
            label2.Text = "Output";
            // 
            // OpenModsFolderButton
            // 
            OpenModsFolderButton.Location = new Point(105, 9);
            OpenModsFolderButton.Name = "OpenModsFolderButton";
            OpenModsFolderButton.Size = new Size(179, 34);
            OpenModsFolderButton.TabIndex = 4;
            OpenModsFolderButton.Text = "Open Folder";
            OpenModsFolderButton.UseVisualStyleBackColor = true;
            OpenModsFolderButton.Click += OpenModsFolderButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 416);
            Controls.Add(OpenModsFolderButton);
            Controls.Add(label2);
            Controls.Add(OutputTextBox);
            Controls.Add(label1);
            Controls.Add(ModsListBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Fangame.ModLoader";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckedListBox ModsListBox;
        private Label label1;
        private TextBox OutputTextBox;
        private Label label2;
        private Button OpenModsFolderButton;
    }
}
