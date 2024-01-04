namespace AppSmokeTesting
{
    partial class Form1
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
            btnExecute = new Button();
            rtbResults = new RichTextBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            lblApplicationName = new Label();
            chkSendEMail = new CheckBox();
            cbEnvironment = new ComboBox();
            lblEnvironment = new Label();
            cbApplication = new ComboBox();
            lblApplication = new Label();
            tabPage2 = new TabPage();
            label2 = new Label();
            label1 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // btnExecute
            // 
            btnExecute.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExecute.Location = new Point(6, 248);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(372, 49);
            btnExecute.TabIndex = 0;
            btnExecute.Text = "Execute";
            btnExecute.UseVisualStyleBackColor = true;
            btnExecute.Click += btnExecute_Click;
            // 
            // rtbResults
            // 
            rtbResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbResults.Location = new Point(400, 0);
            rtbResults.Name = "rtbResults";
            rtbResults.Size = new Size(613, 331);
            rtbResults.TabIndex = 1;
            rtbResults.Text = "";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(2, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(392, 331);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lblApplicationName);
            tabPage1.Controls.Add(chkSendEMail);
            tabPage1.Controls.Add(cbEnvironment);
            tabPage1.Controls.Add(lblEnvironment);
            tabPage1.Controls.Add(btnExecute);
            tabPage1.Controls.Add(cbApplication);
            tabPage1.Controls.Add(lblApplication);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(384, 303);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Run";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblApplicationName
            // 
            lblApplicationName.AutoSize = true;
            lblApplicationName.Location = new Point(89, 32);
            lblApplicationName.Name = "lblApplicationName";
            lblApplicationName.Size = new Size(0, 15);
            lblApplicationName.TabIndex = 11;
            // 
            // chkSendEMail
            // 
            chkSendEMail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkSendEMail.AutoSize = true;
            chkSendEMail.Location = new Point(6, 223);
            chkSendEMail.Name = "chkSendEMail";
            chkSendEMail.Size = new Size(241, 19);
            chkSendEMail.TabIndex = 10;
            chkSendEMail.Text = "Composed mail will be display on screen";
            chkSendEMail.UseVisualStyleBackColor = true;
            chkSendEMail.CheckedChanged += chkSendEMail_CheckedChanged;
            // 
            // cbEnvironment
            // 
            cbEnvironment.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEnvironment.FormattingEnabled = true;
            cbEnvironment.Location = new Point(89, 53);
            cbEnvironment.Name = "cbEnvironment";
            cbEnvironment.Size = new Size(186, 23);
            cbEnvironment.TabIndex = 9;
            // 
            // lblEnvironment
            // 
            lblEnvironment.AutoSize = true;
            lblEnvironment.Location = new Point(6, 56);
            lblEnvironment.Name = "lblEnvironment";
            lblEnvironment.Size = new Size(80, 15);
            lblEnvironment.TabIndex = 8;
            lblEnvironment.Text = "Environments";
            // 
            // cbApplication
            // 
            cbApplication.DropDownStyle = ComboBoxStyle.DropDownList;
            cbApplication.FormattingEnabled = true;
            cbApplication.Location = new Point(89, 6);
            cbApplication.Name = "cbApplication";
            cbApplication.Size = new Size(186, 23);
            cbApplication.TabIndex = 7;
            cbApplication.SelectedIndexChanged += cbApplication_SelectedIndexChanged;
            // 
            // lblApplication
            // 
            lblApplication.AutoSize = true;
            lblApplication.Location = new Point(15, 9);
            lblApplication.Name = "lblApplication";
            lblApplication.Size = new Size(68, 15);
            lblApplication.TabIndex = 6;
            lblApplication.Text = "Application";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(384, 303);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Settings";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 55);
            label2.Name = "label2";
            label2.Size = new Size(112, 15);
            label2.TabIndex = 8;
            label2.Text = "newman script path";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 3);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 7;
            label1.Text = "node path";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1013, 331);
            Controls.Add(tabControl1);
            Controls.Add(rtbResults);
            Name = "Form1";
            Text = "..:: ciro | application testing ::..";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnExecute;
        private RichTextBox rtbResults;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private ComboBox cbEnvironment;
        private Label lblEnvironment;
        private ComboBox cbApplication;
        private Label lblApplication;
        private TabPage tabPage2;
        private Label label1;
        private Label label2;
        private CheckBox chkSendEMail;
        private Label lblApplicationName;
    }
}