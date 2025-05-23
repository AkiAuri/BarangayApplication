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
using static BarangayApplication.LoginMenu;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;

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

            this.DataGrid.DataSource = dt;
            if (this.DataGrid.Columns["ID"] != null)
                this.DataGrid.Columns["ID"].Visible = false;

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in DataGrid.Columns)
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
                var repo = new ResidentsRepository();

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
                MessageBox.Show("Please select a resident to archive.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var val = this.DataGrid.SelectedRows[0].Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(val))
                return;

            // Get the name from the DataGrid using your column names
            var lastName = this.DataGrid.SelectedRows[0].Cells["Last Name"].Value?.ToString();
            var firstName = this.DataGrid.SelectedRows[0].Cells["First Name"].Value?.ToString();
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
                    string archiveReason = reasonForm.ArchiveReason;

                    var repo = new ResidentsRepository();
                    repo.ArchiveResident(residentId);

                    // Log the action using the name and reason
                    repo.AddUserLog(CurrentUser.AccountID, "Archived",
                        $"Archived resident: {fullName}. Reason: {archiveReason}");

                    ReadResidents();
                    SetupSearchBarAutocomplete();
                }
            }
        }

        private void Viewbtn_Click(object sender, EventArgs e)
        {
            if (this.DataGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a resident to view/print.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRow = this.DataGrid.SelectedRows[0];
            var idVal = selectedRow.Cells["ID"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(idVal))
            {
                MessageBox.Show("Invalid resident ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int residentId;
            if (!int.TryParse(idVal, out residentId))
            {
                MessageBox.Show("Invalid resident ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var repo = new ResidentsRepository();
            var resident = repo.GetApplicant(residentId);
            if (resident == null)
            {
                MessageBox.Show("Resident not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get first purpose (or ask user if multiple)
            string purposeType = "";
            if (resident.Purposes != null && resident.Purposes.Count > 0)
                purposeType = GetPurposeText(resident.Purposes[0]); // or ask user if ambiguity matters

            string address = resident.Address ?? "(Address not specified)";
            string clearanceNo = resident.ResidentID.ToString();
            string printDate = DateTime.Now.ToString("MMMM dd, yyyy");
            string lastName = resident.LastName ?? "";
            string firstName = resident.FirstName ?? "";
            string middleName = resident.MiddleName ?? "";

            // Prompt user where to save
            using (var saveDlg = new SaveFileDialog())
            {
                saveDlg.Filter = "Word Document (*.docx)|*.docx";
                saveDlg.FileName = $"{lastName}_{firstName}_Clearance.docx";
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;

                string filename = saveDlg.FileName;

                // Create docx
                var doc = DocX.Create(filename);

                // Header: Clearance No. and Date
                var para1 = doc.InsertParagraph();
                para1.Append("Clearance No.: ").FontSize(12);
                para1.Append(clearanceNo).Bold().FontSize(12);
                para1.SpacingAfter(5);

                doc.InsertParagraph(printDate)
                    .FontSize(12)
                    .SpacingAfter(20);

                // Centered clearance type header
                string clearanceHeader = $"{purposeType} Clearance";
                doc.InsertParagraph(clearanceHeader)
                    .FontSize(14)
                    .Bold()
                    .Alignment = Alignment.center;

                doc.InsertParagraph(new string('=', 30) + clearanceHeader + new string('=', 30))
                    .FontSize(10)
                    .SpacingAfter(20)
                    .Alignment = Alignment.center;

                // Body
                var p = doc.InsertParagraph();
                p.Append("To Whom this may Concern,\n\n").FontSize(12);

                p.Append("This certifies that the bearer ").FontSize(12)
                 .Append(lastName).Bold().FontSize(12)
                 .Append(", ").FontSize(12)
                 .Append(firstName).Bold().FontSize(12)
                 .Append(" ").FontSize(12)
                 .Append(middleName).Bold().FontSize(12)
                 .Append(" of ").FontSize(12)
                 .Append(address).Bold().FontSize(12)
                 .Append(" has been issued a ").FontSize(12)
                 .Append(purposeType).Bold().FontSize(12)
                 .Append(" Clearance.").FontSize(12);

                p.SpacingAfter(25);

                // Footer/Signature
                doc.InsertParagraph("______________________________")
                    .FontSize(12)
                    .SpacingBefore(40)
                    .Alignment = Alignment.right;
                doc.InsertParagraph("Authorized Officer")
                    .FontSize(12)
                    .Alignment = Alignment.right;

                doc.Save();

                MessageBox.Show("Clearance document generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optionally, open the file automatically
                if (MessageBox.Show("Open the generated document?", "Open Document", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filename);
                }
            }
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

            this.DataGrid.DataSource = dt;
            if (this.DataGrid.Columns["ID"] != null)
                this.DataGrid.Columns["ID"].Visible = false;

            // Make header ALL CAPS
            foreach (DataGridViewColumn col in DataGrid.Columns)
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
            // Hide buttons
            Addbtn.Visible = false;
            btnEdit.Visible = false;
            Archivebtn.Visible = false;
            Viewbtn.Visible = false;

            // Extend SearchBar to occupy the space left by the buttons
            SearchBar.Width = this.Width - SearchBar.Left - 20; // Adjust width dynamically
        }

        private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}