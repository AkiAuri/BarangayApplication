using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Occupation : Form
    {
        private Resident _resident;

        public Occupation(Resident resident)
        {
            InitializeComponent();
            _resident = resident ?? throw new ArgumentNullException(nameof(resident));

            // Alphanumeric for spouse full name
            txtSpouseName.KeyPress += AlphanumericOnly_KeyPress;

            // Numbers only for spouse contact, 11 digits max (PH phone standard)
            txtSpouseContact.KeyPress += txtSpouseContact_KeyPress;
            txtSpouseContact.TextChanged += txtSpouseContact_TextChanged;

            LoadFromModel();
        }

        // Load model data into controls
        public void LoadFromModel()
        {
            // Helper for getting the first value from a list or default
            string GetCompany(List<Employment> emps) => emps != null && emps.Count > 0 ? emps[0].Company : "";
            string GetPosition(List<Employment> emps) => emps != null && emps.Count > 0 ? emps[0].Position : "";
            string GetLengthOfService(List<Employment> emps) => emps != null && emps.Count > 0 ? emps[0].LengthOfService : "";

            string GetPrevCompany(List<PreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Company : "";
            string GetPrevPosition(List<PreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Position : "";
            string GetPrevLengthOfService(List<PreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].LengthOfService : "";

            // Employment (handle null safely)
            txtOccCompany.Text = GetCompany(_resident.Employments);
            txtOccPos1.Text = GetPosition(_resident.Employments);
            SetYearMonthFromString(GetLengthOfService(_resident.Employments), YearLength1, MonthLength1);

            txtOccPrev.Text = GetPrevCompany(_resident.PreviousEmployments);
            txtOccPos2.Text = GetPrevPosition(_resident.PreviousEmployments);
            SetYearMonthFromString(GetPrevLengthOfService(_resident.PreviousEmployments), YearLength2, MonthLength2);

            // Spouse (handle null safely)
            txtSpouseName.Text = _resident.Spouse?.SpouseName ?? "";
            txtSpouseContact.Text = _resident.Spouse?.SpousePhone ?? "";

            string GetSpouseCompany(List<SpouseEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Company : "";
            string GetSpousePosition(List<SpouseEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Position : "";
            string GetSpouseLengthOfService(List<SpouseEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].LengthOfService : "";

            string GetSpousePrevCompany(List<SpousePreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Company : "";
            string GetSpousePrevPosition(List<SpousePreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].Position : "";
            string GetSpousePrevLengthOfService(List<SpousePreviousEmployment> emps) => emps != null && emps.Count > 0 ? emps[0].LengthOfService : "";

            txtSpouseCompany.Text = GetSpouseCompany(_resident.Spouse?.Employments);
            txtSpousePos1.Text = GetSpousePosition(_resident.Spouse?.Employments);
            SetYearMonthFromString(GetSpouseLengthOfService(_resident.Spouse?.Employments), YearLength3, MonthLength3);

            txtSpousePrev.Text = GetSpousePrevCompany(_resident.Spouse?.PreviousEmployments);
            txtSpousePos2.Text = GetSpousePrevPosition(_resident.Spouse?.PreviousEmployments);
            SetYearMonthFromString(GetSpousePrevLengthOfService(_resident.Spouse?.PreviousEmployments), YearLength4, MonthLength4);

            // Local helper: parse "3 years 2 months" type string
            void SetYearMonthFromString(string value, ComboBox yearBox, ComboBox monthBox)
            {
                int years = 0, months = 0;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length - 1; i += 2)
                    {
                        if (parts[i + 1].StartsWith("year", StringComparison.OrdinalIgnoreCase) && int.TryParse(parts[i], out int y))
                            years = y;
                        else if (parts[i + 1].StartsWith("month", StringComparison.OrdinalIgnoreCase) && int.TryParse(parts[i], out int m))
                            months = m;
                    }
                }
                yearBox.SelectedItem = years > 0 ? years.ToString() : "0";
                monthBox.SelectedItem = months > 0 ? months.ToString() : "0";
            }
        }

        // Save control values back to model
        public void ApplyToModel()
        {
            // Helper to join to "X years Y months" (skipping zeroes)
            string ComposeYearMonth(ComboBox yearBox, ComboBox monthBox)
            {
                int years = int.TryParse(yearBox.SelectedItem?.ToString(), out int y) ? y : 0;
                int months = int.TryParse(monthBox.SelectedItem?.ToString(), out int m) ? m : 0;
                var sb = new List<string>();
                if (years > 0) sb.Add($"{years} year{(years > 1 ? "s" : "")}");
                if (months > 0) sb.Add($"{months} month{(months > 1 ? "s" : "")}");
                return sb.Count > 0 ? string.Join(" ", sb) : "";
            }

            // Employment (create list if null)
            if (_resident.Employments == null)
                _resident.Employments = new List<Employment>();
            if (_resident.Employments.Count == 0)
                _resident.Employments.Add(new Employment());
            _resident.Employments[0].Company = txtOccCompany.Text;
            _resident.Employments[0].Position = txtOccPos1.Text;
            _resident.Employments[0].LengthOfService = ComposeYearMonth(YearLength1, MonthLength1);

            // Previous Employment (create list if null)
            if (_resident.PreviousEmployments == null)
                _resident.PreviousEmployments = new List<PreviousEmployment>();
            if (_resident.PreviousEmployments.Count == 0)
                _resident.PreviousEmployments.Add(new PreviousEmployment());
            _resident.PreviousEmployments[0].Company = txtOccPrev.Text;
            _resident.PreviousEmployments[0].Position = txtOccPos2.Text;
            _resident.PreviousEmployments[0].LengthOfService = ComposeYearMonth(YearLength2, MonthLength2);

            // Spouse (create if null)
            if (_resident.Spouse == null)
                _resident.Spouse = new Spouse();

            _resident.Spouse.SpouseName = txtSpouseName.Text;
            _resident.Spouse.SpousePhone = txtSpouseContact.Text;

            // Spouse Employment (create list if null)
            if (_resident.Spouse.Employments == null)
                _resident.Spouse.Employments = new List<SpouseEmployment>();
            if (_resident.Spouse.Employments.Count == 0)
                _resident.Spouse.Employments.Add(new SpouseEmployment());
            _resident.Spouse.Employments[0].Company = txtSpouseCompany.Text;
            _resident.Spouse.Employments[0].Position = txtSpousePos1.Text;
            _resident.Spouse.Employments[0].LengthOfService = ComposeYearMonth(YearLength3, MonthLength3);

            // Spouse Previous Employment (create list if null)
            if (_resident.Spouse.PreviousEmployments == null)
                _resident.Spouse.PreviousEmployments = new List<SpousePreviousEmployment>();
            if (_resident.Spouse.PreviousEmployments.Count == 0)
                _resident.Spouse.PreviousEmployments.Add(new SpousePreviousEmployment());
            _resident.Spouse.PreviousEmployments[0].Company = txtSpousePrev.Text;
            _resident.Spouse.PreviousEmployments[0].Position = txtSpousePos2.Text;
            _resident.Spouse.PreviousEmployments[0].LengthOfService = ComposeYearMonth(YearLength4, MonthLength4);
        }
        private void AlphanumericOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        // --- Spouse Phone Number: Only digits, max 11 ---
        private void txtSpouseContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;

            if (!char.IsControl(e.KeyChar) && txtSpouseContact.Text.Length >= 11 && txtSpouseContact.SelectionLength == 0)
                e.Handled = true;
        }

        private void txtSpouseContact_TextChanged(object sender, EventArgs e)
        {
            if (txtSpouseContact.Text.Length > 11)
                txtSpouseContact.Text = txtSpouseContact.Text.Substring(0, 11);

            txtSpouseContact.SelectionStart = txtSpouseContact.Text.Length;
        }

        // You can keep this if you use it for other number-only fields
        private void NumberOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtOccCompany_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOccCompany_Click(object sender, EventArgs e)
        {

        }

        private void lblOccLOS1_Click(object sender, EventArgs e)
        {

        }

        private void txtOccPos1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOccPos1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtOccLOS2_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOccLOS2_Click(object sender, EventArgs e)
        {

        }

        private void txtOccPos2_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOccPos2_Click(object sender, EventArgs e)
        {

        }

        private void txtOccPrev_TextChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblOccPrev_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtSpouseCompany_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSpouseName_Click(object sender, EventArgs e)
        {

        }

        private void txtSpouseName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpousePos1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpouseLOS1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpouseLOS2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpousePos2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpousePrev_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSpouseContact_Click(object sender, EventArgs e)
        {

        }

        private void lblSpouseLOS1_Click(object sender, EventArgs e)
        {

        }

        private void lblSpsCompany_Click(object sender, EventArgs e)
        {

        }

        private void lblSpouseLOS2_Click(object sender, EventArgs e)
        {

        }

        private void lblSpousePos2_Click(object sender, EventArgs e)
        {

        }

        private void lblSpousePrev_Click(object sender, EventArgs e)
        {

        }

        private void lblSpousePos1_Click(object sender, EventArgs e)
        {

        }

        private void nudAge_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtOccLOS1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}