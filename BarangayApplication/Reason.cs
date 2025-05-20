using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarangayApplication
{
    public partial class Reason : Form
    {
        public string ArchiveReason { get; private set; }

        public Reason()
        {
            InitializeComponent();
        }

        private void btnFinished_Click_1(object sender, EventArgs e)
        {
            // Validate input if needed
            ArchiveReason = txtReason.Text.Trim();
            if (string.IsNullOrWhiteSpace(ArchiveReason))
            {
                MessageBox.Show("Please provide a reason for archiving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
