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

            // Use Purposes navigation property
            var purposes = _resident.Purposes;

            if (purposes != null && !string.IsNullOrWhiteSpace(purposes.PurposeOthers))
            {
                comboBox2.Text = "OTHER";
                txtOthers.Text = purposes.PurposeOthers;
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

            // Ensure Purposes exists
            if (_resident.Purposes == null)
                _resident.Purposes = new Purposes();

            // Clear all flags first
            ClearPurposeFlags(_resident.Purposes);

            if (comboBox2.Text == "OTHER")
            {
                _resident.Purposes.PurposeOthers = txtOthers.Text;
            }
            else
            {
                _resident.Purposes.PurposeOthers = "";
                SetPurposeFlags(_resident.Purposes, comboBox2.Text);
            }
        }

        // --- Updated helpers to use Purposes ---

        private void ClearPurposeFlags(Purposes purposes)
        {
            purposes.PurposeResidency = false;
            purposes.PurposePostalID = false;
            purposes.PurposeLocalEmployment = false;
            purposes.PurposeMarriage = false;
            purposes.PurposeLoan = false;
            purposes.PurposeMeralco = false;
            purposes.PurposeBankTransaction = false;
            purposes.PurposeTravelAbroad = false;
            purposes.PurposeSeniorCitizen = false;
            purposes.PurposeSchool = false;
            purposes.PurposeMedical = false;
            purposes.PurposeBurial = false;
        }

        private void SetPurposeFlags(Purposes purposes, string purpose)
        {
            var value = purpose.ToUpper();
            purposes.PurposeResidency = value == "RESIDENCY";
            purposes.PurposePostalID = value == "POSTAL ID";
            purposes.PurposeLocalEmployment = value == "LOCAL EMPLOYMENT";
            purposes.PurposeMarriage = value == "MARRIAGE";
            purposes.PurposeLoan = value == "LOAN";
            purposes.PurposeMeralco = value == "MERALCO";
            purposes.PurposeBankTransaction = value == "BANK TRANSACTION";
            purposes.PurposeTravelAbroad = value == "TRAVEL ABROAD";
            purposes.PurposeSeniorCitizen = value == "SENIOR CITIZEN";
            purposes.PurposeSchool = value == "SCHOOL";
            purposes.PurposeMedical = value == "MEDICAL";
            purposes.PurposeBurial = value == "BURIAL";
        }

        private string GetPurposeText(Residents resident)
        {
            var purposes = resident.Purposes;
            if (purposes == null) return "";
            if (purposes.PurposeResidency) return "RESIDENCY";
            if (purposes.PurposePostalID) return "POSTAL ID";
            if (purposes.PurposeLocalEmployment) return "LOCAL EMPLOYMENT";
            if (purposes.PurposeMarriage) return "MARRIAGE";
            if (purposes.PurposeLoan) return "LOAN";
            if (purposes.PurposeMeralco) return "MERALCO";
            if (purposes.PurposeBankTransaction) return "BANK TRANSACTION";
            if (purposes.PurposeTravelAbroad) return "TRAVEL ABROAD";
            if (purposes.PurposeSeniorCitizen) return "SENIOR CITIZEN";
            if (purposes.PurposeSchool) return "SCHOOL";
            if (purposes.PurposeMedical) return "MEDICAL";
            if (purposes.PurposeBurial) return "BURIAL";
            return "";
        }
    }
}