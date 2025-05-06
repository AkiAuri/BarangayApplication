using System;
using System.Windows.Forms;
using BarangayApplication.Models;

namespace BarangayApplication
{
    public partial class Collection : Form
    {
        private void Collection_Load(object sender, EventArgs e)
        {
            // You can leave this empty if you don't need it, or put any startup logic here.
        }
        
        private Residents _resident;

        // Accept Residents model in the constructor
        public Collection(Residents resident)
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
            // Purpose flags and others
            if (!string.IsNullOrWhiteSpace(_resident.PurposeOthers))
            {
                comboBox2.Text = "OTHER";
                txtOthers.Text = _resident.PurposeOthers;
                txtOthers.Visible = true;
                txtOthers.Enabled = true;
            }
            else
            {
                comboBox2.Text = GetPurposeText(_resident);
                txtOthers.Visible = false;
                txtOthers.Enabled = false;
                txtOthers.Text = "";
            }
        }

        // Copy values to model
        public void ApplyToModel()
        {
            _resident.ResidenceType = comboBox1.Text;

            // Clear all flags first
            ClearPurposeFlags(_resident);

            if (comboBox2.Text == "OTHER")
            {
                _resident.PurposeOthers = txtOthers.Text;
            }
            else
            {
                _resident.PurposeOthers = "";
                SetPurposeFlags(_resident, comboBox2.Text);
            }
        }

        // --- Reuse these helpers from your main code ---
        private void ClearPurposeFlags(Residents resident)
        {
            resident.PurposeResidency = false;
            resident.PurposePostalID = false;
            resident.PurposeLocalEmployment = false;
            resident.PurposeMarriage = false;
            resident.PurposeLoan = false;
            resident.PurposeMeralco = false;
            resident.PurposeBankTransaction = false;
            resident.PurposeTravelAbroad = false;
            resident.PurposeSeniorCitizen = false;
            resident.PurposeSchool = false;
            resident.PurposeMedical = false;
            resident.PurposeBurial = false;
        }
        private void SetPurposeFlags(Residents resident, string purpose)
        {
            resident.PurposeResidency = purpose.ToUpper() == "RESIDENCY";
            resident.PurposePostalID = purpose.ToUpper() == "POSTAL ID";
            resident.PurposeLocalEmployment = purpose.ToUpper() == "LOCAL EMPLOYMENT";
            resident.PurposeMarriage = purpose.ToUpper() == "MARRIAGE";
            resident.PurposeLoan = purpose.ToUpper() == "LOAN";
            resident.PurposeMeralco = purpose.ToUpper() == "MERALCO";
            resident.PurposeBankTransaction = purpose.ToUpper() == "BANK TRANSACTION";
            resident.PurposeTravelAbroad = purpose.ToUpper() == "TRAVEL ABROAD";
            resident.PurposeSeniorCitizen = purpose.ToUpper() == "SENIOR CITIZEN";
            resident.PurposeSchool = purpose.ToUpper() == "SCHOOL";
            resident.PurposeMedical = purpose.ToUpper() == "MEDICAL";
            resident.PurposeBurial = purpose.ToUpper() == "BURIAL";
        }
        private string GetPurposeText(Residents resident)
        {
            if (resident.PurposeResidency) return "RESIDENCY";
            if (resident.PurposePostalID) return "POSTAL ID";
            if (resident.PurposeLocalEmployment) return "LOCAL EMPLOYMENT";
            if (resident.PurposeMarriage) return "MARRIAGE";
            if (resident.PurposeLoan) return "LOAN";
            if (resident.PurposeMeralco) return "MERALCO";
            if (resident.PurposeBankTransaction) return "BANK TRANSACTION";
            if (resident.PurposeTravelAbroad) return "TRAVEL ABROAD";
            if (resident.PurposeSeniorCitizen) return "SENIOR CITIZEN";
            if (resident.PurposeSchool) return "SCHOOL";
            if (resident.PurposeMedical) return "MEDICAL";
            if (resident.PurposeBurial) return "BURIAL";
            return "";
        }
    }
}