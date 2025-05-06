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
        private const int rowsPerPage = 50;

        public Logbook()
        {
            InitializeComponent();
        }

        private void LoadDataForPage(int page)
        {
            var repo = new ResidentsRepository();
            int offset = (page - 1) * rowsPerPage;

            // Fetch logs for the current page
            var logs = repo.GetLogsForPage(offset, rowsPerPage);

            // Bind logs to the DataGridView
            dgvLog.DataSource = logs;

            // Update pagination labels (if any)
            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
        }
        private void CalculateTotalPages()
        {
            var repo = new ResidentsRepository();
            int totalLogs = repo.GetTotalLogCount();
            totalPages = (int)Math.Ceiling((double)totalLogs / rowsPerPage);
        }

        private void Logbook_Load(object sender, EventArgs e)
        {
            Bar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; // Ensure the Bar panel resizes its width
            CalculateTotalPages();
            LoadDataForPage(currentPage);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadDataForPage(currentPage);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1){
                currentPage--;
                LoadDataForPage(currentPage);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages){
                currentPage++;
                LoadDataForPage(currentPage);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            currentPage = totalPages;
            LoadDataForPage(currentPage);
        }

        private void dgvLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
