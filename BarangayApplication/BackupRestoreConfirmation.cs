using System;
using System.Windows.Forms;

namespace BarangayApplication
{
    public partial class BackupRestoreConfirmation : Form
    {
        /// <summary>
        /// True if the user confirmed the action, false if cancelled.
        /// </summary>
        public bool IsConfirmed { get; private set; } = false;

        /// <summary>
        /// Constructor. Optionally pass a custom action message.
        /// </summary>
        public BackupRestoreConfirmation(string action = "perform this action")
        {
            InitializeComponent(); // <-- THIS IS REQUIRED
            lblWarning.Text = $"Are you sure you want to {action}? This action cannot be undone.";
        }

        private void Confirmbtn_Click(object sender, EventArgs e)
        {
            IsConfirmed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}