namespace BarangayApplication
{
    partial class LogoutConfirm
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
            this.Confirmbtn = new System.Windows.Forms.Button();
            this.Cancelbtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Confirmbtn
            // 
            this.Confirmbtn.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Confirmbtn.ForeColor = System.Drawing.Color.White;
            this.Confirmbtn.Location = new System.Drawing.Point(296, 155);
            this.Confirmbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Confirmbtn.Name = "Confirmbtn";
            this.Confirmbtn.Size = new System.Drawing.Size(83, 38);
            this.Confirmbtn.TabIndex = 0;
            this.Confirmbtn.Text = "Confirm";
            this.Confirmbtn.UseVisualStyleBackColor = false;
            this.Confirmbtn.Click += new System.EventHandler(this.Confirmbtn_Click);
            // 
            // Cancelbtn
            // 
            this.Cancelbtn.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Cancelbtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.AppWorkspace;
            this.Cancelbtn.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cancelbtn.Location = new System.Drawing.Point(178, 155);
            this.Cancelbtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Cancelbtn.Name = "Cancelbtn";
            this.Cancelbtn.Size = new System.Drawing.Size(83, 38);
            this.Cancelbtn.TabIndex = 1;
            this.Cancelbtn.Text = "Cancel";
            this.Cancelbtn.UseVisualStyleBackColor = false;
            this.Cancelbtn.Click += new System.EventHandler(this.Cancelbtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Medium", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 34);
            this.label1.TabIndex = 2;
            this.label1.Text = "Confirm Logout";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Location = new System.Drawing.Point(11, 54);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 2);
            this.panel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Are you sure you want to logout?";
            // 
            // LogoutConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(392, 206);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancelbtn);
            this.Controls.Add(this.Confirmbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LogoutConfirm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LogoutConfirm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Confirmbtn;
        private System.Windows.Forms.Button Cancelbtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
    }
}