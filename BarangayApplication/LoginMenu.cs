using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Runtime.InteropServices;

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

        // Event handler for the Login button click.
        // Validates user credentials and navigates to the MainMenu if successful.
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Retrieve the entered account ID and password.
            String accountID, passwordHash;
            accountID = AccountID.Text;
            passwordHash = Password.Text;

            try
            {
                _conn.Open(); // Opens the database connection.

                // SQL query to select the user matching the provided credentials.
                string _query = "select * from Users where accountID = @accountID and passwordHash = @passwordHash";

                // Create a SqlDataAdapter to execute the query.
                SqlDataAdapter _da = new SqlDataAdapter(_query, _conn);

                // Add parameters to the adapter to prevent SQL injection.
                _da.SelectCommand.Parameters.AddWithValue("@accountID", accountID);
                _da.SelectCommand.Parameters.AddWithValue("@passwordHash", passwordHash);

                // Create a DataTable to hold the query results.
                DataTable _UserTable = new DataTable();
                _da.Fill(_UserTable); // Fill the DataTable with data.

                // Check if any matching user exists.
                if (_UserTable.Rows.Count > 0)
                {
                    // Successful login: Show the MainMenu form and hide the LoginMenu form.
                    mainMenu.Show();
                    this.Hide();
                }
                else
                {
                    // Failed login: Alert user and clear the credentials input fields.
                    MessageBox.Show("Account ID or password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AccountID.Clear();
                    Password.Clear();
                    AccountID.Focus(); // Set focus back to the AccountID textbox.
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, display an error message and clear input fields.
                MessageBox.Show("Something went wrong, please try again later or call CS for assistance." + ex,
                    "Error - 101", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AccountID.Clear();
                Password.Clear();
                AccountID.Focus();
            }
            finally
            {
                _conn.Close(); // Ensures the database connection is closed, even if an exception occurs.
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