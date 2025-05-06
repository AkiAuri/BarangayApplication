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
    public partial class LogoutConfirm : Form
    {
        public LogoutConfirm()
        {
            InitializeComponent(); // Constructor: Initializes the components of the form.
        }

        private void Confirmbtn_Click(object sender, EventArgs e)
        {
            LoginMenu login = new LoginMenu(); // Creates a new instance of the LoginMenu form.
            Form mainMenu = Application.OpenForms["MainMenu"]; // Attempts to retrieve the MainMenu form from the collection of open forms.

            if (mainMenu != null) // Checks if the MainMenu form was found.  This is the crucial null check.
            {
                mainMenu.Close(); // Closes the MainMenu form if it was found.
            }
            else
            {
                // Handle the situation where MainMenu is not found.  Important!
                MessageBox.Show("Error: Main menu form not found.  It may already be closed.", "Logout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Consider logging this error for debugging.  A more robust application might log this to a file or use a logging framework.
            }

            login.Show(); // Shows the LoginMenu form.
            this.Hide();    // Hides the current LogoutConfirm form.
        }

        private void Cancelbtn_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hides the LogoutConfirm form when the Cancel button is clicked.
        }
    }
}
