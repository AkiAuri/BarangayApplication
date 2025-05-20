using BarangayApplication.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BarangayApplication
{
    public partial class Logbook: Form
    {
        private int currentPage = 1;
        private int totalPages = 0;
        private const int rowsPerPage = 37;
        private bool isLogUpdateInProgress = false;

        public Logbook()
        {
            InitializeComponent();
            cBxFilterByAction.SelectedIndexChanged += cBxFilterByAction_SelectedIndexChanged;

            // Set filter options in ALL UPPERCASE
            cBxFilterByAction.Items.Clear();
            cBxFilterByAction.Items.AddRange(new string[] {
                "ADD",
                "EDIT",
                "ARCHIVE",
                "RESTORE",
                "LOGIN"
            });
        }

        private void LoadDataForPage(int page)
        {
            var repo = new ResidentsRepository();
            int offset = (page - 1) * rowsPerPage;

            // Fetch logs for the current page
            var logs = repo.GetLogsForPage(offset, rowsPerPage);

            //bind data to dgv
            dgvLog.DataSource = logs;

            //for the paging text label
            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";

            // Enable/disable navigation buttons as appropriate
            btnFirst.Enabled = btnPrevious.Enabled = (currentPage > 1);
            btnNext.Enabled = btnLast.Enabled = (currentPage < totalPages);
        }
        private void LoadFilteredLogs(string action)
        {
            var repo = new ResidentsRepository();
            var logs = repo.GetFilteredLogs(action);

            dgvLog.DataSource = logs;
            lblPageInfo.Text = $"Filtered by: {action}";

            // Disable navigation buttons during filter
            btnFirst.Enabled = btnPrevious.Enabled = btnNext.Enabled = btnLast.Enabled = false;
        }


        private void CalculateTotalPages()
        {
            var repo = new ResidentsRepository();
            int totalLogs = repo.GetTotalLogCount();
            totalPages = (int)Math.Ceiling((double)totalLogs / rowsPerPage);
        }

        private void Logbook_Load(object sender, EventArgs e)
        {
            Bar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CalculateTotalPages();
            LoadDataForPage(currentPage); // Load all logs initially
        }


        private void btnFirst_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadDataForPage(currentPage);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            PerformLogUpdate(() =>
            {
                if (currentPage > 1){
                    currentPage--;
                    LoadDataForPage(currentPage);
                }
            });
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            PerformLogUpdate(() =>
            {
                if (currentPage < totalPages){
                    currentPage++;
                    LoadDataForPage(currentPage);
                }
            });
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            currentPage = totalPages;
            LoadDataForPage(currentPage);
        }
        private void cBxFilterByAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedAction = cBxFilterByAction.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedAction))
            {
                LoadFilteredLogs(selectedAction);
            }
        }
        private void llClearFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Reload all logs
            LoadDataForPage(currentPage);

            // Clear the comboBox selection
            cBxFilterByAction.SelectedIndex = -1;

            // Optionally reset the comboBox text
            cBxFilterByAction.Text = "Select Action";
        }

        private void PerformLogUpdate(Action logAction)
        {
            if (isLogUpdateInProgress) return;

            try
            {
                isLogUpdateInProgress = true;
                logAction();
            }
            finally
            {
                isLogUpdateInProgress = false;
            }
        }
    }
}
