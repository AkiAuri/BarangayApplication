﻿using System;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Personalinfo : Form
    {
        private Residents _resident;

        // Constructor accepts the shared Residents object
        public Personalinfo(Residents resident)
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

            Age.KeyPress += NumberOnly_KeyPress; // Age is now a TextBox

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

        // Populate controls from Residents object
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
            Age.Text = _resident.Age > 0 ? _resident.Age.ToString() : "";
            txtPoB.Text = _resident.PlaceOfBirth ?? "";
            cBxCivilStatus.Text = _resident.CivilStatus ?? "";
            txtVoterIdNo.Text = _resident.VoterIdNo ?? "";
            txtPoll.Text = _resident.PollingPlace ?? "";
        }

        // Copy control values into Residents object
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

            if (int.TryParse(Age.Text, out var age))
                _resident.Age = age;
            else
                _resident.Age = 0;

            _resident.PlaceOfBirth = txtPoB.Text;
            _resident.CivilStatus = cBxCivilStatus.Text;
            _resident.VoterIdNo = txtVoterIdNo.Text;
            _resident.PollingPlace = txtPoll.Text;
        }

        // --- INPUT RESTRICTION HANDLERS ---

        private void LetterOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
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
        
        // TODO: ERROR X.XX only does X.X, FIX SOON.
        // --- Height: X.XX format (max 1 digit before, 2 after) ---
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
                if (!char.IsControl(e.KeyChar) && char.IsDigit(e.KeyChar) && afterDecimal >= 3)
                    e.Handled = true;
            }
        }
        // TODO: ERROR XXX.XX only does XXX.XX, FIX SOON.
        // --- Weight: XXX.XX format (max 3 digits before, 2 after) ---
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

        private void txtPoll_TextChanged(object sender, EventArgs e)
        {
        }
    }
}