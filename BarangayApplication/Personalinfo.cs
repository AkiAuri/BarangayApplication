using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Personalinfo : Form
    {
        private Resident _resident;

        // Height: 1 digit before, up to 2 after decimal
        private static readonly Regex HeightRegex = new Regex(@"^\d?(\.\d{0,2})?$");
        // Weight: up to 3 digits before, up to 2 after decimal
        private static readonly Regex WeightRegex = new Regex(@"^\d{0,3}(\.\d{0,2})?$");

        // Constructor accepts the shared Resident object
        public Personalinfo(Resident resident)
        {
            InitializeComponent();
            _resident = resident ?? throw new ArgumentNullException(nameof(resident));
            
            // Letter-only handlers
            txtFname.KeyPress += LetterOnly_KeyPress;
            txtMname.KeyPress += LetterOnly_KeyPress;
            txtLname.KeyPress += LetterOnly_KeyPress;

            // Specific handlers for input restrictions and max length
            txtTelCel.KeyPress += txtTelCel_KeyPress;
            txtTelCel.TextChanged += txtTelCel_TextChanged;

            txtVoterIdNo.KeyPress += txtVoterIdNo_KeyPress;
            txtVoterIdNo.TextChanged += txtVoterIdNo_TextChanged;

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
            txtVoterIdNo.Text = _resident.VoterIDNo ?? "";
            txtPoll.Text = _resident.PollingPlace ?? "";
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
            _resident.VoterIDNo = txtVoterIdNo.Text;
            _resident.PollingPlace = txtPoll.Text;
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
            if (!char.IsControl(e.KeyChar) && txtVoterIdNo.Text.Length >= 10 && txtVoterIdNo.SelectionLength == 0)
                e.Handled = true;
        }
        private void txtVoterIdNo_TextChanged(object sender, EventArgs e)
        {
            if (txtVoterIdNo.Text.Length > 10)
                txtVoterIdNo.Text = txtVoterIdNo.Text.Substring(0, 10);
            txtVoterIdNo.SelectionStart = txtVoterIdNo.Text.Length;
        }

        // --- HEIGHT AND WEIGHT WITH REGEX ---
        private static bool IsInputValid(TextBox tb, KeyPressEventArgs e, Regex regex)
        {
            // Allow control keys (e.g., Backspace)
            if (char.IsControl(e.KeyChar))
                return true;

            // Simulate the result if this key is accepted
            string text = tb.Text;
            int selStart = tb.SelectionStart;
            int selLength = tb.SelectionLength;

            // Replace selected text with the new char
            string newText = text.Substring(0, selStart) + e.KeyChar + text.Substring(selStart + selLength);

            // Test against regex
            return regex.IsMatch(newText);
        }

        // Height: X.XX format (max 1 digit before, 2 after)
        private void txtHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            e.Handled = !IsInputValid(tb, e, HeightRegex);
        }

        // Weight: XXX.XX format (max 3 digits before, 2 after)
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            e.Handled = !IsInputValid(tb, e, WeightRegex);
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

        private void txtPoll_TextChanged(object sender, EventArgs e)
        {
        }
    }
}