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
    public partial class Purpose: Form
    {
        public Purpose()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance of FormApplication.
            using (FormApplication form = new FormApplication())
            {
                // Show the form as a modal dialog.
                var result = form.ShowDialog();

                // Check the result of the dialog.
                if (result == DialogResult.OK)
                {
                    // Perform any actions if the form was completed successfully.
                    MessageBox.Show("FormApplication completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Perform any actions if the form was canceled or closed.
                    MessageBox.Show("FormApplication was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Optionally hide or close the Purpose form after showing FormApplication.
            this.Hide(); // Hides the Purpose form.
        }

    }
}
