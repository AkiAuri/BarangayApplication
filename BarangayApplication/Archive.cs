using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BarangayApplication.Models;
using BarangayApplication.Models.Repositories;

namespace BarangayApplication
{
    public partial class Archive : Form
    {
        // --- PAGINATION FIELDS ---
        private int currentPage = 1;
        private int totalPages = 0;
        private const int rowsPerPage = 28; // You can adjust this if you want
        private List<Resident> archivedResidentsCache = null; // For paging/filtering

        public Archive()
        {
            InitializeComponent();
            ReadArchivedResidents();
            SetupSearchBarAutocomplete();

            // Set alternating row color for DataGridView
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;

            // --- PAGING BUTTON HOOKUP ---
            if (this.Controls.ContainsKey("btnFirst"))
                this.Controls["btnFirst"].Click += btnFirst_Click;
            if (this.Controls.ContainsKey("btnPrevious"))
                this.Controls["btnPrevious"].Click += btnPrevious_Click;
            if (this.Controls.ContainsKey("btnNext"))
                this.Controls["btnNext"].Click += btnNext_Click;
            if (this.Controls.ContainsKey("btnLast"))
                this.Controls["btnLast"].Click += btnLast_Click;
        }

        // ALL CAPS for all displayed cell data
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value is string)
            {
                e.Value = e.Value.ToString().ToUpper();
                e.FormattingApplied = true;
            }
        }

        private void SetupSearchBarAutocomplete()
        {
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();
            var repo = new ResidentsRepository();
            var residents = repo.GetArchivedApplicants();

            foreach (var r in residents)
            {
                if (!string.IsNullOrWhiteSpace(r.LastName))
                    autoCompleteCollection.Add("N:" + r.LastName);
                if (!string.IsNullOrWhiteSpace(r.FirstName))
                    autoCompleteCollection.Add("N:" + r.FirstName);
                if (!string.IsNullOrWhiteSpace(r.MiddleName))
                    autoCompleteCollection.Add("N:" + r.MiddleName);
            }
            foreach (var age in residents.Select(r => r.DateOfBirth != DateTime.MinValue ? CalculateAge(r.DateOfBirth) : (int?)null).Where(a => a.HasValue).Select(a => a.Value).Distinct())
            {
                autoCompleteCollection.Add("A:" + age);
            }
            var allPurposeNames = residents
                .SelectMany(r => (r.Purposes != null) ? r.Purposes.Select(p => GetPurposeText(p)) : new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct();
            foreach (var purpose in allPurposeNames)
            {
                autoCompleteCollection.Add("P:" + purpose);
            }
            // ------- ADDRESS AUTOCOMPLETE -------
            var allAddresses = residents
                .Where(r => !string.IsNullOrWhiteSpace(r.Address))
                .Select(r => r.Address.Trim())
                .Distinct();
            foreach (var address in allAddresses)
            {
                autoCompleteCollection.Add("D:" + address);
            }
            // ------- END ADDRESS AUTOCOMPLETE -------

            var unique = autoCompleteCollection.Cast<string>().Distinct().ToArray();

            if (this.Controls.ContainsKey("SearchBar"))
            {
                var SearchBar = this.Controls["SearchBar"] as TextBox;
                SearchBar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchBar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                SearchBar.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                SearchBar.AutoCompleteCustomSource.AddRange(unique);
                SearchBar.TextChanged -= SearchBar_TextChanged; // Prevent double-event
                SearchBar.TextChanged += SearchBar_TextChanged;
            }
        }

        private List<Resident> allArchivedResidents = null; // MASTER copy, never filtered

        /// <summary>
        /// Reads archived resident data from the repository, creates a DataTable,
        /// populates it with select resident details, and displays the data in the DataGrid.
        /// </summary>
        private void ReadArchivedResidents()
        {
            var repo = new ResidentsRepository();
            allArchivedResidents = repo.GetArchivedApplicants();
            archivedResidentsCache = new List<Resident>(allArchivedResidents); // Start unfiltered
            CalculateTotalPages();
            LoadDataForPage(1);
        }

        // --- PAGINATION SUPPORT ---

        private void CalculateTotalPages()
        {
            int totalRows = archivedResidentsCache?.Count ?? 0;
            totalPages = (int)Math.Ceiling((double)totalRows / rowsPerPage);
            if (totalPages == 0) totalPages = 1;
        }

        private void LoadDataForPage(int page)
        {
            if (archivedResidentsCache == null) ReadArchivedResidents();
            int offset = (page - 1) * rowsPerPage;
            var pageResidents = archivedResidentsCache.Skip(offset).Take(rowsPerPage).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("First Name");
            dt.Columns.Add("Middle Name");
            dt.Columns.Add("Age");
            dt.Columns.Add("Contact Number");
            dt.Columns.Add("Purpose");

            foreach (var resident in pageResidents)
            {
                var row = dt.NewRow();
                row["ID"] = resident.ResidentID;
                row["Last Name"] = resident.LastName;
                row["First Name"] = resident.FirstName;
                row["Middle Name"] = resident.MiddleName;
                row["Age"] = resident.DateOfBirth != DateTime.MinValue ? CalculateAge(resident.DateOfBirth) : 0;
                row["Contact Number"] = resident.TelCelNo;
                row["Purpose"] = resident.Purposes != null && resident.Purposes.Count > 0
                    ? string.Join(", ", resident.Purposes.Select(GetPurposeText))
                    : "";
                dt.Rows.Add(row);
            }

            dataGridView1.DataSource = dt;
            if (dataGridView1.Columns["ID"] != null)
                dataGridView1.Columns["ID"].Visible = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.HeaderText = col.HeaderText.ToUpper();

            UpdatePagingButtons();
            UpdatePageLabel();
        }

        private void UpdatePagingButtons()
        {
            if (this.Controls.ContainsKey("btnFirst") && this.Controls.ContainsKey("btnPrevious") &&
                this.Controls.ContainsKey("btnNext") && this.Controls.ContainsKey("btnLast"))
            {
                this.Controls["btnFirst"].Enabled = this.Controls["btnPrevious"].Enabled = (currentPage > 1);
                this.Controls["btnNext"].Enabled = this.Controls["btnLast"].Enabled = (currentPage < totalPages);
            }
        }

        private void UpdatePageLabel()
        {
            if (this.Controls.ContainsKey("lblPageInfo"))
            {
                var lbl = this.Controls["lblPageInfo"] as Label;
                lbl.Text = $"Page {currentPage} of {totalPages}";
            }
        }

        // --- PAGINATION BUTTON EVENTS ---

        private void btnFirst_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            LoadDataForPage(currentPage);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadDataForPage(currentPage);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadDataForPage(currentPage);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            currentPage = totalPages;
            LoadDataForPage(currentPage);
        }

        // --- FILTERING SUPPORT ---

        private void Archive_Load(object sender, EventArgs e)
        {
            ReadArchivedResidents();
            SetupSearchBarAutocomplete();
        }

        private void SearchBar_TextChanged(object sender, EventArgs e)
        {
            var SearchBar = sender as TextBox;
            string query = SearchBar.Text.Trim();

            // Always filter from MASTER list
            List<Resident> baseResidents = allArchivedResidents ?? new List<Resident>();

            IEnumerable<Resident> filtered = baseResidents;

            if (string.IsNullOrWhiteSpace(query))
            {
                // Reset to show all residents
                archivedResidentsCache = new List<Resident>(allArchivedResidents);
                CalculateTotalPages();
                currentPage = 1;
                LoadDataForPage(currentPage);
                return;
            }

            // --- Advanced Multi-Criteria Parsing ---
            // Supports: N:John Doe P:Loan S:Jane Doe A:30 D:Main Street (any order, any combination)
            // Split by space, parse known prefixes
            var criteria = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string currentPrefix = "";
            var sb = new System.Text.StringBuilder();
            foreach (var part in parts)
            {
                if (part.Length > 2 && part[1] == ':' && "NnAaPpSsDd".Contains(part[0]))
                {
                    // Save previous
                    if (!string.IsNullOrEmpty(currentPrefix) && sb.Length > 0)
                    {
                        if (!criteria.ContainsKey(currentPrefix))
                            criteria[currentPrefix] = new List<string>();
                        criteria[currentPrefix].Add(sb.ToString().Trim());
                        sb.Clear();
                    }
                    currentPrefix = part.Substring(0, 2).ToUpper(); // e.g. N:
                    sb.Append(part.Substring(2));
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

            // Now filter residents
            filtered = baseResidents.Where(r =>
            {
                bool match = true;
                // Name (N:)
                if (criteria.ContainsKey("N:"))
                {
                    foreach (var nameQuery in criteria["N:"])
                    {
                        var words = nameQuery.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string fullName = $"{r.FirstName} {r.MiddleName} {r.LastName}".ToLower();
                        if (!words.All(w => fullName.Contains(w)))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                // Age (A:)
                if (match && criteria.ContainsKey("A:"))
                {
                    foreach (var ageQuery in criteria["A:"])
                    {
                        if (int.TryParse(ageQuery, out int age))
                        {
                            if (!(r.DateOfBirth != DateTime.MinValue && CalculateAge(r.DateOfBirth) == age))
                            {
                                match = false;
                                break;
                            }
                        }
                        else
                        {
                            if (!(r.DateOfBirth != DateTime.MinValue && CalculateAge(r.DateOfBirth).ToString().Contains(ageQuery)))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                }
                // Purpose (P:)
                if (match && criteria.ContainsKey("P:"))
                {
                    foreach (var purposeQuery in criteria["P:"])
                    {
                        string pq = purposeQuery.ToLower();
                        if (!(r.Purposes != null && r.Purposes.Any(p => GetPurposeText(p).ToLower().Contains(pq))))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                // Spouse (S:)
                if (match && criteria.ContainsKey("S:"))
                {
                    foreach (var spouseQuery in criteria["S:"])
                    {
                        var words = spouseQuery.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string spouseName = r.Spouse != null && !string.IsNullOrWhiteSpace(r.Spouse.SpouseName)
                            ? r.Spouse.SpouseName.ToLower()
                            : "";
                        if (string.IsNullOrEmpty(spouseName) || !words.All(w => spouseName.Contains(w)))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                // -------- Address (D:) --------
                if (match && criteria.ContainsKey("D:"))
                {
                    foreach (var addressQuery in criteria["D:"])
                    {
                        string addr = r.Address != null ? r.Address.ToLower() : "";
                        var words = addressQuery.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (string.IsNullOrWhiteSpace(addr) || !words.All(w => addr.Contains(w)))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                // If no criteria matched, fallback to general search (for untagged queries)
                if (criteria.Count == 0)
                {
                    var words = query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string fullName = $"{r.FirstName} {r.MiddleName} {r.LastName}".ToLower();
                    string spouseName = r.Spouse != null && !string.IsNullOrWhiteSpace(r.Spouse.SpouseName)
                        ? r.Spouse.SpouseName.ToLower()
                        : "";
                    string purposes = r.Purposes != null
                        ? string.Join(" ", r.Purposes.Select(GetPurposeText)).ToLower()
                        : "";
                    string age = r.DateOfBirth != DateTime.MinValue ? CalculateAge(r.DateOfBirth).ToString() : "";
                    string addr = r.Address != null ? r.Address.ToLower() : "";

                    // All words must match ANY field (not ALL in one field)
                    if (words.All(w =>
                        fullName.Contains(w) ||
                        spouseName.Contains(w) ||
                        purposes.Contains(w) ||
                        age.Contains(w) ||
                        addr.Contains(w)
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

            // For paging: update cache, recalculate pages, show page 1
            archivedResidentsCache = filtered.ToList();
            CalculateTotalPages();
            currentPage = 1;
            LoadDataForPage(currentPage);
        }

        private string GetPurposeText(ResidentPurpose purpose)
        {
            if (purpose == null) return "";
            if (!string.IsNullOrWhiteSpace(purpose.PurposeOthers))
                return purpose.PurposeOthers;
            if (purpose.PurposeType != null && !string.IsNullOrWhiteSpace(purpose.PurposeType.PurposeName))
                return purpose.PurposeType.PurposeName;
            return "";
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now < dateOfBirth.AddYears(age)) age--;
            return age;
        }

        public void AdjustLayoutForFormApplication()
        {
            if (this.Controls.ContainsKey("Addbtn"))
                this.Controls["Addbtn"].Visible = false;
            if (this.Controls.ContainsKey("btnEdit"))
                this.Controls["btnEdit"].Visible = false;
            if (this.Controls.ContainsKey("Archivebtn"))
                this.Controls["Archivebtn"].Visible = false;
            if (this.Controls.ContainsKey("Viewbtn"))
                this.Controls["Viewbtn"].Visible = false;
            if (this.Controls.ContainsKey("SearchBar"))
            {
                var SearchBar = this.Controls["SearchBar"];
                SearchBar.Width = this.Width - SearchBar.Left - 20;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a resident to restore.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var val = this.dataGridView1.SelectedRows[0].Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(val))
                return;

            var lastName = this.dataGridView1.SelectedRows[0].Cells["Last Name"].Value?.ToString();
            var firstName = this.dataGridView1.SelectedRows[0].Cells["First Name"].Value?.ToString();
            var fullName = $"{firstName} {lastName}".Trim();

            int residentId;
            if (!int.TryParse(val, out residentId))
            {
                MessageBox.Show("Invalid resident ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var reasonForm = new Reason())
            {
                if (reasonForm.ShowDialog() == DialogResult.OK)
                {
                    string restoreReason = reasonForm.ArchiveReason;
                    var repo = new ResidentsRepository();
                    repo.RestoreResident(residentId);
                    repo.AddUserLog(Convert.ToInt32(LoginMenu.CurrentUser.AccountID), "restore",
                        $"Restored resident: {fullName}. Reason: {restoreReason}");
                    ReadArchivedResidents();
                    SetupSearchBarAutocomplete();
                }
            }
        }
    }
}