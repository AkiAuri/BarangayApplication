using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Collection : Form
    {
        private Resident _resident;

        public Collection(Resident resident)
        {
            InitializeComponent();
            _resident = resident ?? throw new ArgumentNullException(nameof(resident));

            // Residence Type drop-down (binds to ResidenceTypeID)
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[]
            {
                new ComboBoxItem<int>("OWNED", ResidenceTypeIds.Owned),
                new ComboBoxItem<int>("RENTED", ResidenceTypeIds.Rented),
                new ComboBoxItem<int>("BOARDERS/BEDSPACE", ResidenceTypeIds.BoardersBedspacer)
            });

            // Purpose drop-down (binds to PurposeTypeID)
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new object[]
            {
                new ComboBoxItem<int>("BANK TRANSACTION", PurposeTypeIds.BankTransaction),
                new ComboBoxItem<int>("BURIAL", PurposeTypeIds.Burial),
                new ComboBoxItem<int>("LOAN", PurposeTypeIds.Loan),
                new ComboBoxItem<int>("LOCAL EMPLOYMENT", PurposeTypeIds.LocalEmployment),
                new ComboBoxItem<int>("MARRIAGE", PurposeTypeIds.Marriage),
                new ComboBoxItem<int>("MEDICAL", PurposeTypeIds.Medical),
                new ComboBoxItem<int>("MERALCO", PurposeTypeIds.Meralco),
                new ComboBoxItem<int>("POSTAL ID", PurposeTypeIds.PostalID),
                new ComboBoxItem<int>("RESIDENCY", PurposeTypeIds.Residency),
                new ComboBoxItem<int>("SCHOOL", PurposeTypeIds.School),
                new ComboBoxItem<int>("SENIOR CITIZEN", PurposeTypeIds.SeniorCitizen),
                new ComboBoxItem<int>("TRAVEL ABROAD", PurposeTypeIds.TravelAbroad),
                new ComboBoxItem<int>("OTHER", PurposeTypeIds.Others)
            });

            comboBox2.SelectedIndexChanged += (s, e) =>
            {
                var selected = comboBox2.SelectedItem as ComboBoxItem<int>;
                if (selected != null && selected.Value == PurposeTypeIds.Others)
                {
                    txtOthers.Enabled = true;
                    txtOthers.Visible = true;
                    txtOthers.Focus();
                }
                else
                {
                    txtOthers.Enabled = false;
                    txtOthers.Visible = false;
                    txtOthers.Text = "";
                }
            };

            txtOthers.Enabled = false;
            txtOthers.Visible = false;

            LoadFromModel();
        }

        // Populate controls from model
        public void LoadFromModel()
        {
            // Residence TypeID
            if (_resident.ResidenceTypeID != 0)
            {
                comboBox1.SelectedIndex = -1;
                foreach (ComboBoxItem<int> item in comboBox1.Items)
                {
                    if (item.Value == _resident.ResidenceTypeID)
                    {
                        comboBox1.SelectedItem = item;
                        break;
                    }
                }
            }

            // Use normalized Purposes navigation property (List<ResidentPurpose>)
            var purposes = _resident.Purposes;

            if (purposes != null && purposes.Any(p => p.PurposeTypeID == PurposeTypeIds.Others && !string.IsNullOrWhiteSpace(p.PurposeOthers)))
            {
                // Set to "OTHER"
                foreach (ComboBoxItem<int> item in comboBox2.Items)
                {
                    if (item.Value == PurposeTypeIds.Others)
                    {
                        comboBox2.SelectedItem = item;
                        break;
                    }
                }
                txtOthers.Text = purposes.First(p => p.PurposeTypeID == PurposeTypeIds.Others).PurposeOthers ?? "";
                txtOthers.Visible = true;
                txtOthers.Enabled = true;
            }
            else if (purposes != null && purposes.Count > 0)
            {
                // Set combo to first matching known purpose
                ComboBoxItem<int> foundItem = null;
                foreach (ComboBoxItem<int> item in comboBox2.Items)
                {
                    if (purposes.Any(p => p.PurposeTypeID == item.Value))
                    {
                        foundItem = item;
                        break;
                    }
                }
                comboBox2.SelectedItem = foundItem;
                txtOthers.Visible = false;
                txtOthers.Enabled = false;
                txtOthers.Text = "";
            }
            else
            {
                comboBox2.SelectedIndex = -1;
                txtOthers.Visible = false;
                txtOthers.Enabled = false;
                txtOthers.Text = "";
            }
        }

        // Copy values to model
        public void ApplyToModel()
        {
            // Residence TypeID by ComboBox
            if (comboBox1.SelectedItem is ComboBoxItem<int> resType)
                _resident.ResidenceTypeID = resType.Value;
            else
                _resident.ResidenceTypeID = 0;

            // Ensure Purposes exists
            if (_resident.Purposes == null)
                _resident.Purposes = new List<ResidentPurpose>();
            else
                _resident.Purposes.Clear();

            if (comboBox2.SelectedItem is ComboBoxItem<int> purposeItem)
            {
                if (purposeItem.Value == PurposeTypeIds.Others)
                {
                    if (!string.IsNullOrWhiteSpace(txtOthers.Text))
                    {
                        _resident.Purposes.Add(new ResidentPurpose
                        {
                            PurposeTypeID = PurposeTypeIds.Others,
                            PurposeOthers = txtOthers.Text.Trim()
                        });
                    }
                }
                else
                {
                    _resident.Purposes.Add(new ResidentPurpose
                    {
                        PurposeTypeID = purposeItem.Value,
                        PurposeOthers = ""
                    });
                }
            }
        }

        public bool CheckRequiredFields(out string missing)
        {
            var missingFields = new List<string>();

            if (comboBox1.SelectedItem == null)
                missingFields.Add("Residence Type");

            if (comboBox2.SelectedItem == null)
                missingFields.Add("Purpose");
            else if (comboBox2.SelectedItem is ComboBoxItem<int> item &&
                     item.Value == PurposeTypeIds.Others && string.IsNullOrWhiteSpace(txtOthers.Text))
                missingFields.Add("Specify Purpose (Others)");

            missing = string.Join(", ", missingFields);
            return missingFields.Count == 0;
        }

        // Add this method to highlight required fields with red border and return validity
        public bool CheckRequiredFieldsAndHighlight()
        {
            bool allValid = true;

            allValid &= HighlightIfEmpty(comboBox1);
            allValid &= HighlightIfEmpty(comboBox2);

            if (comboBox2.SelectedItem is ComboBoxItem<int> item && item.Value == PurposeTypeIds.Others)
                allValid &= HighlightIfEmpty(txtOthers);
            else
                ResetHighlight(txtOthers);

            return allValid;
        }

        // Helper method to highlight a ComboBox if empty
        private bool HighlightIfEmpty(ComboBox cb)
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

        // Helper method to highlight a TextBox if empty
        private bool HighlightIfEmpty(TextBox tb)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.BackColor = System.Drawing.Color.MistyRose;
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.TextChanged -= ResetTextBoxBorder;
                tb.TextChanged += ResetTextBoxBorder;
                return false;
            }
            else
            {
                tb.BackColor = System.Drawing.SystemColors.Window;
                tb.BorderStyle = BorderStyle.Fixed3D;
                tb.TextChanged -= ResetTextBoxBorder;
                return true;
            }
        }

        private void ResetTextBoxBorder(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.TextChanged -= ResetTextBoxBorder;
        }

        private void ResetComboBoxBorder(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            cb.BackColor = System.Drawing.SystemColors.Window;
            cb.FlatStyle = FlatStyle.Standard;
            cb.SelectedIndexChanged -= ResetComboBoxBorder;
        }

        private void ResetHighlight(TextBox tb)
        {
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.TextChanged -= ResetTextBoxBorder;
        }
        
        private void Collection_Load(object sender, EventArgs e)
        {
            // Optional: Add any startup logic here or leave empty if not needed. Literally just to fix an error.
        }
    }
}