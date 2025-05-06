namespace BarangayApplication
{
    partial class MainViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainViewForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.TopBar = new System.Windows.Forms.Panel();
            this.occuBtn = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.collectionBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.personalBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.TopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(252, 433);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(960, 50);
            this.panel1.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.Red;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(828, 3);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 32);
            this.button2.TabIndex = 199;
            this.button2.Text = "CANCEL";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LawnGreen;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(733, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 32);
            this.button1.TabIndex = 200;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(252, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(960, 483);
            this.MainPanel.TabIndex = 13;
            // 
            // TopBar
            // 
            this.TopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.TopBar.Controls.Add(this.occuBtn);
            this.TopBar.Controls.Add(this.panel5);
            this.TopBar.Controls.Add(this.collectionBtn);
            this.TopBar.Controls.Add(this.panel3);
            this.TopBar.Controls.Add(this.personalBtn);
            this.TopBar.Controls.Add(this.panel2);
            this.TopBar.Controls.Add(this.panel10);
            this.TopBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.TopBar.Location = new System.Drawing.Point(0, 0);
            this.TopBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TopBar.Name = "TopBar";
            this.TopBar.Size = new System.Drawing.Size(252, 483);
            this.TopBar.TabIndex = 12;
            // 
            // occuBtn
            // 
            this.occuBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.occuBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.occuBtn.FlatAppearance.BorderSize = 0;
            this.occuBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.occuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.occuBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.occuBtn.ForeColor = System.Drawing.Color.LightGray;
            this.occuBtn.Image = global::BarangayApplication.Properties.Resources.employee_man;
            this.occuBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.occuBtn.Location = new System.Drawing.Point(0, 332);
            this.occuBtn.Name = "occuBtn";
            this.occuBtn.Size = new System.Drawing.Size(252, 83);
            this.occuBtn.TabIndex = 17;
            this.occuBtn.Text = "Occupation";
            this.occuBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.occuBtn.UseVisualStyleBackColor = true;
            this.occuBtn.Click += new System.EventHandler(this.occuBtn_Click);
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 316);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(252, 16);
            this.panel5.TabIndex = 23;
            // 
            // collectionBtn
            // 
            this.collectionBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.collectionBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.collectionBtn.FlatAppearance.BorderSize = 0;
            this.collectionBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.collectionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.collectionBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.collectionBtn.ForeColor = System.Drawing.Color.LightGray;
            this.collectionBtn.Image = global::BarangayApplication.Properties.Resources.album_collection;
            this.collectionBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.collectionBtn.Location = new System.Drawing.Point(0, 233);
            this.collectionBtn.Name = "collectionBtn";
            this.collectionBtn.Size = new System.Drawing.Size(252, 83);
            this.collectionBtn.TabIndex = 15;
            this.collectionBtn.Text = "Collection";
            this.collectionBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.collectionBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.collectionBtn.UseVisualStyleBackColor = true;
            this.collectionBtn.Click += new System.EventHandler(this.collectionBtn_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 217);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(252, 16);
            this.panel3.TabIndex = 21;
            // 
            // personalBtn
            // 
            this.personalBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.personalBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.personalBtn.FlatAppearance.BorderSize = 0;
            this.personalBtn.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.personalBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.personalBtn.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.personalBtn.ForeColor = System.Drawing.Color.LightGray;
            this.personalBtn.Image = global::BarangayApplication.Properties.Resources.terms_info;
            this.personalBtn.Location = new System.Drawing.Point(0, 134);
            this.personalBtn.Name = "personalBtn";
            this.personalBtn.Size = new System.Drawing.Size(252, 83);
            this.personalBtn.TabIndex = 13;
            this.personalBtn.Text = "Personal Info";
            this.personalBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.personalBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.personalBtn.UseVisualStyleBackColor = true;
            this.personalBtn.Click += new System.EventHandler(this.personalBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 16);
            this.panel2.TabIndex = 20;
            // 
            // panel10
            // 
            this.panel10.BackgroundImage = global::BarangayApplication.Properties.Resources.Residence;
            this.panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(252, 118);
            this.panel10.TabIndex = 19;
            // 
            // MainViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(230)))));
            this.ClientSize = new System.Drawing.Size(1212, 483);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.TopBar);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MainViewForm";
            this.Load += new System.EventHandler(this.MainViewForm_Load);
            this.panel1.ResumeLayout(false);
            this.TopBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button occuBtn;
        private System.Windows.Forms.Button collectionBtn;
        private System.Windows.Forms.Button personalBtn;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Panel TopBar;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}