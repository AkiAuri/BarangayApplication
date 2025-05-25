using System;
using System.Drawing;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Personalinfo : Form
    {
        private Resident _resident;

        public Personalinfo(Resident resident)
        {
            InitializeComponent();
            _resident = resident ?? throw new ArgumentNullException(nameof(resident));

            // Letter-only handlers
            txtFname.KeyPress += LetterOnly_KeyPress;
            txtMname.KeyPress += LetterOnly_KeyPress;
            txtLname.KeyPress += LetterOnly_KeyPress;

            // Reset color on input handlers
            txtFname.TextChanged += ResetTextBoxColor;
            txtMname.TextChanged += ResetTextBoxColor;
            txtLname.TextChanged += ResetTextBoxColor;
            txtAdd.TextChanged += ResetTextBoxColor;
            txtPoB.TextChanged += ResetTextBoxColor;
            txtHeight.TextChanged += ResetTextBoxColor;
            txtWeight.TextChanged += ResetTextBoxColor;
            txtTelCel.TextChanged += ResetTextBoxColor;
            Age.TextChanged += ResetTextBoxColor;

            // ComboBox highlight reset handlers
            cbxSex.SelectedIndexChanged += ResetComboBoxBorder;
            cBxCivilStatus.SelectedIndexChanged += ResetComboBoxBorder;

            // Input restrictions and max length
            txtTelCel.KeyPress += txtTelCel_KeyPress;
            txtTelCel.TextChanged += txtTelCel_TextChanged;
            txtHeight.KeyPress += txtHeight_KeyPress;
            txtWeight.KeyPress += txtWeight_KeyPress;

            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            // Sex and Civil Status dropdowns (IDs, not just text)
            cbxSex.Items.Clear();
            cbxSex.Items.AddRange(new object[]
            {
                new ComboBoxItem<byte>("MALE", SexIds.Male),
                new ComboBoxItem<byte>("FEMALE", SexIds.Female)
            });

            cBxCivilStatus.Items.Clear();
            cBxCivilStatus.Items.AddRange(new object[]
            {
                new ComboBoxItem<int>("SINGLE", 1),
                new ComboBoxItem<int>("MARRIED", 2),
                new ComboBoxItem<int>("WIDOWED", 3),
                new ComboBoxItem<int>("LEGALLY SEPARATED", 4)
            });

            // Load resident data into controls if editing
            LoadFromModel();
        }

        // Populate controls from Resident object
        public void LoadFromModel()
        {
            txtLname.Text = _resident.LastName ?? "";
            txtFname.Text = _resident.FirstName ?? "";
            txtMname.Text = _resident.MiddleName ?? "";
            txtAdd.Text = _resident.Address ?? "";
            txtTelCel.Text = _resident.TelCelNo ?? "";
            txtHeight.Text = _resident.Height > 0 ? _resident.Height.ToString("0.##") : "";
            txtWeight.Text = _resident.Weight > 0 ? _resident.Weight.ToString("0.##") : "";
            txtPoB.Text = _resident.PlaceOfBirth ?? "";

            dateTimePicker1.Value = _resident.DateOfBirth == DateTime.MinValue ? DateTime.Today : _resident.DateOfBirth;
            Age.Text = _resident.DateOfBirth != DateTime.MinValue ? CalculateAge(_resident.DateOfBirth).ToString() : "";

            // Sex ComboBox by SexID
            cbxSex.SelectedIndex = -1;
            foreach (ComboBoxItem<byte> item in cbxSex.Items)
            {
                if (_resident.SexID == item.Value)
                {
                    cbxSex.SelectedItem = item;
                    break;
                }
            }

            // Civil Status ComboBox by CivilStatusID
            cBxCivilStatus.SelectedIndex = -1;
            foreach (ComboBoxItem<int> item in cBxCivilStatus.Items)
            {
                if (_resident.CivilStatusID == item.Value)
                {
                    cBxCivilStatus.SelectedItem = item;
                    break;
                }
            }

            // --------- EMPLOYMENT (OCCUPATION) ---------
            if (_resident.Employments != null && _resident.Employments.Count > 0)
            {
                var occ = _resident.Employments[0];
                txtOccCompany.Text = occ.Company ?? "";
                txtOccPos1.Text = occ.Position ?? "";
                // Split LengthOfService into years and months if formatted as "X Year/s, Y Month/s"
                string[] occParts = (occ.LengthOfService ?? "").Split(',');
                if (occParts.Length > 0)
                    YearLength1.Text = occParts[0].Trim().Replace("Year/s", "").Trim();
                else
                    YearLength1.Text = "";
                if (occParts.Length > 1)
                    MonthLength1.Text = occParts[1].Trim().Replace("Month/s", "").Trim();
                else
                    MonthLength1.Text = "";
            }
            else
            {
                txtOccCompany.Text = "";
                txtOccPos1.Text = "";
                YearLength1.Text = "";
                MonthLength1.Text = "";
            }

            // --------- SPOUSE ---------
            if (_resident.Spouse != null)
            {
                txtSpouseName.Text = _resident.Spouse.SpouseName ?? "";
                if (_resident.Spouse.Employments != null && _resident.Spouse.Employments.Count > 0)
                {
                    var spEmp = _resident.Spouse.Employments[0];
                    txtSpouseCompany.Text = spEmp.Company ?? "";
                    txtSpousePos1.Text = spEmp.Position ?? "";
                    string[] spParts = (spEmp.LengthOfService ?? "").Split(',');
                    if (spParts.Length > 0)
                        YearLength3.Text = spParts[0].Trim().Replace("Year/s", "").Trim();
                    else
                        YearLength3.Text = "";
                    if (spParts.Length > 1)
                        MonthLength3.Text = spParts[1].Trim().Replace("Month/s", "").Trim();
                    else
                        MonthLength3.Text = "";
                }
                else
                {
                    txtSpouseCompany.Text = "";
                    txtSpousePos1.Text = "";
                    YearLength3.Text = "";
                    MonthLength3.Text = "";
                }
            }
            else
            {
                txtSpouseName.Text = "";
                txtSpouseCompany.Text = "";
                txtSpousePos1.Text = "";
                YearLength3.Text = "";
                MonthLength3.Text = "";
            }
        }

        // Copy control values into Resident object
        public void ApplyToModel()
        {
            _resident.LastName = txtLname.Text.Trim();
            _resident.FirstName = txtFname.Text.Trim();
            _resident.MiddleName = txtMname.Text.Trim();
            _resident.Address = txtAdd.Text.Trim();
            _resident.TelCelNo = txtTelCel.Text.Trim();

            if (decimal.TryParse(txtHeight.Text, out var height))
                _resident.Height = height;
            else
                _resident.Height = 0;

            if (decimal.TryParse(txtWeight.Text, out var weight))
                _resident.Weight = weight;
            else
                _resident.Weight = 0;

            _resident.DateOfBirth = dateTimePicker1.Value;
            _resident.PlaceOfBirth = txtPoB.Text.Trim();

            // Save SexID and navigation property
            var sexItem = cbxSex.SelectedItem as ComboBoxItem<byte>;
            if (sexItem != null)
            {
                _resident.SexID = sexItem.Value;
                _resident.Sex = new Sex { SexID = sexItem.Value, SexDescription = sexItem.Text };
            }
            else
            {
                _resident.SexID = 0;
                _resident.Sex = null;
            }

            // Save CivilStatusID and navigation property
            var civilStatusItem = cBxCivilStatus.SelectedItem as ComboBoxItem<int>;
            if (civilStatusItem != null)
            {
                _resident.CivilStatusID = civilStatusItem.Value;
                _resident.CivilStatus = new CivilStatus { CivilStatusID = civilStatusItem.Value, CivilStatusDescription = civilStatusItem.Text };
            }
            else
            {
                _resident.CivilStatusID = 1; // Default to SINGLE
                _resident.CivilStatus = null;
            }

            // --------- ADD THIS FOR EMPLOYMENT (OCCUPATION) ----------
            bool hasOcc = !string.IsNullOrWhiteSpace(txtOccCompany.Text) ||
                          !string.IsNullOrWhiteSpace(txtOccPos1.Text) ||
                          !string.IsNullOrWhiteSpace(YearLength1.Text) ||
                          !string.IsNullOrWhiteSpace(MonthLength1.Text);

            if (hasOcc)
            {
                string years = YearLength1.Text.Trim();
                string months = MonthLength1.Text.Trim();
                string lengthOfService;
                if (string.IsNullOrWhiteSpace(years) && string.IsNullOrWhiteSpace(months))
                    lengthOfService = "";
                else if (string.IsNullOrWhiteSpace(years))
                    lengthOfService = $"{months} Month/s";
                else if (string.IsNullOrWhiteSpace(months))
                    lengthOfService = $"{years} Year/s";
                else
                    lengthOfService = $"{years} Year/s, {months} Month/s";

                _resident.Employments.Clear();
                _resident.Employments.Add(new Employment
                {
                    Company = txtOccCompany.Text.Trim(),
                    Position = txtOccPos1.Text.Trim(),
                    LengthOfService = lengthOfService
                });
            }
            else
            {
                _resident.Employments.Clear();
            }

            // --------- ADD THIS FOR SPOUSE ----------
            bool hasSpouse = !string.IsNullOrWhiteSpace(txtSpouseName.Text);

            if (hasSpouse)
            {
                string spouseYears = YearLength3.Text.Trim();
                string spouseMonths = MonthLength3.Text.Trim();
                string spouseLengthOfService;
                if (string.IsNullOrWhiteSpace(spouseYears) && string.IsNullOrWhiteSpace(spouseMonths))
                    spouseLengthOfService = "";
                else if (string.IsNullOrWhiteSpace(spouseYears))
                    spouseLengthOfService = $"{spouseMonths} Month/s";
                else if (string.IsNullOrWhiteSpace(spouseMonths))
                    spouseLengthOfService = $"{spouseYears} Year/s";
                else
                    spouseLengthOfService = $"{spouseYears} Year/s, {spouseMonths} Month/s";

                var spouse = new Spouse
                {
                    SpouseName = txtSpouseName.Text.Trim(),
                    Employments = new System.Collections.Generic.List<SpouseEmployment>()
                };

                // Only add spouse employment if any details present
                if (!string.IsNullOrWhiteSpace(txtSpouseCompany.Text) ||
                    !string.IsNullOrWhiteSpace(txtSpousePos1.Text) ||
                    !string.IsNullOrWhiteSpace(spouseLengthOfService))
                {
                    spouse.Employments.Add(new SpouseEmployment
                    {
                        Company = txtSpouseCompany.Text.Trim(),
                        Position = txtSpousePos1.Text.Trim(),
                        LengthOfService = spouseLengthOfService
                    });
                }

                _resident.Spouse = spouse;
            }
            else
            {
                _resident.Spouse = null;
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now < dateOfBirth.AddYears(age)) age--;
            return age;
        }

        // --- INPUT RESTRICTION HANDLERS ---

        private void LetterOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                e.Handled = true;
        }

        private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Phone number: 11 digits only
        private void txtTelCel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
            if (!char.IsControl(e.KeyChar) && txtTelCel.Text.Length >= 11 && txtTelCel.SelectionLength == 0)
                e.Handled = true;
        }
        private void txtTelCel_TextChanged(object sender, EventArgs e)
        {
            if (txtTelCel.Text.Length > 11)
                txtTelCel.Text = txtTelCel.Text.Substring(0, 11);
            txtTelCel.SelectionStart = txtTelCel.Text.Length;
        }

        // Height: X.XX (max 4 chars including decimal)
        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Allow only control chars, digits, and decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            // Only one decimal point allowed
            if (e.KeyChar == '.' && tb.Text.Contains("."))
                e.Handled = true;

            // Enforce max length 4 including decimal (e.g., 9.99)
            // Allow selection overwrite
            int selectionLength = tb.SelectionLength;
            int textLen = tb.Text.Length;
            if (!char.IsControl(e.KeyChar) &&
                (textLen - selectionLength + 1) >= 4)
                e.Handled = true;
        }

        // Weight: YYY.YY (max 6 chars including decimal)
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Allow only control chars, digits, and decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            // Only one decimal point allowed
            if (e.KeyChar == '.' && tb.Text.Contains("."))
                e.Handled = true;

            // Enforce max length 6 including decimal (e.g., 123.45)
            // Allow selection overwrite
            int selectionLength = tb.SelectionLength;
            int textLen = tb.Text.Length;
            if (!char.IsControl(e.KeyChar) &&
                (textLen - selectionLength + 1) >= 6)
                e.Handled = true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            var today = DateTime.Today;
            var dob = dateTimePicker1.Value;
            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            Age.Text = Math.Max(age, 0).ToString();
        }

        public bool CheckRequiredFields(out string missing)
        {
            var missingFields = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(txtLname.Text))
                missingFields.Add("Last Name");
            if (string.IsNullOrWhiteSpace(txtFname.Text))
                missingFields.Add("First Name");
            if (string.IsNullOrWhiteSpace(txtMname.Text))
                missingFields.Add("Middle Name");
            if (string.IsNullOrWhiteSpace(txtAdd.Text))
                missingFields.Add("Address");
            if (string.IsNullOrWhiteSpace(txtTelCel.Text))
                missingFields.Add("Tel/Cel No.");
            if (cbxSex.SelectedItem == null)
                missingFields.Add("Sex");
            if (cBxCivilStatus.SelectedItem == null)
                missingFields.Add("Civil Status");
            if (string.IsNullOrWhiteSpace(txtPoB.Text))
                missingFields.Add("Place of Birth");

            // Date of Birth: Check if it is not min/today and 18+
            var dob = dateTimePicker1.Value;
            if (dob == DateTime.MinValue || dob == DateTime.Today)
                missingFields.Add("Date of Birth");
            else
            {
                int age = CalculateAge(dob);
                if (age < 18)
                    missingFields.Add("Date of Birth (Must be at least 18 years old)");
            }

            missing = string.Join(", ", missingFields);
            return missingFields.Count == 0;
        }

        public bool CheckRequiredFieldsAndHighlight()
        {
            bool allValid = true;
            void Highlight(Control ctrl, bool highlight)
            {
                ctrl.BackColor = highlight ? Color.MistyRose : SystemColors.Window;
                ctrl.Invalidate();
            }

            bool fnameEmpty = string.IsNullOrWhiteSpace(txtFname.Text);
            Highlight(txtFname, fnameEmpty);
            allValid &= !fnameEmpty;

            bool mnameEmpty = string.IsNullOrWhiteSpace(txtMname.Text);
            Highlight(txtMname, mnameEmpty);
            allValid &= !mnameEmpty;

            bool lnameEmpty = string.IsNullOrWhiteSpace(txtLname.Text);
            Highlight(txtLname, lnameEmpty);
            allValid &= !lnameEmpty;

            bool addEmpty = string.IsNullOrWhiteSpace(txtAdd.Text);
            Highlight(txtAdd, addEmpty);
            allValid &= !addEmpty;

            bool pobEmpty = string.IsNullOrWhiteSpace(txtPoB.Text);
            Highlight(txtPoB, pobEmpty);
            allValid &= !pobEmpty;

            bool ageEmpty = string.IsNullOrWhiteSpace(Age.Text);
            Highlight(Age, ageEmpty);
            allValid &= !ageEmpty;

            bool heightEmpty = string.IsNullOrWhiteSpace(txtHeight.Text);
            Highlight(txtHeight, heightEmpty);
            allValid &= !heightEmpty;

            bool weightEmpty = string.IsNullOrWhiteSpace(txtWeight.Text);
            Highlight(txtWeight, weightEmpty);
            allValid &= !weightEmpty;

            bool telCelEmpty = string.IsNullOrWhiteSpace(txtTelCel.Text);
            Highlight(txtTelCel, telCelEmpty);
            allValid &= !telCelEmpty;

            allValid &= HighlightIfEmptyComboBox(cbxSex);
            allValid &= HighlightIfEmptyComboBox(cBxCivilStatus);

            bool dateInvalid = dateTimePicker1.Value == default(DateTime);
            Highlight(dateTimePicker1, dateInvalid);
            allValid &= !dateInvalid;

            return allValid;
        }

        private bool HighlightIfEmptyComboBox(ComboBox cb)
        {
            if (cb.SelectedItem == null)
            {
                cb.BackColor = System.Drawing.Color.MistyRose;
                cb.FlatStyle = FlatStyle.Popup;
                cb.SelectedIndexChanged -= ResetComboBoxBorder;
                cb.SelectedIndexChanged += ResetComboBoxBorder;
                return false;
            }
            else
            {
                cb.BackColor = System.Drawing.SystemColors.Window;
                cb.FlatStyle = FlatStyle.Standard;
                cb.SelectedIndexChanged -= ResetComboBoxBorder;
                return true;
            }
        }

        private void ResetComboBoxBorder(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            cb.BackColor = System.Drawing.SystemColors.Window;
            cb.FlatStyle = FlatStyle.Standard;
            cb.SelectedIndexChanged -= ResetComboBoxBorder;
        }

        private bool HighlightIfEmpty(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.BackColor = System.Drawing.Color.MistyRose;
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.Paint += TextBox_PaintRedBorder;
                tb.TextChanged -= ResetTextBoxBorder;
                tb.TextChanged += ResetTextBoxBorder;
                return false;
            }
            else
            {
                tb.BackColor = System.Drawing.SystemColors.Window;
                tb.BorderStyle = BorderStyle.Fixed3D;
                tb.Paint -= TextBox_PaintRedBorder;
                return true;
            }
        }

        private void TextBox_PaintRedBorder(object sender, PaintEventArgs e)
        {
            var tb = sender as TextBox;
            var g = e.Graphics;
            var rect = tb.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void ResetTextBoxBorder(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.Paint -= TextBox_PaintRedBorder;
            tb.TextChanged -= ResetTextBoxBorder;
        }

        private void txtPoll_TextChanged(object sender, EventArgs e) { }
        private void ResetTextBoxColor(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.BackColor = System.Drawing.SystemColors.Window;
            }
        }
    }
}