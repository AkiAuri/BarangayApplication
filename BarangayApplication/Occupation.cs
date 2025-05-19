using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Occupation : Form
    {
        private Residents _resident;

        public Occupation(Residents resident)
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
            // Parse helper for year/month strings
            void SetYearMonthFromString(string value, ComboBox yearBox, ComboBox monthBox)
            {
                int years = 0, months = 0;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Accepts formats like "3 years 2 months", "1 year", "8 months", etc.
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

            // Employment (handle null safely)
            txtOccCompany.Text = _resident.Employment?.Company ?? "";
            txtOccPos1.Text = _resident.Employment?.Position ?? "";
            SetYearMonthFromString(_resident.Employment?.LengthOfService, YearLength1, MonthLength1);
            txtOccPrev.Text = _resident.Employment?.PreviousCompany ?? "";
            txtOccPos2.Text = _resident.Employment?.PreviousPosition ?? "";
            SetYearMonthFromString(_resident.Employment?.PreviousLengthOfService, YearLength2, MonthLength2);

            // Spouse (handle null safely)
            txtSpouseName.Text = _resident.Spouse?.SpouseName ?? "";
            txtSpouseContact.Text = _resident.Spouse?.SpousePhone ?? "";
            txtSpouseCompany.Text = _resident.Spouse?.SpouseCompany ?? "";
            txtSpousePos1.Text = _resident.Spouse?.SpousePosition ?? "";
            SetYearMonthFromString(_resident.Spouse?.SpouseLengthOfService, YearLength3, MonthLength3);
            txtSpousePrev.Text = _resident.Spouse?.SpousePreviousCompany ?? "";
            txtSpousePos2.Text = _resident.Spouse?.SpousePreviousPosition ?? "";
            SetYearMonthFromString(_resident.Spouse?.SpousePreviousLengthOfService, YearLength4, MonthLength4);
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

            // Employment (create if null)
            if (_resident.Employment == null)
                _resident.Employment = new Employment();

            _resident.Employment.Company = txtOccCompany.Text;
            _resident.Employment.Position = txtOccPos1.Text;
            _resident.Employment.LengthOfService = ComposeYearMonth(YearLength1, MonthLength1);
            _resident.Employment.PreviousCompany = txtOccPrev.Text;
            _resident.Employment.PreviousPosition = txtOccPos2.Text;
            _resident.Employment.PreviousLengthOfService = ComposeYearMonth(YearLength2, MonthLength2);

            // Spouse (create if null)
            if (_resident.Spouse == null)
                _resident.Spouse = new Spouse();

            _resident.Spouse.SpouseName = txtSpouseName.Text;
            _resident.Spouse.SpousePhone = txtSpouseContact.Text;
            _resident.Spouse.SpouseCompany = txtSpouseCompany.Text;
            _resident.Spouse.SpousePosition = txtSpousePos1.Text;
            _resident.Spouse.SpouseLengthOfService = ComposeYearMonth(YearLength3, MonthLength3);
            _resident.Spouse.SpousePreviousCompany = txtSpousePrev.Text;
            _resident.Spouse.SpousePreviousPosition = txtSpousePos2.Text;
            _resident.Spouse.SpousePreviousLengthOfService = ComposeYearMonth(YearLength4, MonthLength4);
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