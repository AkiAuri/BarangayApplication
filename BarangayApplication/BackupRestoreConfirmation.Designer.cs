namespace BarangayApplication
{
    partial class BackupRestoreConfirmation
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
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblWarning = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Confirmbtn = new System.Windows.Forms.Button();
            this.Cancelbtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblWarning.Location = new System.Drawing.Point(16, 57);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(254, 20);
            this.lblWarning.TabIndex = 9;
            this.lblWarning.Text = "Are you sure you want to proceed?";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Location = new System.Drawing.Point(9, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(276, 2);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Medium", 16F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(46, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 28);
            this.label1.TabIndex = 7;
            this.label1.Text = "Confirmation";
            // 
            // Confirmbtn
            // 
            this.Confirmbtn.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Confirmbtn.ForeColor = System.Drawing.Color.White;
            this.Confirmbtn.Location = new System.Drawing.Point(223, 123);
            this.Confirmbtn.Name = "Confirmbtn";
            this.Confirmbtn.Size = new System.Drawing.Size(62, 31);
            this.Confirmbtn.TabIndex = 5;
            this.Confirmbtn.Text = "Confirm";
            this.Confirmbtn.UseVisualStyleBackColor = false;
            this.Confirmbtn.Click += new System.EventHandler(this.Confirmbtn_Click);
            // 
            // Cancelbtn
            // 
            this.Cancelbtn.BackColor = System.Drawing.Color.Transparent;
            this.Cancelbtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cancelbtn.FlatAppearance.BorderSize = 2;
            this.Cancelbtn.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cancelbtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.Cancelbtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Cancelbtn.Location = new System.Drawing.Point(134, 123);
            this.Cancelbtn.Name = "Cancelbtn";
            this.Cancelbtn.Size = new System.Drawing.Size(62, 31);
            this.Cancelbtn.TabIndex = 6;
            this.Cancelbtn.Text = "Cancel";
            this.Cancelbtn.UseVisualStyleBackColor = false;
            this.Cancelbtn.Click += new System.EventHandler(this.Cancelbtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BarangayApplication.Properties.Resources.warning;
            this.pictureBox1.Location = new System.Drawing.Point(11, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 29);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // BackupRestoreConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(294, 167);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancelbtn);
            this.Controls.Add(this.Confirmbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BackupRestoreConfirmation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Confirmation";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Confirmbtn;
        private System.Windows.Forms.Button Cancelbtn;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}