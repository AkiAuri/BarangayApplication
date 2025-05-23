namespace BarangayApplication
{
    partial class Settings
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
            this.BackupLoc = new System.Windows.Forms.TextBox();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSetPassword = new System.Windows.Forms.Button();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbAccountID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BackupLoc
            // 
            this.BackupLoc.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BackupLoc.BackColor = System.Drawing.SystemColors.MenuBar;
            this.BackupLoc.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackupLoc.Location = new System.Drawing.Point(55, 8);
            this.BackupLoc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BackupLoc.Name = "BackupLoc";
            this.BackupLoc.ReadOnly = true;
            this.BackupLoc.Size = new System.Drawing.Size(539, 43);
            this.BackupLoc.TabIndex = 36;
            // 
            // lblDateTime
            // 
            this.lblDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(487, 575);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(153, 38);
            this.lblDateTime.TabIndex = 35;
            this.lblDateTime.Text = "[date-time]";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(37, 562);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(386, 54);
            this.label7.TabIndex = 34;
            this.label7.Text = "LAST BACKEDUP @";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(322, 45);
            this.label6.TabIndex = 30;
            this.label6.Text = "BACKUP LOCATION";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // btnSetPassword
            // 
            this.btnSetPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.btnSetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetPassword.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetPassword.ForeColor = System.Drawing.Color.LightGray;
            this.btnSetPassword.Location = new System.Drawing.Point(1252, 761);
            this.btnSetPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSetPassword.Name = "btnSetPassword";
            this.btnSetPassword.Size = new System.Drawing.Size(125, 59);
            this.btnSetPassword.TabIndex = 29;
            this.btnSetPassword.Text = "EDIT";
            this.btnSetPassword.UseVisualStyleBackColor = false;
            this.btnSetPassword.Click += new System.EventHandler(this.btnSetPassword_Click);
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewPassword.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewPassword.Location = new System.Drawing.Point(894, 525);
            this.txtNewPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(395, 47);
            this.txtNewPassword.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(886, 478);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(203, 45);
            this.label5.TabIndex = 27;
            this.label5.Text = "PASSWORD";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtAccountName
            // 
            this.txtAccountName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccountName.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtAccountName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountName.Location = new System.Drawing.Point(894, 360);
            this.txtAccountName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.Size = new System.Drawing.Size(395, 47);
            this.txtAccountName.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(886, 313);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(286, 45);
            this.label4.TabIndex = 25;
            this.label4.Text = "ACCOUNT NAME";
            // 
            // panel3
            // 
            this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel3.Location = new System.Drawing.Point(32, 112);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(673, 3);
            this.panel3.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(335, 81);
            this.label3.TabIndex = 24;
            this.label3.Text = "DATABASE";
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel2.Location = new System.Drawing.Point(776, 112);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(673, 3);
            this.panel2.TabIndex = 19;
            // 
            // cmbAccountID
            // 
            this.cmbAccountID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAccountID.BackColor = System.Drawing.SystemColors.MenuBar;
            this.cmbAccountID.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAccountID.FormattingEnabled = true;
            this.cmbAccountID.Location = new System.Drawing.Point(894, 206);
            this.cmbAccountID.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbAccountID.Name = "cmbAccountID";
            this.cmbAccountID.Size = new System.Drawing.Size(395, 49);
            this.cmbAccountID.TabIndex = 23;
            this.cmbAccountID.SelectedIndexChanged += new System.EventHandler(this.cmbAccountID_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(886, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 45);
            this.label2.TabIndex = 21;
            this.label2.Text = "ACCOUNT ID";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(821, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 81);
            this.label1.TabIndex = 20;
            this.label1.Text = "ACCOUNTS";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(646, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(3, 838);
            this.panel1.TabIndex = 18;
            // 
            // panel4
            // 
            this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Controls.Add(this.BackupLoc);
            this.panel4.Location = new System.Drawing.Point(29, 184);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(617, 53);
            this.panel4.TabIndex = 37;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::BarangayApplication.Properties.Resources.circle_ellipsis;
            this.pictureBox1.Location = new System.Drawing.Point(6, 8);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(43, 43);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 31;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.ForeColor = System.Drawing.Color.LightGray;
            this.btnRestore.Image = global::BarangayApplication.Properties.Resources.time_past;
            this.btnRestore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestore.Location = new System.Drawing.Point(29, 419);
            this.btnRestore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(412, 85);
            this.btnRestore.TabIndex = 33;
            this.btnRestore.Text = "RESTORE DATABASE";
            this.btnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.btnBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackup.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackup.ForeColor = System.Drawing.Color.LightGray;
            this.btnBackup.Image = global::BarangayApplication.Properties.Resources.secure_backup1;
            this.btnBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackup.Location = new System.Drawing.Point(29, 306);
            this.btnBackup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(412, 85);
            this.btnBackup.TabIndex = 32;
            this.btnBackup.Text = "BACKUP DATABASE";
            this.btnBackup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(1424, 861);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSetPassword);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAccountName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.cmbAccountID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Settings";
            this.Text = "Settings";
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BackupLoc;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSetPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtAccountName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbAccountID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
    }
}