namespace BarangayApplication
{
    partial class Overview
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Overview));
            this.lblOverview = new System.Windows.Forms.Label();
            this.panelOverview = new System.Windows.Forms.Panel();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.chartOverview = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblChartTopic = new System.Windows.Forms.Label();
            this.pnlChart = new System.Windows.Forms.Panel();
            this.picNext = new System.Windows.Forms.PictureBox();
            this.picPrevious = new System.Windows.Forms.PictureBox();
            this.lblData1 = new System.Windows.Forms.Label();
            this.lblData2 = new System.Windows.Forms.Label();
            this.lblData3 = new System.Windows.Forms.Label();
            this.lblData4 = new System.Windows.Forms.Label();
            this.lblData5 = new System.Windows.Forms.Label();
            this.lblData6 = new System.Windows.Forms.Label();
            this.pnlTotalResidentsApplied = new System.Windows.Forms.Panel();
            this.lblTotalResApplied = new System.Windows.Forms.Label();
            this.lblTotalResidentsApplied = new System.Windows.Forms.Label();
            this.pbPerson = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTotalAppMonth = new System.Windows.Forms.Label();
            this.pbPaper = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblQuickShortcuts = new System.Windows.Forms.Label();
            this.tipCreateNew = new System.Windows.Forms.ToolTip(this.components);
            this.pbSearch = new System.Windows.Forms.PictureBox();
            this.pbCreateNew = new System.Windows.Forms.PictureBox();
            this.tipSearch = new System.Windows.Forms.ToolTip(this.components);
            this.Box1 = new System.Windows.Forms.Panel();
            this.Box2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chartOverview)).BeginInit();
            this.pnlChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPrevious)).BeginInit();
            this.pnlTotalResidentsApplied.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPerson)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPaper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreateNew)).BeginInit();
            this.Box1.SuspendLayout();
            this.Box2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Font = new System.Drawing.Font("Franklin Gothic Medium", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverview.Location = new System.Drawing.Point(13, 20);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(177, 38);
            this.lblOverview.TabIndex = 0;
            this.lblOverview.Text = "OVERVIEW";
            // 
            // panelOverview
            // 
            this.panelOverview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelOverview.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelOverview.Location = new System.Drawing.Point(12, 114);
            this.panelOverview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelOverview.Name = "panelOverview";
            this.panelOverview.Size = new System.Drawing.Size(1709, 2);
            this.panelOverview.TabIndex = 1;
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.Location = new System.Drawing.Point(13, 59);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(233, 41);
            this.lblGreeting.TabIndex = 2;
            this.lblGreeting.Text = "[Greeting here]";
            // 
            // lblDateTime
            // 
            this.lblDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(1064, 62);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(153, 38);
            this.lblDateTime.TabIndex = 3;
            this.lblDateTime.Text = "[date-time]";
            // 
            // chartOverview
            // 
            this.chartOverview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chartOverview.BackColor = System.Drawing.SystemColors.Window;
            chartArea1.Name = "ChartArea1";
            this.chartOverview.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartOverview.Legends.Add(legend1);
            this.chartOverview.Location = new System.Drawing.Point(63, 46);
            this.chartOverview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chartOverview.Name = "chartOverview";
            this.chartOverview.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartOverview.Series.Add(series1);
            this.chartOverview.Size = new System.Drawing.Size(923, 466);
            this.chartOverview.TabIndex = 4;
            this.chartOverview.Text = "chartOverview";
            this.chartOverview.Click += new System.EventHandler(this.chartOverview_Click);
            // 
            // lblChartTopic
            // 
            this.lblChartTopic.AutoSize = true;
            this.lblChartTopic.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChartTopic.Location = new System.Drawing.Point(49, 39);
            this.lblChartTopic.Name = "lblChartTopic";
            this.lblChartTopic.Size = new System.Drawing.Size(182, 41);
            this.lblChartTopic.TabIndex = 7;
            this.lblChartTopic.Text = "[Data Type]";
            // 
            // pnlChart
            // 
            this.pnlChart.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pnlChart.Controls.Add(this.picNext);
            this.pnlChart.Controls.Add(this.chartOverview);
            this.pnlChart.Controls.Add(this.picPrevious);
            this.pnlChart.Location = new System.Drawing.Point(575, 374);
            this.pnlChart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlChart.Name = "pnlChart";
            this.pnlChart.Size = new System.Drawing.Size(1084, 527);
            this.pnlChart.TabIndex = 8;
            // 
            // picNext
            // 
            this.picNext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picNext.Image = global::BarangayApplication.Properties.Resources.right_arrow;
            this.picNext.Location = new System.Drawing.Point(992, 263);
            this.picNext.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picNext.Name = "picNext";
            this.picNext.Size = new System.Drawing.Size(51, 50);
            this.picNext.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picNext.TabIndex = 6;
            this.picNext.TabStop = false;
            this.picNext.Click += new System.EventHandler(this.picNext_Click);
            // 
            // picPrevious
            // 
            this.picPrevious.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPrevious.Image = ((System.Drawing.Image)(resources.GetObject("picPrevious.Image")));
            this.picPrevious.Location = new System.Drawing.Point(7, 263);
            this.picPrevious.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picPrevious.Name = "picPrevious";
            this.picPrevious.Size = new System.Drawing.Size(51, 50);
            this.picPrevious.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPrevious.TabIndex = 5;
            this.picPrevious.TabStop = false;
            this.picPrevious.Click += new System.EventHandler(this.picPrevious_Click);
            // 
            // lblData1
            // 
            this.lblData1.AutoSize = true;
            this.lblData1.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData1.Location = new System.Drawing.Point(56, 103);
            this.lblData1.Name = "lblData1";
            this.lblData1.Size = new System.Drawing.Size(74, 25);
            this.lblData1.TabIndex = 9;
            this.lblData1.Text = "[Data 1]";
            // 
            // lblData2
            // 
            this.lblData2.AutoSize = true;
            this.lblData2.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData2.Location = new System.Drawing.Point(56, 138);
            this.lblData2.Name = "lblData2";
            this.lblData2.Size = new System.Drawing.Size(77, 25);
            this.lblData2.TabIndex = 10;
            this.lblData2.Text = "[Data 2]";
            // 
            // lblData3
            // 
            this.lblData3.AutoSize = true;
            this.lblData3.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData3.Location = new System.Drawing.Point(56, 174);
            this.lblData3.Name = "lblData3";
            this.lblData3.Size = new System.Drawing.Size(77, 25);
            this.lblData3.TabIndex = 11;
            this.lblData3.Text = "[Data 3]";
            // 
            // lblData4
            // 
            this.lblData4.AutoSize = true;
            this.lblData4.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData4.Location = new System.Drawing.Point(56, 208);
            this.lblData4.Name = "lblData4";
            this.lblData4.Size = new System.Drawing.Size(77, 25);
            this.lblData4.TabIndex = 12;
            this.lblData4.Text = "[Data 4]";
            // 
            // lblData5
            // 
            this.lblData5.AutoSize = true;
            this.lblData5.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData5.Location = new System.Drawing.Point(56, 244);
            this.lblData5.Name = "lblData5";
            this.lblData5.Size = new System.Drawing.Size(77, 25);
            this.lblData5.TabIndex = 13;
            this.lblData5.Text = "[Data 5]";
            // 
            // lblData6
            // 
            this.lblData6.AutoSize = true;
            this.lblData6.Font = new System.Drawing.Font("Segoe UI Semibold", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData6.Location = new System.Drawing.Point(56, 279);
            this.lblData6.Name = "lblData6";
            this.lblData6.Size = new System.Drawing.Size(77, 25);
            this.lblData6.TabIndex = 14;
            this.lblData6.Text = "[Data 6]";
            // 
            // pnlTotalResidentsApplied
            // 
            this.pnlTotalResidentsApplied.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTotalResidentsApplied.BackColor = System.Drawing.SystemColors.Window;
            this.pnlTotalResidentsApplied.Controls.Add(this.lblTotalResApplied);
            this.pnlTotalResidentsApplied.Controls.Add(this.lblTotalResidentsApplied);
            this.pnlTotalResidentsApplied.Controls.Add(this.pbPerson);
            this.pnlTotalResidentsApplied.Location = new System.Drawing.Point(617, 135);
            this.pnlTotalResidentsApplied.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlTotalResidentsApplied.Name = "pnlTotalResidentsApplied";
            this.pnlTotalResidentsApplied.Size = new System.Drawing.Size(463, 208);
            this.pnlTotalResidentsApplied.TabIndex = 15;
            // 
            // lblTotalResApplied
            // 
            this.lblTotalResApplied.AutoSize = true;
            this.lblTotalResApplied.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalResApplied.Location = new System.Drawing.Point(169, 126);
            this.lblTotalResApplied.Name = "lblTotalResApplied";
            this.lblTotalResApplied.Size = new System.Drawing.Size(212, 54);
            this.lblTotalResApplied.TabIndex = 2;
            this.lblTotalResApplied.Text = "(total no.)";
            this.lblTotalResApplied.Click += new System.EventHandler(this.lblTotalResApplied_Click);
            // 
            // lblTotalResidentsApplied
            // 
            this.lblTotalResidentsApplied.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalResidentsApplied.Location = new System.Drawing.Point(109, 7);
            this.lblTotalResidentsApplied.Name = "lblTotalResidentsApplied";
            this.lblTotalResidentsApplied.Size = new System.Drawing.Size(333, 97);
            this.lblTotalResidentsApplied.TabIndex = 1;
            this.lblTotalResidentsApplied.Text = "TOTAL RESIDENTS APPLIED";
            this.lblTotalResidentsApplied.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbPerson
            // 
            this.pbPerson.Image = global::BarangayApplication.Properties.Resources.person1;
            this.pbPerson.Location = new System.Drawing.Point(7, 7);
            this.pbPerson.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbPerson.Name = "pbPerson";
            this.pbPerson.Size = new System.Drawing.Size(97, 97);
            this.pbPerson.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPerson.TabIndex = 0;
            this.pbPerson.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.lblTotalAppMonth);
            this.panel2.Controls.Add(this.pbPaper);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(1128, 135);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(531, 208);
            this.panel2.TabIndex = 16;
            // 
            // lblTotalAppMonth
            // 
            this.lblTotalAppMonth.AutoSize = true;
            this.lblTotalAppMonth.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAppMonth.Location = new System.Drawing.Point(207, 126);
            this.lblTotalAppMonth.Name = "lblTotalAppMonth";
            this.lblTotalAppMonth.Size = new System.Drawing.Size(212, 54);
            this.lblTotalAppMonth.TabIndex = 3;
            this.lblTotalAppMonth.Text = "(total no.)";
            // 
            // pbPaper
            // 
            this.pbPaper.Image = global::BarangayApplication.Properties.Resources.paper1;
            this.pbPaper.Location = new System.Drawing.Point(8, 7);
            this.pbPaper.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbPaper.Name = "pbPaper";
            this.pbPaper.Size = new System.Drawing.Size(97, 97);
            this.pbPaper.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPaper.TabIndex = 3;
            this.pbPaper.TabStop = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(123, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(379, 97);
            this.label2.TabIndex = 2;
            this.label2.Text = "TOTAL APPLICATIONS THIS MONTH\r\n";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQuickShortcuts
            // 
            this.lblQuickShortcuts.AutoSize = true;
            this.lblQuickShortcuts.BackColor = System.Drawing.Color.Transparent;
            this.lblQuickShortcuts.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuickShortcuts.Location = new System.Drawing.Point(99, 5);
            this.lblQuickShortcuts.Name = "lblQuickShortcuts";
            this.lblQuickShortcuts.Size = new System.Drawing.Size(320, 54);
            this.lblQuickShortcuts.TabIndex = 17;
            this.lblQuickShortcuts.Text = "Quick Shortcuts";
            // 
            // tipCreateNew
            // 
            this.tipCreateNew.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // pbSearch
            // 
            this.pbSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSearch.Image = global::BarangayApplication.Properties.Resources.search1;
            this.pbSearch.Location = new System.Drawing.Point(284, 85);
            this.pbSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbSearch.Name = "pbSearch";
            this.pbSearch.Size = new System.Drawing.Size(161, 161);
            this.pbSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSearch.TabIndex = 19;
            this.pbSearch.TabStop = false;
            this.pbSearch.Click += new System.EventHandler(this.pbSearch_Click);
            // 
            // pbCreateNew
            // 
            this.pbCreateNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbCreateNew.Image = global::BarangayApplication.Properties.Resources.CreateNew;
            this.pbCreateNew.Location = new System.Drawing.Point(43, 85);
            this.pbCreateNew.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pbCreateNew.Name = "pbCreateNew";
            this.pbCreateNew.Size = new System.Drawing.Size(161, 161);
            this.pbCreateNew.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCreateNew.TabIndex = 18;
            this.pbCreateNew.TabStop = false;
            this.pbCreateNew.Click += new System.EventHandler(this.pbCreateNew_Click);
            // 
            // Box1
            // 
            this.Box1.BackColor = System.Drawing.SystemColors.Window;
            this.Box1.Controls.Add(this.lblQuickShortcuts);
            this.Box1.Controls.Add(this.pbCreateNew);
            this.Box1.Controls.Add(this.pbSearch);
            this.Box1.Location = new System.Drawing.Point(44, 135);
            this.Box1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Box1.Name = "Box1";
            this.Box1.Size = new System.Drawing.Size(497, 281);
            this.Box1.TabIndex = 20;
            // 
            // Box2
            // 
            this.Box2.BackColor = System.Drawing.SystemColors.Window;
            this.Box2.Controls.Add(this.lblChartTopic);
            this.Box2.Controls.Add(this.lblData1);
            this.Box2.Controls.Add(this.lblData6);
            this.Box2.Controls.Add(this.lblData2);
            this.Box2.Controls.Add(this.lblData5);
            this.Box2.Controls.Add(this.lblData3);
            this.Box2.Controls.Add(this.lblData4);
            this.Box2.Location = new System.Drawing.Point(44, 442);
            this.Box2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Box2.Name = "Box2";
            this.Box2.Size = new System.Drawing.Size(497, 482);
            this.Box2.TabIndex = 21;
            // 
            // Overview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(1733, 935);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlTotalResidentsApplied);
            this.Controls.Add(this.pnlChart);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblGreeting);
            this.Controls.Add(this.panelOverview);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.Box1);
            this.Controls.Add(this.Box2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Overview";
            this.Text = "Overview";
            this.Load += new System.EventHandler(this.Overview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartOverview)).EndInit();
            this.pnlChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPrevious)).EndInit();
            this.pnlTotalResidentsApplied.ResumeLayout(false);
            this.pnlTotalResidentsApplied.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPerson)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPaper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCreateNew)).EndInit();
            this.Box1.ResumeLayout(false);
            this.Box1.PerformLayout();
            this.Box2.ResumeLayout(false);
            this.Box2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.Panel panelOverview;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartOverview;
        private System.Windows.Forms.PictureBox picPrevious;
        private System.Windows.Forms.PictureBox picNext;
        private System.Windows.Forms.Label lblChartTopic;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.Label lblData1;
        private System.Windows.Forms.Label lblData2;
        private System.Windows.Forms.Label lblData3;
        private System.Windows.Forms.Label lblData4;
        private System.Windows.Forms.Label lblData5;
        private System.Windows.Forms.Label lblData6;
        private System.Windows.Forms.Panel pnlTotalResidentsApplied;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pbPerson;
        private System.Windows.Forms.Label lblTotalResidentsApplied;
        private System.Windows.Forms.Label lblTotalResApplied;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pbPaper;
        private System.Windows.Forms.Label lblTotalAppMonth;
        private System.Windows.Forms.Label lblQuickShortcuts;
        private System.Windows.Forms.PictureBox pbCreateNew;
        private System.Windows.Forms.ToolTip tipCreateNew;
        private System.Windows.Forms.PictureBox pbSearch;
        private System.Windows.Forms.ToolTip tipSearch;
        private System.Windows.Forms.Panel Box1;
        private System.Windows.Forms.Panel Box2;
    }
}