namespace BarangayApplication
{
    partial class Backup
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
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Confirmbtn = new System.Windows.Forms.Button();
            this.Cancelbtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(266, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "Restore from this backup file?";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Location = new System.Drawing.Point(12, 50);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 2);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Medium", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(61, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 34);
            this.label1.TabIndex = 7;
            this.label1.Text = "Confirmation";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // Confirmbtn
            // 
            this.Confirmbtn.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Confirmbtn.ForeColor = System.Drawing.Color.White;
            this.Confirmbtn.Location = new System.Drawing.Point(297, 151);
            this.Confirmbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Confirmbtn.Name = "Confirmbtn";
            this.Confirmbtn.Size = new System.Drawing.Size(83, 38);
            this.Confirmbtn.TabIndex = 5;
            this.Confirmbtn.Text = "Confirm";
            this.Confirmbtn.UseVisualStyleBackColor = false;
            // 
            // Cancelbtn
            // 
            this.Cancelbtn.BackColor = System.Drawing.Color.Transparent;
            this.Cancelbtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cancelbtn.FlatAppearance.BorderSize = 2;
            this.Cancelbtn.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cancelbtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.Cancelbtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Cancelbtn.Location = new System.Drawing.Point(179, 151);
            this.Cancelbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cancelbtn.Name = "Cancelbtn";
            this.Cancelbtn.Size = new System.Drawing.Size(83, 38);
            this.Cancelbtn.TabIndex = 6;
            this.Cancelbtn.Text = "Cancel";
            this.Cancelbtn.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BarangayApplication.Properties.Resources.warning;
            this.pictureBox1.Location = new System.Drawing.Point(15, 13);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 36);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // Backup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(392, 206);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancelbtn);
            this.Controls.Add(this.Confirmbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Backup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Occupation";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Confirmbtn;
        private System.Windows.Forms.Button Cancelbtn;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}