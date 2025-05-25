using BarangayApplication.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        private readonly ResidentsRepository repo = new ResidentsRepository();

        public Logbook()
        {
            InitializeComponent();

            dgvLog.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            dgvLog.CellFormatting += DgvLog_CellFormatting;

            cBxFilterByAction.SelectedIndexChanged += cBxFilterByAction_SelectedIndexChanged;
            cbxUserFilter.SelectedIndexChanged += cbxUserFilter_SelectedIndexChanged;

            cBxFilterByAction.Items.Clear();
            cBxFilterByAction.Items.Add("ALL ACTIONS");
            cBxFilterByAction.Items.AddRange(new string[] {
                "ADD", "EDIT", "ARCHIVE", "RESTORE", "LOGIN"
            });

            // Hook up the archive button
            if (this.Controls.ContainsKey("archiveBtn"))
                this.Controls["archiveBtn"].Click += archiveBtn_Click;
        }

        private void DgvLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value is string val)
            {
                e.Value = val.ToUpper();
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

            // Use the logbook DB for user list
            using (var conn = new System.Data.SqlClient.SqlConnection("Data Source=localhost,1433;Initial Catalog=ResidentsLogDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"))
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
            int totalLogs = repo.GetTotalLogCount();
            totalPages = (int)Math.Ceiling((double)totalLogs / rowsPerPage);
        }

        private void LoadDataForPage(int page)
        {
            int offset = (page - 1) * rowsPerPage;
            var logs = repo.GetLogsForPage(offset, rowsPerPage);

            dgvLog.DataSource = logs;
            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";

            btnFirst.Enabled = btnPrevious.Enabled = (currentPage > 1);
            btnNext.Enabled = btnLast.Enabled = (currentPage < totalPages);
        }

        private void LoadFilteredLogs(string action, string userName)
        {
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
        
        private void ApplyCombinedFilters()
        {
            string action = GetCurrentFilterAction();
            string user = GetCurrentFilterUser();
            string query = SearchBar.Text.Trim();

            // Start from all logs
            var allLogs = repo.GetAllLogs();

            // Step 1: filter by action/user if needed
            IEnumerable<DataRow> rows = allLogs.AsEnumerable();
            if (action != "ALL ACTIONS")
                rows = rows.Where(row => row["ACTION"].ToString().Equals(action, StringComparison.OrdinalIgnoreCase));
            if (user != "ALL USERS")
                rows = rows.Where(row => row["USER"].ToString().Equals(user, StringComparison.OrdinalIgnoreCase));

            // Step 2: filter by search query (if not empty)
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Advanced Multi-Criteria Parsing (reuse your parsing code)
                var criteria = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string currentPrefix = "";
                var sb = new System.Text.StringBuilder();
                foreach (var part in parts)
                {
                    if (
                        (part.Length > 2 && part[1] == ':' && "UuAaDd".Contains(part[0])) ||
                        (part.Length > 5 && part.StartsWith("Desc:", StringComparison.OrdinalIgnoreCase))
                    )
                    {
                        if (!string.IsNullOrEmpty(currentPrefix) && sb.Length > 0)
                        {
                            if (!criteria.ContainsKey(currentPrefix))
                                criteria[currentPrefix] = new List<string>();
                            criteria[currentPrefix].Add(sb.ToString().Trim());
                            sb.Clear();
                        }
                        if (part.StartsWith("Desc:", StringComparison.OrdinalIgnoreCase))
                        {
                            currentPrefix = "DESC:";
                            sb.Append(part.Substring(5));
                        }
                        else
                        {
                            currentPrefix = part.Substring(0, 2).ToUpper();
                            sb.Append(part.Substring(2));
                        }
                    }
                    else if (!string.IsNullOrEmpty(currentPrefix))
                    {
                        sb.Append(" " + part);
                    }
                }
                if (!string.IsNullOrEmpty(currentPrefix) && sb.Length > 0)
                {
                    if (!criteria.ContainsKey(currentPrefix))
                        criteria[currentPrefix] = new List<string>();
                    criteria[currentPrefix].Add(sb.ToString().Trim());
                }

                rows = rows.Where(row =>
                {
                    bool match = true;
                    if (criteria.ContainsKey("U:"))
                    {
                        foreach (var userQuery in criteria["U:"])
                        {
                            var words = userQuery.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string username = row["USER"].ToString().ToLower();
                            if (!words.All(w => username.Contains(w)))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match && criteria.ContainsKey("A:"))
                    {
                        foreach (var actionQuery in criteria["A:"])
                        {
                            string aq = actionQuery.ToLower();
                            string actionVal = row["ACTION"].ToString().ToLower();
                            if (!actionVal.Contains(aq))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match && criteria.ContainsKey("D:"))
                    {
                        foreach (var dateQuery in criteria["D:"])
                        {
                            string dq = dateQuery.ToLower();
                            string date = row["DATE & TIME"].ToString().ToLower();
                            if (!date.Contains(dq))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match && criteria.ContainsKey("DESC:"))
                    {
                        foreach (var descQuery in criteria["DESC:"])
                        {
                            var words = descQuery.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string desc = row["DESCRIPTION"].ToString().ToLower();
                            if (!words.All(w => desc.Contains(w)))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (criteria.Count == 0)
                    {
                        var words = query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        string username = row["USER"].ToString().ToLower();
                        string actionVal = row["ACTION"].ToString().ToLower();
                        string date = row["DATE & TIME"].ToString().ToLower();
                        string desc = row["DESCRIPTION"].ToString().ToLower();

                        if (words.All(w =>
                            username.Contains(w) ||
                            actionVal.Contains(w) ||
                            date.Contains(w) ||
                            desc.Contains(w)
                        ))
                        {
                            match = true;
                        }
                        else
                        {
                            match = false;
                        }
                    }
                    return match;
                });
            }

            // Compose filtered DataTable
            DataTable dt = allLogs.Clone();
            foreach (var row in rows)
                dt.ImportRow(row);

            dgvLog.DataSource = dt;
            lblPageInfo.Text = $"Filtered ({dt.Rows.Count} records)";
            btnFirst.Enabled = btnPrevious.Enabled = btnNext.Enabled = btnLast.Enabled = false;
        }
        
        private void SearchBar_TextChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilters();
        }
        
        private void cBxFilterByAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilters();
        }
        private void cbxUserFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCombinedFilters();
        }
        
        private void llClearFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cBxFilterByAction.SelectedIndex = 0;
            cbxUserFilter.SelectedIndex = 0;
            SearchBar.Text = ""; // Also clear the search bar
            LoadDataForPage(currentPage); // Restore unfiltered
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
        
        // ================== ARCHIVE LOGS OLDER THAN A MONTH =========================

        private void archiveBtn_Click(object sender, EventArgs e)
        {
            // Get all logs older than a month (based on current time)
            var allLogs = repo.GetAllLogs();
            var cutoffDate = DateTime.Now.AddMonths(-1);

            var oldRows = allLogs.AsEnumerable()
                .Where(row =>
                {
                    string dateRaw = row["DATE & TIME"].ToString();
                    DateTime logDate;
                    // TryParseExact for 'yyyy-MM-dd hh:mm:ss tt'
                    if (DateTime.TryParse(dateRaw, out logDate))
                    {
                        return logDate < cutoffDate;
                    }
                    return false;
                }).ToList();

            if (oldRows.Count == 0)
            {
                MessageBox.Show("No logs older than a month to archive.");
                return;
            }

            // Save to text file
            using (var saveDlg = new SaveFileDialog
            {
                Title = "Export old logs to text file",
                Filter = "Text Files (*.txt)|*.txt",
                FileName = $"ArchivedLogs_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                OverwritePrompt = true
            })
            {
                saveDlg.AddExtension = true;
                saveDlg.CheckPathExists = true;
                saveDlg.ValidateNames = true;
                saveDlg.RestoreDirectory = true;
                saveDlg.SupportMultiDottedExtensions = false;
                saveDlg.AutoUpgradeEnabled = true;
                saveDlg.ShowHelp = false;
                saveDlg.CreatePrompt = false;
                saveDlg.DefaultExt = "txt";
                saveDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Only OK and Cancel buttons will be shown by default

                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;

                using (var sw = new StreamWriter(saveDlg.FileName, false))
                {
                    // Write header
                    var cols = allLogs.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
                    sw.WriteLine(string.Join("\t", cols));
                    foreach (var row in oldRows)
                    {
                        var vals = cols.Select(col => row[col]?.ToString() ?? "");
                        sw.WriteLine(string.Join("\t", vals));
                    }
                }
            }

            // Remove archived logs from DB
            repo.DeleteLogsBefore(cutoffDate);
            MessageBox.Show($"{oldRows.Count} logs archived and removed from the database.");
            // Refresh log display
            CalculateTotalPages();
            LoadDataForPage(1);
        }

        // ================== DOCX PRINTER =========================

        private Tuple<int, int> SelectMonthYear()
        {
            var available = repo.GetAvailableLogMonths(); // List<Tuple<int, int>>

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
            // Use current filters for the report (not search bar!)
            string action = GetCurrentFilterAction();
            string user = GetCurrentFilterUser();

            DataTable logs;
            if (action != "ALL ACTIONS" && user != "ALL USERS")
                logs = repo.GetLogsByUserAndAction(user, action);
            else if (action != "ALL ACTIONS")
                logs = repo.GetFilteredLogs(action);
            else if (user != "ALL USERS")
                logs = repo.GetLogsByUser(user);
            else
                logs = repo.GetAllLogs();

            if (logs.Rows.Count == 0)
            {
                MessageBox.Show("No logs found for the selected filters.");
                return;
            }

            // Only OK/Cancel for SaveFileDialog (this is default)
            using (var saveDlg = new SaveFileDialog { Filter = "Word Document|*.docx", FileName = $"LogReport_{DateTime.Now:yyyyMMdd}.docx" })
            {
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;

                var stats = new LogStats();
                foreach (DataRow row in logs.Rows)
                {
                    string logUser = row["USER"].ToString();
                    string logAction = row["ACTION"].ToString().ToUpperInvariant();
                    stats.Add(logUser, logAction);
                }

                using (var doc = DocX.Create(saveDlg.FileName))
                {
                    doc.InsertParagraph($"Log Report ({(action == "ALL ACTIONS" ? "All Actions" : action)}, {(user == "ALL USERS" ? "All Users" : user)})")
                        .FontSize(18).Bold().Alignment = Alignment.center;
                    doc.InsertParagraph();

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

                    doc.InsertParagraph("Top Users per Action:").Bold();
                    foreach (var act in stats.ActionCounts.Keys)
                    {
                        var userList = stats.UserActionCounts
                            .Where(u => u.Value.ContainsKey(act))
                            .OrderByDescending(u => u.Value[act])
                            .ToList();
                        if (userList.Count > 0)
                        {
                            doc.InsertParagraph($"{act}: {userList[0].Key} ({userList[0].Value[act]} times)");
                        }
                    }
                    doc.InsertParagraph();

                    doc.InsertParagraph("Detailed Logs:").Bold();
                    var logTable = doc.AddTable(logs.Rows.Count + 1, logs.Columns.Count);
                    logTable.Design = TableDesign.TableGrid;
                    for (int c = 0; c < logs.Columns.Count; c++)
                        logTable.Rows[0].Cells[c].Paragraphs[0].Append(logs.Columns[c].ColumnName).Bold();
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