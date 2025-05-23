using BarangayApplication.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BarangayApplication
{
    public partial class Logbook : Form
    {
        private int currentPage = 1;
        private int totalPages = 0;
        private const int rowsPerPage = 28;
        private bool isLogUpdateInProgress = false;

        public Logbook()
        {
            InitializeComponent();

            // Add alternating row color (like Archive)
            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            //Add ALL CAPS text.
            dgvLog.CellFormatting += DgvLog_CellFormatting;

            cBxFilterByAction.SelectedIndexChanged += cBxFilterByAction_SelectedIndexChanged;
            cbxUserFilter.SelectedIndexChanged += cbxUserFilter_SelectedIndexChanged;

            cBxFilterByAction.Items.Clear();
            cBxFilterByAction.Items.Add("ALL ACTIONS");
            cBxFilterByAction.Items.AddRange(new string[] {
                                                    "ADD",
                                                    "EDIT",
                                                    "ARCHIVE",
                                                    "RESTORE",
                                                    "LOGIN"
            });
        }

        private void DgvLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value is string)
            {
                e.Value = e.Value.ToString().ToUpper();
                e.FormattingApplied = true;
            }
        }
                                                    

        private void Logbook_Load(object sender, EventArgs e)
        {
            Bar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PopulateUserFilter();
            CalculateTotalPages();
            LoadDataForPage(currentPage);
            cBxFilterByAction.SelectedIndex = 0;
        }

        private void PopulateUserFilter()
        {
            cbxUserFilter.Items.Clear();
            cbxUserFilter.Items.Add("ALL USERS");

            using (var conn = new System.Data.SqlClient.SqlConnection(Settings.ConnectionString))
            {
                string query = "SELECT accountID, accountName FROM users ORDER BY accountName";
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cbxUserFilter.Items.Add(new AccountItem
                                {
                                    AccountID = reader.GetInt32(0),
                                    AccountName = reader.GetString(1)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading users: " + ex.Message);
                    }
                }
            }
            cbxUserFilter.SelectedIndex = 0;
        }

        private void CalculateTotalPages()
        {
            var repo = new ResidentsRepository();
            int totalLogs = repo.GetTotalLogCount();
            totalPages = (int)Math.Ceiling((double)totalLogs / rowsPerPage);
        }

        private void LoadDataForPage(int page)
        {
            var repo = new ResidentsRepository();
            int offset = (page - 1) * rowsPerPage;

            var logs = repo.GetLogsForPage(offset, rowsPerPage);

            dgvLog.DataSource = logs;

            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";

            btnFirst.Enabled = btnPrevious.Enabled = (currentPage > 1);
            btnNext.Enabled = btnLast.Enabled = (currentPage < totalPages);
        }

        private void LoadFilteredLogs(string action, string userName)
        {
            var repo = new ResidentsRepository();
            DataTable logs;

            bool filterByAction = !string.IsNullOrEmpty(action) && action != "ALL ACTIONS";
            bool filterByUser = !string.IsNullOrEmpty(userName) && userName != "ALL USERS";

            if (filterByAction && filterByUser)
            {
                logs = repo.GetLogsByUserAndAction(userName, action);
                lblPageInfo.Text = $"Filtered by: {action} | {userName}";
            }
            else if (filterByAction)
            {
                logs = repo.GetFilteredLogs(action);
                lblPageInfo.Text = $"Filtered by: {action}";
            }
            else if (filterByUser)
            {
                logs = repo.GetLogsByUser(userName);
                lblPageInfo.Text = $"Filtered by user: {userName}";
            }
            else
            {
                LoadDataForPage(1);
                return;
            }

            dgvLog.DataSource = logs;
            btnFirst.Enabled = btnPrevious.Enabled = btnNext.Enabled = btnLast.Enabled = false;
        }

        private string GetCurrentFilterAction()
        {
            return cBxFilterByAction.SelectedItem?.ToString() ?? "ALL ACTIONS";
        }
        private string GetCurrentFilterUser()
        {
            if (cbxUserFilter.SelectedItem is AccountItem user)
                return user.AccountName;
            return cbxUserFilter.SelectedItem?.ToString() ?? "ALL USERS";
        }

        private void cBxFilterByAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFilteredLogs(GetCurrentFilterAction(), GetCurrentFilterUser());
        }

        private void cbxUserFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFilteredLogs(GetCurrentFilterAction(), GetCurrentFilterUser());
        }

        private void llClearFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cBxFilterByAction.SelectedIndex = 0;
            cbxUserFilter.SelectedIndex = 0;
            LoadDataForPage(currentPage);
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
                if (currentPage > 1)
                {
                    currentPage--;
                    LoadDataForPage(currentPage);
                }
            });
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            PerformLogUpdate(() =>
            {
                if (currentPage < totalPages)
                {
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

        // ================== DOCX PRINTER =========================

        private Tuple<int, int> SelectMonthYear()
        {
            // Query available year/month pairs from the database
            // Replace with your actual repository call if needed
            var repo = new ResidentsRepository();
            var available = repo.GetAvailableLogMonths(); // Returns List<Tuple<int, int>>

            var availableYears = available.Select(x => x.Item1).Distinct().OrderBy(y => y).ToList();

            using (var form = new Form())
            {
                form.Text = "Select Month and Year";
                var monthBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = 10, Top = 10, Width = 100 };
                var yearBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Left = 120, Top = 10, Width = 80 };
                var okBtn = new Button { Text = "OK", Left = 210, Width = 80, Top = 10, DialogResult = DialogResult.OK };

                foreach (var y in availableYears)
                    yearBox.Items.Add(y);

                yearBox.SelectedItem = availableYears.LastOrDefault();

                Action updateMonths = () =>
                {
                    monthBox.Items.Clear();
                    int selYear = (int)(yearBox.SelectedItem ?? 0);
                    var months = available.Where(x => x.Item1 == selYear).Select(x => x.Item2).OrderBy(m => m).ToList();
                    foreach (var m in months)
                        monthBox.Items.Add(m);

                    if (monthBox.Items.Count > 0)
                        monthBox.SelectedIndex = 0;
                };

                yearBox.SelectedIndexChanged += (s, e) => updateMonths();

                form.Controls.Add(monthBox);
                form.Controls.Add(yearBox);
                form.Controls.Add(okBtn);
                form.AcceptButton = okBtn;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new System.Drawing.Size(300, 50);

                // Initial population
                updateMonths();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (yearBox.SelectedItem == null || monthBox.SelectedItem == null)
                    {
                        MessageBox.Show("No logs available for the selected month and year.");
                        return null;
                    }

                    int selYear = (int)yearBox.SelectedItem;
                    int selMonth = (int)monthBox.SelectedItem;
                    if (!available.Any(x => x.Item1 == selYear && x.Item2 == selMonth))
                    {
                        MessageBox.Show("No logs available for the selected month and year.");
                        return null;
                    }
                    return Tuple.Create(selMonth, selYear);
                }
                return null;
            }
        }

        private class LogStats
        {
            public Dictionary<string, int> ActionCounts = new();
            public Dictionary<string, Dictionary<string, int>> UserActionCounts = new();
            public void Add(string user, string action)
            {
                if (!ActionCounts.ContainsKey(action)) ActionCounts[action] = 0;
                ActionCounts[action]++;
                if (!UserActionCounts.ContainsKey(user)) UserActionCounts[user] = new();
                if (!UserActionCounts[user].ContainsKey(action)) UserActionCounts[user][action] = 0;
                UserActionCounts[user][action]++;
            }
        }

        private void btnLogPrinter_Click(object sender, EventArgs e)
        {
            var ym = SelectMonthYear();
            if (ym == null) return;
            int month = ym.Item1, year = ym.Item2;

            var repo = new ResidentsRepository();
            var logs = repo.GetLogsForMonth(year, month);

            if (logs.Rows.Count == 0)
            {
                MessageBox.Show("No logs found for the selected month.");
                return;
            }

            // Calculate statistics
            var stats = new LogStats();
            foreach (DataRow row in logs.Rows)
            {
                string user = row["USER"].ToString();
                string action = row["ACTION"].ToString().ToUpperInvariant();
                stats.Add(user, action);
            }

            using (var saveDlg = new SaveFileDialog { Filter = "Word Document|*.docx", FileName = $"Logs_{year}_{month:00}.docx" })
            {
                if (saveDlg.ShowDialog() != DialogResult.OK) return;

                using (var doc = DocX.Create(saveDlg.FileName))
                {
                    doc.InsertParagraph($"Log Report for {new DateTime(year, month, 1):MMMM yyyy}")
                        .FontSize(18).Bold().Alignment = Alignment.center;
                    doc.InsertParagraph();

                    // Overall statistics
                    doc.InsertParagraph("Action Counts:").Bold();
                    var table = doc.AddTable(stats.ActionCounts.Count + 1, 2);
                    table.Design = TableDesign.LightGrid;
                    table.Rows[0].Cells[0].Paragraphs[0].Append("Action").Bold();
                    table.Rows[0].Cells[1].Paragraphs[0].Append("Count").Bold();
                    int r = 1;
                    foreach (var kv in stats.ActionCounts)
                    {
                        table.Rows[r].Cells[0].Paragraphs[0].Append(kv.Key);
                        table.Rows[r].Cells[1].Paragraphs[0].Append(kv.Value.ToString());
                        r++;
                    }
                    doc.InsertTable(table);
                    doc.InsertParagraph();

                    // Top performers per action
                    doc.InsertParagraph("Top Users per Action:").Bold();
                    foreach (var action in stats.ActionCounts.Keys)
                    {
                        var userList = stats.UserActionCounts
                            .Where(u => u.Value.ContainsKey(action))
                            .OrderByDescending(u => u.Value[action])
                            .ToList();
                        if (userList.Count > 0)
                        {
                            doc.InsertParagraph($"{action}: {userList[0].Key} ({userList[0].Value[action]} times)");
                        }
                    }
                    doc.InsertParagraph();

                    // Log entries table
                    doc.InsertParagraph("Detailed Logs:").Bold();
                    var logTable = doc.AddTable(logs.Rows.Count + 1, logs.Columns.Count);
                    logTable.Design = TableDesign.TableGrid;
                    // Header
                    for (int c = 0; c < logs.Columns.Count; c++)
                        logTable.Rows[0].Cells[c].Paragraphs[0].Append(logs.Columns[c].ColumnName).Bold();
                    // Rows
                    for (int i = 0; i < logs.Rows.Count; i++)
                        for (int j = 0; j < logs.Columns.Count; j++)
                            logTable.Rows[i + 1].Cells[j].Paragraphs[0].Append(logs.Rows[i][j].ToString());
                    doc.InsertTable(logTable);

                    doc.Save();
                    MessageBox.Show("Log report generated successfully!");
                }
            }
        }
    }

    public class AccountItem
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public override string ToString() => AccountName;
    }
}