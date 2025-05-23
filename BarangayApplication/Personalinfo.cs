using System;
using System.Drawing;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Personalinfo : Form
    {
        private Resident _resident;

        // Constructor accepts the shared Resident object
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

            // Specific handlers for input restrictions and max length
            txtTelCel.KeyPress += txtTelCel_KeyPress;
            txtTelCel.TextChanged += txtTelCel_TextChanged;

            txtHeight.KeyPress += txtHeight_KeyPress;
            txtWeight.KeyPress += txtWeight_KeyPress;

            // Age update on DOB change
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            // Civil status/sex dropdowns (you may want to pre-populate these)
            cbxSex.Items.Clear();
            cbxSex.Items.AddRange(new string[] { "MALE", "FEMALE" });

            cBxCivilStatus.Items.Clear();
            cBxCivilStatus.Items.AddRange(new string[] { "SINGLE", "MARRIED", "WIDOWED", "LEGALLY SEPARATED" });

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
            cbxSex.Text = _resident.Sex ?? "";
            txtHeight.Text = _resident.Height > 0 ? _resident.Height.ToString("0.##") : "";
            txtWeight.Text = _resident.Weight > 0 ? _resident.Weight.ToString("0.##") : "";
            dateTimePicker1.Value = _resident.DateOfBirth == DateTime.MinValue ? DateTime.Today : _resident.DateOfBirth;
            // Age is not a stored property anymore, so calculate for display if needed
            Age.Text = _resident.DateOfBirth != DateTime.MinValue ? CalculateAge(_resident.DateOfBirth).ToString() : "";
            txtPoB.Text = _resident.PlaceOfBirth ?? "";
            cBxCivilStatus.Text = _resident.CivilStatus ?? "";
        }

        // Copy control values into Resident object
        public void ApplyToModel()
        {
            _resident.LastName = txtLname.Text;
            _resident.FirstName = txtFname.Text;
            _resident.MiddleName = txtMname.Text;
            _resident.Address = txtAdd.Text;
            _resident.TelCelNo = txtTelCel.Text;
            _resident.Sex = cbxSex.Text;

            if (decimal.TryParse(txtHeight.Text, out var height))
                _resident.Height = height;
            else
                _resident.Height = 0;

            if (decimal.TryParse(txtWeight.Text, out var weight))
                _resident.Weight = weight;
            else
                _resident.Weight = 0;

            _resident.DateOfBirth = dateTimePicker1.Value;

            // Remove Age from model - always calculate from DateOfBirth
            // If needed elsewhere, expose as a property: int Age => CalculateAge(DateOfBirth);

            _resident.PlaceOfBirth = txtPoB.Text;
            _resident.CivilStatus = cBxCivilStatus.Text;
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
            // Allow control chars (backspace, etc.), letters, and spaces
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                e.Handled = true;
        }

        private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // --- Phone number: 11 digits only ---
        private void txtTelCel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

            // enforce max length
            if (!char.IsControl(e.KeyChar) && txtTelCel.Text.Length >= 11 && txtTelCel.SelectionLength == 0)
                e.Handled = true;
        }
        private void txtTelCel_TextChanged(object sender, EventArgs e)
        {
            if (txtTelCel.Text.Length > 11)
                txtTelCel.Text = txtTelCel.Text.Substring(0, 11);
            // Move cursor to end after change
            txtTelCel.SelectionStart = txtTelCel.Text.Length;
        }

        // --- Voter's ID: 10 digits only ---
        private void txtVoterIdNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
        private void txtVoterIdNo_TextChanged(object sender, EventArgs e)
        {
        }
        
        // TODO: ERROR X.XX only does X.X, FIX SOON.
        // Height: X.XX format (max 1 digit before, 2 after)
        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            // Only one decimal point allowed
            if (e.KeyChar == '.' && tb.Text.Contains("."))
                e.Handled = true;

            string[] parts = tb.Text.Split('.');
            int selectionStart = tb.SelectionStart;
            int beforeDecimal = parts.Length > 0 ? parts[0].Length : 0;
            int afterDecimal = parts.Length > 1 ? parts[1].Length : 0;
            bool hasDecimal = tb.Text.Contains(".");

            // 1 digit before decimal for height
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && (!hasDecimal && beforeDecimal >= 1 && selectionStart <= beforeDecimal))
                e.Handled = true;

            // 2 digits after decimal for height
            if (hasDecimal && selectionStart > tb.Text.IndexOf("."))
            {
                if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && afterDecimal >= 2)
                    e.Handled = true;
            }
        }
        
        // TODO: ERROR XXX.XX only does XXX.X, FIX SOON.
        // Weight: XXX.XX format (max 3 digits before, 2 after)
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            // Only one decimal point allowed
            if (e.KeyChar == '.' && tb.Text.Contains("."))
                e.Handled = true;

            string[] parts = tb.Text.Split('.');
            int selectionStart = tb.SelectionStart;
            int beforeDecimal = parts.Length > 0 ? parts[0].Length : 0;
            int afterDecimal = parts.Length > 1 ? parts[1].Length : 0;
            bool hasDecimal = tb.Text.Contains(".");

            // 3 digits before decimal for weight
            if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && (!hasDecimal && beforeDecimal >= 3 && selectionStart <= beforeDecimal))
                e.Handled = true;

            // 2 digits after decimal for weight
            if (hasDecimal && selectionStart > tb.Text.IndexOf("."))
            {
                if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && afterDecimal >= 2)
                    e.Handled = true;
            }
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
            if (string.IsNullOrWhiteSpace(cbxSex.Text))
                missingFields.Add("Sex");
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

            // Helper to highlight controls
            void Highlight(Control ctrl, bool highlight)
            {
                ctrl.BackColor = highlight ? Color.MistyRose : SystemColors.Window;
                ctrl.Invalidate();
            }

            // TextBoxes
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

            // ComboBoxes (use new highlight logic)
            allValid &= HighlightIfEmptyComboBox(cbxSex);
            allValid &= HighlightIfEmptyComboBox(cBxCivilStatus);



            // DateTimePicker (example: you may want to check for a valid range)
            // Here, we just check if the value is not the default
            bool dateInvalid = dateTimePicker1.Value == default(DateTime);
            Highlight(dateTimePicker1, dateInvalid);
            allValid &= !dateInvalid;

            return allValid;
        }

        // Helper method to highlight a ComboBox if empty (like in Collection.cs)
        private bool HighlightIfEmptyComboBox(ComboBox cb)
        {
            if (string.IsNullOrWhiteSpace(cb.Text) || cb.SelectedIndex < 0)
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

        // Reset border when user changes ComboBox selection
        private void ResetComboBoxBorder(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            cb.BackColor = System.Drawing.SystemColors.Window;
            cb.FlatStyle = FlatStyle.Standard;
            cb.SelectedIndexChanged -= ResetComboBoxBorder;
        }

        // Helper method to highlight a TextBox if empty
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

        // Paint event to draw a red border
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

        // Reset border when user starts typing
        private void ResetTextBoxBorder(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.Paint -= TextBox_PaintRedBorder;
            tb.TextChanged -= ResetTextBoxBorder;
        }

        private void txtPoll_TextChanged(object sender, EventArgs e)
        {
        }

        // Resets the textbox color to its original when user inputs text
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