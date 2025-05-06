using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarangayApplication.Models;
using BarangayApplication.Models.Repositories;

namespace BarangayApplication
{
    /// <summary>
    /// The Data form is responsible for displaying the list of residents
    /// and providing functionality to add and edit resident records.
    /// </summary>
    public partial class Data : Form
    {
        public Data()
        {
            InitializeComponent();
            ReadResidents();
            SetupSearchBarAutocomplete(); // Make sure suggestions are ready at start

            // Set alternating row color for DataGridView
            DataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Attach the CellFormatting event for ALL CAPS
            DataGrid.CellFormatting += DataGrid_CellFormatting;
        }

        // ALL CAPS for all displayed cell data
        private void DataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value is string)
            {
                e.Value = e.Value.ToString().ToUpper();
                e.FormattingApplied = true;
            }
        }

        // Call this in your Data() constructor or Data_Load
        private void SetupSearchBarAutocomplete()
        {
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

            var repo = new ResidentsRepository();
            var residents = repo.GetApplicants();

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
            foreach (var age in residents.Select(r => r.Age).Distinct())
            {
                autoCompleteCollection.Add("A:" + age);
            }

            // Add purposes
            foreach (var r in residents)
            {
                string purpose = GetPurposeText(r);
                if (!string.IsNullOrWhiteSpace(purpose))
                    autoCompleteCollection.Add("P:" + purpose);
            }

            // Remove duplicates
            var unique = autoCompleteCollection.Cast<string>().Distinct().ToArray();

            // Attach to the SearchBar
            SearchBar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            SearchBar.AutoCompleteSource = AutoCompleteSource.CustomSource;
            SearchBar.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            SearchBar.AutoCompleteCustomSource.AddRange(unique);
        }

        /// <summary>
        /// Reads resident data from the repository, creates a DataTable,
        /// populates it with select resident details, and displays the data in the DataGrid.
        /// </summary>
        private void ReadResidents()
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
            var residents = repo.GetApplicants();

            foreach (var resident in residents)
            {
                var row = dt.NewRow();
                row["ID"] = resident.Id;
                row["Last Name"] = resident.LastName;
                row["First Name"] = resident.FirstName;
                row["Middle Name"] = resident.MiddleName;
                row["Age"] = resident.Age;
                row["Contact Number"] = resident.TelCelNo;
                row["Purpose"] = GetPurposeText(resident);
                dt.Rows.Add(row);
            }

            this.DataGrid.DataSource = dt;
            if (this.DataGrid.Columns["ID"] != null)
                this.DataGrid.Columns["ID"].Visible = false;

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in DataGrid.Columns)
                col.HeaderText = col.HeaderText.ToUpper();
        }

        private string GetPurposeText(Residents resident)
        {
            if (!string.IsNullOrWhiteSpace(resident.PurposeOthers))
                return resident.PurposeOthers;
            if (resident.PurposeResidency) return "Residency";
            if (resident.PurposePostalID) return "Postal ID";
            if (resident.PurposeLocalEmployment) return "Local Employment";
            if (resident.PurposeMarriage) return "Marriage";
            if (resident.PurposeLoan) return "Loan";
            if (resident.PurposeMeralco) return "Meralco";
            if (resident.PurposeBankTransaction) return "Bank Transaction";
            if (resident.PurposeTravelAbroad) return "Travel Abroad";
            if (resident.PurposeSeniorCitizen) return "Senior Citizen";
            if (resident.PurposeSchool) return "School";
            if (resident.PurposeMedical) return "Medical";
            if (resident.PurposeBurial) return "Burial";
            return "";
        }

        private void Data_Load(object sender, EventArgs e)
        {
            // If you use Data_Load instead of constructor for setup:
            // ReadResidents();
            // SetupSearchBarAutocomplete();
        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            MainViewForm Form = new MainViewForm();
            if (Form.ShowDialog() == DialogResult.OK)
            {
                ReadResidents();
                SetupSearchBarAutocomplete(); // Refresh suggestions after add
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.DataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a resident to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var val = this.DataGrid.SelectedRows[0].Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(val))
                return;

            int applicantId;
            if (!int.TryParse(val, out applicantId))
            {
                MessageBox.Show("Invalid resident ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var repo = new ResidentsRepository();
            var applicant = repo.GetApplicant(applicantId);
            if (applicant == null)
            {
                MessageBox.Show("Resident not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainViewForm editForm = new MainViewForm(applicant);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                ReadResidents();
                SetupSearchBarAutocomplete(); // Refresh suggestions after edit
            }
        }

        private void Archivebtn_Click(object sender, EventArgs e)
        {
            if (this.DataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a resident to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var val = this.DataGrid.SelectedRows[0].Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(val))
                return;

            int applicantId;
            if (!int.TryParse(val, out applicantId))
            {
                MessageBox.Show("Invalid resident ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var repo = new ResidentsRepository();
            repo.DeleteResident(applicantId);

            ReadResidents();
            SetupSearchBarAutocomplete(); // Refresh suggestions after delete
        }

        private void Viewbtn_Click(object sender, EventArgs e)
        {
            // For viewing/printing details of a resident
        }

        private void SearchBar_TextChanged(object sender, EventArgs e)
        {
            string query = SearchBar.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                ReadResidents();
                return;
            }

            var repo = new ResidentsRepository();
            var residents = repo.GetApplicants();
            IEnumerable<Residents> filtered = residents;

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
                    filtered = residents.Where(r => r.Age == age);
                }
                else
                {
                    filtered = residents.Where(r => r.Age.ToString().Contains(agePart));
                }
            }
            else if (query.StartsWith("P:", StringComparison.OrdinalIgnoreCase))
            {
                string purposePart = query.Substring(2).Trim().ToLower();
                filtered = residents.Where(r => GetPurposeText(r).ToLower().Contains(purposePart));
            }
            else
            {
                string lower = query.ToLower();
                filtered = residents.Where(r =>
                      (!string.IsNullOrEmpty(r.LastName) && r.LastName.ToLower().Contains(lower))
                   || (!string.IsNullOrEmpty(r.FirstName) && r.FirstName.ToLower().Contains(lower))
                   || (!string.IsNullOrEmpty(r.MiddleName) && r.MiddleName.ToLower().Contains(lower))
                   || r.Age.ToString().Contains(lower)
                   || GetPurposeText(r).ToLower().Contains(lower)
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
                row["ID"] = resident.Id;
                row["Last Name"] = resident.LastName;
                row["First Name"] = resident.FirstName;
                row["Middle Name"] = resident.MiddleName;
                row["Age"] = resident.Age;
                row["Contact Number"] = resident.TelCelNo;
                row["Purpose"] = GetPurposeText(resident);
                dt.Rows.Add(row);
            }

            this.DataGrid.DataSource = dt;
            if (this.DataGrid.Columns["ID"] != null)
                this.DataGrid.Columns["ID"].Visible = false;

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in DataGrid.Columns)
                col.HeaderText = col.HeaderText.ToUpper();
        }

        public void AdjustLayoutForFormApplication()
        {
            // Hide buttons
            Addbtn.Visible = false;
            btnEdit.Visible = false;
            Archivebtn.Visible = false;
            Viewbtn.Visible = false;

            // Extend SearchBar to occupy the space left by the buttons
            SearchBar.Width = this.Width - SearchBar.Left - 20; // Adjust width dynamically
        }
    }
}