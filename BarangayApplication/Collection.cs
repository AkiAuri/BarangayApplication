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

        // Accept Resident model in the constructor
        public Collection(Resident resident)
        {
            InitializeComponent();
            _resident = resident ?? throw new ArgumentNullException(nameof(resident));

            // Residency drop-down
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] {
                "OWNED",
                "RENTED",
                "BOARDERS/BEDSPACE"
            });

            // Purpose drop-down
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] {
                "BANK TRANSACTION",
                "BURIAL",
                "LOAN",
                "LOCAL EMPLOYMENT",
                "MARRIAGE",
                "MEDICAL",
                "MERALCO",
                "POSTAL ID",
                "RESIDENCY",
                "SCHOOL",
                "SENIOR CITIZEN",
                "TRAVEL ABROAD",
                "OTHER"
            });

            comboBox2.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox2.SelectedItem?.ToString() == "OTHER")
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
            comboBox1.Text = _resident.ResidenceType ?? "";

            // Use normalized Purposes navigation property (List<ResidentPurpose>)
            var purposes = _resident.Purposes;

            if (purposes != null && purposes.Any(p => p.PurposeTypeID == PurposeTypeIds.Others && !string.IsNullOrWhiteSpace(p.PurposeOthers)))
            {
                comboBox2.Text = "OTHER";
                txtOthers.Text = purposes.First(p => p.PurposeTypeID == PurposeTypeIds.Others).PurposeOthers ?? "";
                txtOthers.Visible = true;
                txtOthers.Enabled = true;
            }
            else if (purposes != null && purposes.Count > 0)
            {
                // Set combo to first matching known purpose, prioritize by order in dropdown
                string foundPurpose = null;
                foreach (var item in comboBox2.Items.Cast<string>())
                {
                    int? ptid = GetPurposeTypeId(item);
                    if (ptid != null && purposes.Any(p => p.PurposeTypeID == ptid))
                    {
                        foundPurpose = item;
                        break;
                    }
                }
                comboBox2.Text = foundPurpose ?? "";
                txtOthers.Visible = false;
                txtOthers.Enabled = false;
                txtOthers.Text = "";
            }
            else
            {
                comboBox2.Text = "";
                txtOthers.Visible = false;
                txtOthers.Enabled = false;
                txtOthers.Text = "";
            }
        }

        // Copy values to model
        public void ApplyToModel()
        {
            _resident.ResidenceType = comboBox1.Text;

            // Ensure Purposes exists
            if (_resident.Purposes == null)
                _resident.Purposes = new List<ResidentPurpose>();
            else
                _resident.Purposes.Clear();

            if (comboBox2.Text == "OTHER")
            {
                if (!string.IsNullOrWhiteSpace(txtOthers.Text))
                {
                    _resident.Purposes.Add(new ResidentPurpose
                    {
                        PurposeTypeID = PurposeTypeIds.Others,
                        PurposeOthers = txtOthers.Text
                    });
                }
            }
            else
            {
                int? ptid = GetPurposeTypeId(comboBox2.Text);
                if (ptid.HasValue)
                {
                    _resident.Purposes.Add(new ResidentPurpose
                    {
                        PurposeTypeID = ptid.Value,
                        PurposeOthers = ""
                    });
                }
            }
        }

        private int? GetPurposeTypeId(string purpose)
        {
            switch (purpose?.ToUpperInvariant())
            {
                case "RESIDENCY": return PurposeTypeIds.Residency;
                case "POSTAL ID": return PurposeTypeIds.PostalID;
                case "LOCAL EMPLOYMENT": return PurposeTypeIds.LocalEmployment;
                case "MARRIAGE": return PurposeTypeIds.Marriage;
                case "LOAN": return PurposeTypeIds.Loan;
                case "MERALCO": return PurposeTypeIds.Meralco;
                case "BANK TRANSACTION": return PurposeTypeIds.BankTransaction;
                case "TRAVEL ABROAD": return PurposeTypeIds.TravelAbroad;
                case "SENIOR CITIZEN": return PurposeTypeIds.SeniorCitizen;
                case "SCHOOL": return PurposeTypeIds.School;
                case "MEDICAL": return PurposeTypeIds.Medical;
                case "BURIAL": return PurposeTypeIds.Burial;
                case "OTHER": return PurposeTypeIds.Others;
                default: return null;
            }
        }
        
        public bool CheckRequiredFields(out string missing)
        {
            var missingFields = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(comboBox1.Text))
                missingFields.Add("Residence Type");

            if (string.IsNullOrWhiteSpace(comboBox2.Text))
                missingFields.Add("Purpose");
            else if (comboBox2.Text == "OTHER" && string.IsNullOrWhiteSpace(txtOthers.Text))
                missingFields.Add("Specify Purpose (Others)");

            missing = string.Join(", ", missingFields);
            return missingFields.Count == 0;
        }
        
        //Removing this somehow breaks the thing, so I'm just keeping it, lol.
        private void Collection_Load(object sender, EventArgs e)
        {
            // Optional: your startup logic
        }

        // Add this method to highlight required fields with red border and return validity
        public bool CheckRequiredFieldsAndHighlight()
        {
            bool allValid = true;

            // Residence Type (comboBox1) is required
            allValid &= HighlightIfEmpty(comboBox1);

            // Purpose (comboBox2) is required
            allValid &= HighlightIfEmpty(comboBox2);

            // If Purpose is "OTHER", txtOthers is required
            if (comboBox2.Text == "OTHER")
                allValid &= HighlightIfEmpty(txtOthers);
            else
                ResetHighlight(txtOthers);

            return allValid;
        }

        // Helper method to highlight a ComboBox if empty
        private bool HighlightIfEmpty(ComboBox cb)
        {
            if (string.IsNullOrWhiteSpace(cb.Text))
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

        // Reset border when user starts typing in TextBox
        private void ResetTextBoxBorder(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.TextChanged -= ResetTextBoxBorder;
        }

        // Reset border when user changes ComboBox selection
        private void ResetComboBoxBorder(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            cb.BackColor = System.Drawing.SystemColors.Window;
            cb.FlatStyle = FlatStyle.Standard;
            cb.SelectedIndexChanged -= ResetComboBoxBorder;
        }

        // Helper to reset highlight for a TextBox (used for txtOthers when not required)
        private void ResetHighlight(TextBox tb)
        {
            tb.BackColor = System.Drawing.SystemColors.Window;
            tb.BorderStyle = BorderStyle.Fixed3D;
            tb.TextChanged -= ResetTextBoxBorder;
        }
    }
}