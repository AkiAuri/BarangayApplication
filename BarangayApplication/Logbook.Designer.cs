namespace BarangayApplication
{
    partial class Logbook
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Bar = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLogbook = new System.Windows.Forms.Panel();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.cBxFilterByAction = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.llClearFilter = new System.Windows.Forms.LinkLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblUserFilter = new System.Windows.Forms.Label();
            this.cbxUserFilter = new System.Windows.Forms.ComboBox();
            this.btnLogPrinter = new System.Windows.Forms.Button();
            this.panelLogbook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Bar
            // 
            this.Bar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.Bar.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Bar.Location = new System.Drawing.Point(8, 54);
            this.Bar.Margin = new System.Windows.Forms.Padding(2);
            this.Bar.Name = "Bar";
            this.Bar.Size = new System.Drawing.Size(772, 2);
            this.Bar.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Medium", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 34);
            this.label1.TabIndex = 2;
            this.label1.Text = "Logbook";
            // 
            // panelLogbook
            // 
            this.panelLogbook.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLogbook.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panelLogbook.Controls.Add(this.dgvLog);
            this.panelLogbook.Location = new System.Drawing.Point(8, 93);
            this.panelLogbook.Margin = new System.Windows.Forms.Padding(2);
            this.panelLogbook.Name = "panelLogbook";
            this.panelLogbook.Size = new System.Drawing.Size(772, 406);
            this.panelLogbook.TabIndex = 5;
            // 
            // dgvLog
            // 
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.AllowUserToResizeRows = false;
            this.dgvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 10.2F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLog.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLog.Location = new System.Drawing.Point(0, 0);
            this.dgvLog.Margin = new System.Windows.Forms.Padding(2);
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.ReadOnly = true;
            this.dgvLog.RowHeadersVisible = false;
            this.dgvLog.RowHeadersWidth = 51;
            this.dgvLog.RowTemplate.Height = 24;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.Size = new System.Drawing.Size(772, 406);
            this.dgvLog.TabIndex = 0;
            // 
            // btnFirst
            // 
            this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFirst.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirst.Location = new System.Drawing.Point(10, 513);
            this.btnFirst.Margin = new System.Windows.Forms.Padding(2);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(56, 28);
            this.btnFirst.TabIndex = 6;
            this.btnFirst.Text = "First";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevious.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrevious.Location = new System.Drawing.Point(82, 513);
            this.btnPrevious.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(78, 28);
            this.btnPrevious.TabIndex = 7;
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(178, 513);
            this.btnNext.Margin = new System.Windows.Forms.Padding(2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(56, 28);
            this.btnNext.TabIndex = 8;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLast.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLast.Location = new System.Drawing.Point(252, 513);
            this.btnLast.Margin = new System.Windows.Forms.Padding(2);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(56, 28);
            this.btnLast.TabIndex = 9;
            this.btnLast.Text = "Last";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // lblPageInfo
            // 
            this.lblPageInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageInfo.AutoSize = true;
            this.lblPageInfo.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageInfo.Location = new System.Drawing.Point(652, 518);
            this.lblPageInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(97, 19);
            this.lblPageInfo.TabIndex = 10;
            this.lblPageInfo.Text = "(Page X of Y)";
            // 
            // cBxFilterByAction
            // 
            this.cBxFilterByAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBxFilterByAction.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBxFilterByAction.FormattingEnabled = true;
            this.cBxFilterByAction.Items.AddRange(new object[] { "ADD", "EDIT", "ARCHIVE" });
            this.cBxFilterByAction.Location = new System.Drawing.Point(118, 3);
            this.cBxFilterByAction.Margin = new System.Windows.Forms.Padding(2);
            this.cBxFilterByAction.Name = "cBxFilterByAction";
            this.cBxFilterByAction.Size = new System.Drawing.Size(98, 27);
            this.cBxFilterByAction.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cBxFilterByAction);
            this.panel1.Location = new System.Drawing.Point(8, 58);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(218, 32);
            this.panel1.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 19);
            this.label2.TabIndex = 12;
            this.label2.Text = "Filter by Action:";
            // 
            // llClearFilter
            // 
            this.llClearFilter.AutoSize = true;
            this.llClearFilter.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llClearFilter.Location = new System.Drawing.Point(452, 64);
            this.llClearFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.llClearFilter.Name = "llClearFilter";
            this.llClearFilter.Size = new System.Drawing.Size(74, 19);
            this.llClearFilter.TabIndex = 13;
            this.llClearFilter.TabStop = true;
            this.llClearFilter.Text = "Clear Filter";
            this.llClearFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llClearFilter_LinkClicked);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblUserFilter);
            this.panel2.Controls.Add(this.cbxUserFilter);
            this.panel2.Location = new System.Drawing.Point(230, 58);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(218, 32);
            this.panel2.TabIndex = 13;
            // 
            // lblUserFilter
            // 
            this.lblUserFilter.AutoSize = true;
            this.lblUserFilter.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserFilter.Location = new System.Drawing.Point(2, 6);
            this.lblUserFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUserFilter.Name = "lblUserFilter";
            this.lblUserFilter.Size = new System.Drawing.Size(96, 19);
            this.lblUserFilter.TabIndex = 12;
            this.lblUserFilter.Text = "Filter by User:";
            // 
            // cbxUserFilter
            // 
            this.cbxUserFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxUserFilter.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxUserFilter.FormattingEnabled = true;
            this.cbxUserFilter.Items.AddRange(new object[] { "ADD", "EDIT", "ARCHIVE" });
            this.cbxUserFilter.Location = new System.Drawing.Point(118, 3);
            this.cbxUserFilter.Margin = new System.Windows.Forms.Padding(2);
            this.cbxUserFilter.Name = "cbxUserFilter";
            this.cbxUserFilter.Size = new System.Drawing.Size(98, 27);
            this.cbxUserFilter.TabIndex = 11;
            // 
            // btnLogPrinter
            // 
            this.btnLogPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogPrinter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLogPrinter.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogPrinter.Location = new System.Drawing.Point(721, 60);
            this.btnLogPrinter.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogPrinter.Name = "btnLogPrinter";
            this.btnLogPrinter.Size = new System.Drawing.Size(56, 28);
            this.btnLogPrinter.TabIndex = 14;
            this.btnLogPrinter.Text = "Print";
            this.btnLogPrinter.UseVisualStyleBackColor = true;
            this.btnLogPrinter.Click += new System.EventHandler(this.btnLogPrinter_Click);
            // 
            // Logbook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 557);
            this.Controls.Add(this.btnLogPrinter);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.llClearFilter);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblPageInfo);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.panelLogbook);
            this.Controls.Add(this.Bar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Logbook";
            this.Text = "Logbook";
            this.Load += new System.EventHandler(this.Logbook_Load);
            this.panelLogbook.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblUserFilter;
        private System.Windows.Forms.ComboBox cbxUserFilter;
        private System.Windows.Forms.Button btnLogPrinter;

        #endregion

        private System.Windows.Forms.Panel Bar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelLogbook;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cBxFilterByAction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel llClearFilter;
    }
}