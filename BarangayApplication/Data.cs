using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
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
using PdfSharp.Pdf;
using PdfSharp.Drawing;

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
            PdfSharp.Fonts.GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            
            InitializeComponent();
            ReadResidents();
            SetupSearchBarAutocomplete(); // Make sure suggestions are ready at start

            // Set alternating row color for DataGridView
            DataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Attach the CellFormatting event for ALL CAPS
            DataGrid.CellFormatting += DataGrid_CellFormatting;
        }
        
        private readonly string _mainConn = "Data Source=localhost,1433;Initial Catalog=ResidentsDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

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
                    repo.AddUserLog(Convert.ToInt32(CurrentUser.AccountID), "archive",
                        $"Archived resident: {fullName}. Reason: {archiveReason}");

                    ReadResidents();
                    SetupSearchBarAutocomplete();
                }
            }
        }
        
        // Helper to load PNG images from the Resources folder (in root, not Properties)
        private XImage LoadResourceImage(string fileName)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(exeDir, "Resources", fileName);
            return XImage.FromFile(imagePath);
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
            if (string.IsNullOrWhiteSpace(idVal) || !int.TryParse(idVal, out int residentId))
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

            // Get TransactionID and Purpose from ResidentPurposes
            string transactionId = "";
            string purposeName = "";
            using (var conn = new SqlConnection(_mainConn))
            {
                conn.Open();
                string sql = @"
                    SELECT TOP 1 rp.TransactionID, pt.PurposeName, rp.PurposeOthers
                    FROM ResidentPurposes rp
                    INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                    WHERE rp.ResidentID = @ResidentID
                    ORDER BY rp.TransactionID DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            transactionId = reader.GetString(0);
                            purposeName = reader.GetString(1);
                            if (!reader.IsDBNull(2) && !string.IsNullOrWhiteSpace(reader.GetString(2)))
                                purposeName = reader.GetString(2); // Use others if provided
                        }
                    }
                }
            }

            // Get Captain's name
            string captainName = repo.GetBarangayCaptainName();

            // Official date string
            var now = DateTime.Now;
            string daySuffix = now.Day % 10 == 1 && now.Day != 11 ? "st" :
                               now.Day % 10 == 2 && now.Day != 12 ? "nd" :
                               now.Day % 10 == 3 && now.Day != 13 ? "rd" : "th";
            string officialDate = $"Given this {now:dddd}, the {now.Day}{daySuffix} day of {now:MMMM}, {now:yyyy}.";

            // Prompt where to save
            using (var saveDlg = new SaveFileDialog())
            {
                saveDlg.Filter = "PDF File (*.pdf)|*.pdf";
                saveDlg.FileName = $"{resident.LastName}_{resident.FirstName}_Clearance.pdf";
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;

                string filename = saveDlg.FileName;
                using (var doc = new PdfDocument())
                {
                    var page = doc.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);

                    var fontHeader = new XFont("Arial", 12, XFontStyleEx.Bold);
                    var fontBody = new XFont("Arial", 11, XFontStyleEx.Regular);
                    var fontSmall = new XFont("Arial", 9, XFontStyleEx.Regular);

                    // Header and footer images
                    var headerImg = LoadResourceImage("Barangay_Header.png");
                    double headerWidth = page.Width;
                    double headerHeight = headerImg.PixelHeight * headerWidth / headerImg.PixelWidth;
                    gfx.DrawImage(headerImg, 0, 0, headerWidth, headerHeight);

                    var footerImg = LoadResourceImage("Barangay_Footer.png");
                    double footerWidth = page.Width;
                    double footerHeight = footerImg.PixelHeight * footerWidth / footerImg.PixelWidth;
                    gfx.DrawImage(footerImg, 0, page.Height - footerHeight, footerWidth, footerHeight);

                    // Margins and content area
                    double leftMargin = 60;
                    double rightMargin = 60;
                    double y = headerHeight + 28;
                    double lineSpacing = 23;

                    // --- Series No. (left) and Date block (right) ---
                    gfx.DrawString($"Series No.: {transactionId}", fontBody, XBrushes.Black, leftMargin, y);

                    // Date block (formatted, with line and label), right-aligned
                    string dateStr = now.ToString("MMMM dd, yyyy");
                    double dateBlockX = page.Width - rightMargin;
                    double dateBlockY = y;
                    XStringFormat topRight = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Near };

                    // Date string
                    gfx.DrawString(dateStr, fontBody, XBrushes.Black, dateBlockX, dateBlockY, topRight);
                    // Line under date
                    gfx.DrawLine(XPens.Black, dateBlockX - 75, dateBlockY + 16, dateBlockX + 10, dateBlockY + 16);
                    // "DATE" label
                    gfx.DrawString("DATE", fontSmall, XBrushes.Black, dateBlockX - 33, dateBlockY + 18, topRight);

                    y += lineSpacing * 2;

                    // --- CERTIFICATION (centered) ---
                    gfx.DrawString("CERTIFICATION", fontHeader, XBrushes.Black, page.Width / 2, y, XStringFormats.TopCenter);
                    y += lineSpacing + 6;

                    // --- TO WHOM IT MAY CONCERN (left-aligned for justified look) ---
                    gfx.DrawString("TO WHOM IT MAY CONCERN:", fontBody, XBrushes.Black, leftMargin, y);
                    y += lineSpacing;

                    // --- Main certificate block (simulate justify with left alignment) ---
                    string fullName = $"{resident.FirstName} {resident.MiddleName} {resident.LastName}".Trim();
                    string address = resident.Address ?? "(Address not specified)";

                    // "This is to certify that   <fullName>"
                    gfx.DrawString("    This is to certify that   " + fullName, fontBody, XBrushes.Black, leftMargin, y); // manually indent for tab
                    y += lineSpacing;

                    gfx.DrawString("is a resident of " + address + ",", fontBody, XBrushes.Black, leftMargin, y);
                    y += lineSpacing;

                    gfx.DrawString("and has stayed in the above address for a period of __________ year/s and __________ month/s.",
                        fontBody, XBrushes.Black, leftMargin, y);
                    y += lineSpacing;

                    // Purpose with Others handling
                    string purposeDisplay;
                    if (purposeName.Trim().ToLower() == "others")
                        purposeDisplay = "Others: _____________________________";
                    else if (purposeName.Trim().ToLower().StartsWith("others:"))
                        purposeDisplay = purposeName;
                    else
                        purposeDisplay = purposeName;

                    gfx.DrawString("This certification is being issued upon request of the above person for: " + purposeDisplay + ".",
                        fontBody, XBrushes.Black, leftMargin, y);
                    y += lineSpacing * 2;

                    // --- Signature and Verification blocks (swapped positions) ---
                    double sigBlockY = y + 18;
                    double sigLineY = sigBlockY + 16;
                    double sigNameY = sigLineY + 18;
                    double sigTitleY = sigNameY + 16;

                    double verifyBlockY = sigBlockY;
                    double verifyX = leftMargin + 3;
                    double certX = page.Width - rightMargin - 3;

                    // Verification (LEFT)
                    gfx.DrawString("Verification of submitted documents,", fontSmall, XBrushes.Black, verifyX, verifyBlockY, XStringFormats.TopLeft);
                    gfx.DrawString("_____________________", fontSmall, XBrushes.Black, verifyX, verifyBlockY + 16, XStringFormats.TopLeft);
                    gfx.DrawString("Not valid w/ erasures/alterations", fontSmall, XBrushes.Black, verifyX, verifyBlockY + 32, XStringFormats.TopLeft);

                    // Certified by (RIGHT)
                    gfx.DrawString("Certified by:", fontBody, XBrushes.Black, certX, sigBlockY, XStringFormats.TopRight);
                    gfx.DrawString("_________________________", fontBody, XBrushes.Black, certX, sigLineY, XStringFormats.TopRight);
                    gfx.DrawString(captainName, fontHeader, XBrushes.Black, certX, sigNameY, XStringFormats.TopRight);
                    gfx.DrawString("Punong Barangay", fontBody, XBrushes.Black, certX, sigTitleY, XStringFormats.TopRight);

                    doc.Save(filename);
                }

                MessageBox.Show("Clearance PDF generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (MessageBox.Show("Open the generated PDF?", "Open PDF", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

            // --- Advanced Multi-Criteria Parsing ---
            // Supports: N:John Doe P:Loan S:Jane Doe A:30 (any order, any combination)
            // Split by space, parse known prefixes
            var criteria = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string currentPrefix = "";
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (part.Length > 2 && part[1] == ':' && "NnAaPpSs".Contains(part[0]))
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
            filtered = residents.Where(r =>
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

                    if (words.All(w =>
                            fullName.Contains(w) ||
                            spouseName.Contains(w) ||
                            purposes.Contains(w) ||
                            age.Contains(w)
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