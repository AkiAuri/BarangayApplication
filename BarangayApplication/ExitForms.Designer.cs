namespace BarangayApplication
{
    partial class ExitForms
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Leavebtn = new System.Windows.Forms.Button();
            this.Staybtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(52, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Any changes will not be saved!";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(76, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Are you sure you want to leave?";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BarangayApplication.Properties.Resources.warning;
            this.pictureBox1.Location = new System.Drawing.Point(9, 14);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 36);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(9, 54);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(447, 2);
            this.panel1.TabIndex = 3;
            // 
            // Leavebtn
            // 
            this.Leavebtn.BackColor = System.Drawing.Color.DarkRed;
            this.Leavebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.Leavebtn.ForeColor = System.Drawing.Color.White;
            this.Leavebtn.Location = new System.Drawing.Point(280, 170);
            this.Leavebtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Leavebtn.Name = "Leavebtn";
            this.Leavebtn.Size = new System.Drawing.Size(106, 40);
            this.Leavebtn.TabIndex = 4;
            this.Leavebtn.Text = "Leave";
            this.Leavebtn.UseVisualStyleBackColor = false;
            this.Leavebtn.Click += new System.EventHandler(this.Leavebtn_Click);
            // 
            // Staybtn
            // 
            this.Staybtn.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Staybtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Staybtn.ForeColor = System.Drawing.Color.White;
            this.Staybtn.Location = new System.Drawing.Point(80, 170);
            this.Staybtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Staybtn.Name = "Staybtn";
            this.Staybtn.Size = new System.Drawing.Size(106, 40);
            this.Staybtn.TabIndex = 5;
            this.Staybtn.Text = "Stay";
            this.Staybtn.UseVisualStyleBackColor = false;
            this.Staybtn.Click += new System.EventHandler(this.Staybtn_Click);
            // 
            // ExitForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.NavajoWhite;
            this.ClientSize = new System.Drawing.Size(476, 220);
            this.Controls.Add(this.Staybtn);
            this.Controls.Add(this.Leavebtn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ExitForms";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExitForms";
            this.Load += new System.EventHandler(this.ExitForms_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Leavebtn;
        private System.Windows.Forms.Button Staybtn;
    }
}