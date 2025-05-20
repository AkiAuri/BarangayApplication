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
    /// <summary>
    /// The Archive form displays all archived residents with the same look and feel as the Data form.
    /// </summary>
    public partial class Archive : Form
    {
        public Archive()
        {
            InitializeComponent();
            ReadArchivedResidents();
            SetupSearchBarAutocomplete();

            // Set alternating row color for DataGridView
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Attach the CellFormatting event for ALL CAPS
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
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
            var residents = repo.GetArchivedResidents();

            // Add name-based suggestions
            foreach (var r in residents)
            {
                if (!string.IsNullOrWhiteSpace(r.LastName))
                    autoCompleteCollection.Add("N:" + r.LastName);
                if (!string.IsNullOrWhiteSpace(r.FirstName))
                    autoCompleteCollection.Add("N:" + r.FirstName);
                if (!string.IsNullOrWhiteSpace(r.MiddleName))
                    autoCompleteCollection.Add("N:" + r.MiddleName);
            }

            // Add age-based suggestions (all unique ages)
            foreach (var age in residents.Select(r => r.DateOfBirth != DateTime.MinValue ? CalculateAge(r.DateOfBirth) : (int?)null).Where(a => a.HasValue).Select(a => a.Value).Distinct())
            {
                autoCompleteCollection.Add("A:" + age);
            }

            // Add purposes (show all unique purpose names)
            var allPurposeNames = residents
                .SelectMany(r => (r.Purposes != null) ? r.Purposes.Select(p => GetPurposeText(p)) : new List<string>())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct();
            foreach (var purpose in allPurposeNames)
            {
                autoCompleteCollection.Add("P:" + purpose);
            }

            // Remove duplicates
            var unique = autoCompleteCollection.Cast<string>().Distinct().ToArray();

            // Attach to the SearchBar (make sure searchBar exists in the designer)
            if (this.Controls.ContainsKey("SearchBar"))
            {
                var SearchBar = this.Controls["SearchBar"] as TextBox;
                SearchBar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                SearchBar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                SearchBar.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                SearchBar.AutoCompleteCustomSource.AddRange(unique);
                SearchBar.TextChanged += SearchBar_TextChanged;
            }
        }

        /// <summary>
        /// Reads archived resident data from the repository, creates a DataTable,
        /// populates it with select resident details, and displays the data in the DataGrid.
        /// </summary>
        private void ReadArchivedResidents()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("First Name");
            dt.Columns.Add("Middle Name");
            dt.Columns.Add("Age");
            dt.Columns.Add("Contact Number");
            dt.Columns.Add("Purpose");

            var repo = new ResidentsRepository();
            var residents = repo.GetArchivedResidents();

            foreach (var resident in residents)
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

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.HeaderText = col.HeaderText.ToUpper();
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

        private void Archive_Load(object sender, EventArgs e)
        {
            ReadArchivedResidents();
            SetupSearchBarAutocomplete();
        }

        private void SearchBar_TextChanged(object sender, EventArgs e)
        {
            var SearchBar = sender as TextBox;
            string query = SearchBar.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                ReadArchivedResidents();
                return;
            }

            var repo = new ResidentsRepository();
            var residents = repo.GetArchivedResidents();
            IEnumerable<Resident> filtered = residents;

            if (query.StartsWith("N:", StringComparison.OrdinalIgnoreCase))
            {
                string namePart = query.Substring(2).Trim().ToLower();
                filtered = residents.Where(r =>
                    (!string.IsNullOrEmpty(r.LastName) && r.LastName.ToLower().Contains(namePart)) ||
                    (!string.IsNullOrEmpty(r.FirstName) && r.FirstName.ToLower().Contains(namePart)) ||
                    (!string.IsNullOrEmpty(r.MiddleName) && r.MiddleName.ToLower().Contains(namePart))
                );
            }
            else if (query.StartsWith("A:", StringComparison.OrdinalIgnoreCase))
            {
                string agePart = query.Substring(2).Trim();
                if (int.TryParse(agePart, out int age))
                {
                    filtered = residents.Where(r => r.DateOfBirth != DateTime.MinValue && CalculateAge(r.DateOfBirth) == age);
                }
                else
                {
                    filtered = residents.Where(r => r.DateOfBirth != DateTime.MinValue && CalculateAge(r.DateOfBirth).ToString().Contains(agePart));
                }
            }
            else if (query.StartsWith("P:", StringComparison.OrdinalIgnoreCase))
            {
                string purposePart = query.Substring(2).Trim().ToLower();
                filtered = residents.Where(r =>
                    r.Purposes != null && r.Purposes.Any(p => GetPurposeText(p).ToLower().Contains(purposePart))
                );
            }
            else
            {
                string lower = query.ToLower();
                filtered = residents.Where(r =>
                      (!string.IsNullOrEmpty(r.LastName) && r.LastName.ToLower().Contains(lower))
                   || (!string.IsNullOrEmpty(r.FirstName) && r.FirstName.ToLower().Contains(lower))
                   || (!string.IsNullOrEmpty(r.MiddleName) && r.MiddleName.ToLower().Contains(lower))
                   || (r.DateOfBirth != DateTime.MinValue && CalculateAge(r.DateOfBirth).ToString().Contains(lower))
                   || (r.Purposes != null && r.Purposes.Any(p => GetPurposeText(p).ToLower().Contains(lower)))
                );
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("First Name");
            dt.Columns.Add("Middle Name");
            dt.Columns.Add("Age");
            dt.Columns.Add("Contact Number");
            dt.Columns.Add("Purpose");

            foreach (var resident in filtered)
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

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.HeaderText = col.HeaderText.ToUpper();
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
            // Example to match Data form: hide buttons and extend SearchBar if you have them
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

            // Show Reason form
            using (var reasonForm = new Reason())
            {
                if (reasonForm.ShowDialog() == DialogResult.OK)
                {
                    string restoreReason = reasonForm.ArchiveReason;

                    var repo = new ResidentsRepository();
                    repo.RestoreResident(residentId); // You must implement this method!

                    // Log the action
                    repo.AddUserLog(LoginMenu.CurrentUser.AccountID, "Restored",
                        $"Restored resident: {fullName}. Reason: {restoreReason}");

                    ReadArchivedResidents();
                    SetupSearchBarAutocomplete();
                }
            }
        }
    }
}