using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Runtime.InteropServices;
using BarangayApplication.Models.Repositories;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BarangayApplication
{
    /// <summary>
    /// Represents the login form for the application.
    /// Allows users to enter credentials, validate against the database,
    /// and navigate to the MainMenu upon a successful login.
    /// </summary>
    public partial class LoginMenu : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );


        // Instance of the MainMenu form that is shown after a successful login.
        MainMenu mainMenu = new MainMenu();

        // Constructor: Initializes the LoginMenu form and populates initial data.
        public LoginMenu()
        {
            InitializeComponent(); // Initializes form components as defined in the designer.
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        // Event handler that runs when the form is loaded.
        private void Form1_Load(object sender, EventArgs e)
        {
            // This event handler is currently empty but can be used for additional initialization.
        }

        // Creates a SqlConnection to the SQL Server database.
        // Update the connection string as needed for your environment.
        SqlConnection _conn = new SqlConnection(@"Data Source=.;Initial Catalog=BarangayDatabase;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

        // Event handler for the checkbox that toggles password visibility.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; // Cast the sender to a CheckBox.

            if (checkBox.Checked)
            {
                // When the checkbox is checked, show the password by setting the PasswordChar to '\0'.
                Password.PasswordChar = '\0';
            }
            else
            {
                // When unchecked, hide the password by setting the PasswordChar to '*' (or any other masking character).
                Password.PasswordChar = '*';
            }
        }

        // Fix for CS0103: The name 'CurrentUser' does not exist in the current context
        // Fix for CS0103: The name 'username' does not exist in the current context

        // Assuming 'CurrentUser' is a static class that holds the current user's information,
        // and 'UserName' is a property of that class. If this class does not exist, it needs to be created.
        // Additionally, 'username' should be retrieved from the database query result.
        public static class CurrentUser
        {
            public static string AccountID { get; set; }
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            String accountID, passwordHash;
            accountID = AccountID.Text;
            passwordHash = Password.Text;

            try
            {
                _conn.Open();

                string _query = "SELECT * FROM users WHERE accountID = @accountID and passwordHash = @passwordHash";
                SqlDataAdapter _da = new SqlDataAdapter(_query, _conn);
                _da.SelectCommand.Parameters.AddWithValue("@accountID", accountID);
                _da.SelectCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                
                //for logbook username
                DataTable _UserTable = new DataTable();
                _da.Fill(_UserTable);
                
                if (_UserTable.Rows.Count > 0)
                {
                    CurrentUser.AccountID = _UserTable.Rows[0]["accountID"].ToString();
                    string accountName = _UserTable.Rows[0]["accountName"].ToString();

                    mainMenu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Account ID or password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AccountID.Clear();
                    Password.Clear();
                    AccountID.Focus();
                } //until here
            }

            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong, please try again later or call CS for assistance." + ex,
                    "Error - 101", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AccountID.Clear();
                Password.Clear();
                AccountID.Focus();
            }
            finally
            {
                _conn.Close();
            }
        }

        // Event handler for the Bypass button click.
        // Allows navigation to MainMenu without credential validation (for testing or demo purposes).
        private void Bypass_Click(object sender, EventArgs e)
        {
            mainMenu.Show(); // Display the MainMenu form.
            this.Hide();     // Hide the LoginMenu form.
        }

    }
}