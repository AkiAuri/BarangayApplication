using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BarangayApplication.Models;
using BarangayApplication.Models.Repositories;

namespace BarangayApplication
{
    public partial class MainViewForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private Resident _resident = new Resident();

        // Track current step (0=personal, 1=collection, 2=occupation)
        private int _currentStep = 0;
        // We'll store as strongly-typed for ApplyToModel access
        private Personalinfo _personalForm;
        private Collection _collectionForm;
        private Occupation _occupationForm;
        private Form[] _forms;

        // NEW: Constructor for editing existing Residents
        public MainViewForm(Resident residentToEdit)
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, Width, Height, 20, 20)
            );

            _resident = residentToEdit ?? new Resident();

            _personalForm = new Personalinfo(_resident);
            _collectionForm = new Collection(_resident);
            _occupationForm = new Occupation(_resident);
            _forms = new Form[]
            {
                _personalForm,
                _collectionForm,
                _occupationForm
            };

            // Pre-populate all subforms with data
            _personalForm.LoadFromModel();
            _collectionForm.LoadFromModel();
            _occupationForm.LoadFromModel();

            _currentStep = 0;
            loadform(_forms[_currentStep]);
        }

        // Default constructor for new entries
        public MainViewForm()
        {
            InitializeComponent();

            // Rounded form
            Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, Width, Height, 20, 20)
            );

            // Initialize the steps with the shared Residents model
            _personalForm = new Personalinfo(_resident);
            _collectionForm = new Collection(_resident);
            _occupationForm = new Occupation(_resident);
            _forms = new Form[]
            {
                _personalForm,
                _collectionForm,
                _occupationForm
            };

            // Show first step
            loadform(_forms[_currentStep]);
        }

        private void MainViewForm_Load(object sender, EventArgs e)
        {
        }

        private void loadform(object form)
        {
            if (this.MainPanel.Controls.Count > 0)
                this.MainPanel.Controls.RemoveAt(0);
            Form f = form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.MainPanel.Controls.Add(f);
            this.MainPanel.Tag = f;
            f.Show();
        }

        private void personalBtn_Click(object sender, EventArgs e)
        {
            _currentStep = 0;
            loadform(_forms[_currentStep]);
        }

        private void collectionBtn_Click(object sender, EventArgs e)
        {
            _currentStep = 1;
            loadform(_forms[_currentStep]);
        }

        private void occuBtn_Click(object sender, EventArgs e)
        {
            _currentStep = 2;
            loadform(_forms[_currentStep]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (ExitForms exit = new ExitForms())
            {
                exit.StartPosition = FormStartPosition.CenterParent;
                exit.ShowDialog(this);
            }
        }

        // "Save" button
        private void button1_Click(object sender, EventArgs e)
        {
            // Validate personal info first
            if (!_personalForm.CheckRequiredFields(out string personalMissing))
            {
                MessageBox.Show(
                    "Please fill in the following required field(s):\n" + personalMissing,
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return; // Prevent save if incomplete
            }

            // Validate collection (residence and purpose)
            if (!_collectionForm.CheckRequiredFields(out string collectionMissing))
            {
                MessageBox.Show(
                    "Please fill in the following required field(s):\n" + collectionMissing,
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return; // Prevent save if incomplete
            }
            
            // Update model from all subforms
            _personalForm.ApplyToModel();
            _collectionForm.ApplyToModel();
            _occupationForm.ApplyToModel();

            var repo = new ResidentsRepository();
            try
            {
                if (_resident.ResidentID == 0)
                    repo.CreateResident(_resident);
                else
                    repo.UpdateResident(_resident);

                // Show Finish form -- replaces MessageBox
                using (Finish finish = new Finish())
                {
                    finish.StartPosition = FormStartPosition.CenterParent;
                    finish.ShowDialog(this);
                }

                // After Finish closes, set DialogResult OK (for Data form refresh)
                this.DialogResult = DialogResult.OK;

                // Close this wizard window as well
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the resident information. Please try again.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Nextbtn_Click(object sender, EventArgs e)
        {
            if (_currentStep < _forms.Length - 1)
            {
                _currentStep++;
                loadform(_forms[_currentStep]);
            }
        }

        private void Backbtn_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                loadform(_forms[_currentStep]);
            }
        }
    }
}