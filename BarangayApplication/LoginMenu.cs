using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Runtime.InteropServices;
using BarangayApplication.Models.Repositories;
using BCrypt.Net;
using BarangayApplication.Helpers; // <-- Add this for AutoBackupHelper

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
        SqlConnection _conn = new SqlConnection(@"Data Source=localhost,1433;Initial Catalog=sybau_database;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

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

        // Static class to hold current user's information.
        public static class CurrentUser
        {
            public static string AccountID { get; set; }
            public static string AccountName { get; set; }
            public static int RoleID { get; set; }
            public static string RoleName { get; set; }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string accountName = AccountID.Text.Trim(); // Use AccountID textbox for username
            string password = Password.Text;     // This is the password entered by the user

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your Account ID and Password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AccountID.Focus();
                return;
            }

            try
            {
                _conn.Open();

                // Query user by username only (do not check password in SQL)
                string _query = @"SELECT users.accountID, users.accountName, users.roleID, users.passwordHash, UserRoles.roleName
                                  FROM users 
                                  INNER JOIN UserRoles ON users.roleID = UserRoles.roleID
                                  WHERE users.accountName = @accountName";
                
                SqlDataAdapter _da = new SqlDataAdapter(_query, _conn);
                _da.SelectCommand.Parameters.AddWithValue("@accountName", accountName);
                
                DataTable _UserTable = new DataTable();
                _da.Fill(_UserTable);

                if (_UserTable.Rows.Count > 0)
                {
                    string storedHash = _UserTable.Rows[0]["passwordHash"].ToString();
                    // BCrypt password verification
                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        // Save current user info
                        CurrentUser.AccountID = _UserTable.Rows[0]["accountID"].ToString();
                        CurrentUser.AccountName = _UserTable.Rows[0]["accountName"].ToString();
                        CurrentUser.RoleID = Convert.ToInt32(_UserTable.Rows[0]["roleID"]);
                        CurrentUser.RoleName = _UserTable.Rows[0]["roleName"].ToString();

                        // Log successful login in UserLogs table
                        string logQuery = @"INSERT INTO UserLogs (Timestamp, UserName, Action, Description)
                                            VALUES (@Timestamp, @UserName, @Action, @Description)";
                        using (SqlCommand logCmd = new SqlCommand(logQuery, _conn))
                        {
                            logCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                            logCmd.Parameters.AddWithValue("@UserName", CurrentUser.AccountName);
                            logCmd.Parameters.AddWithValue("@Action", "Login");
                            logCmd.Parameters.AddWithValue("@Description", $"User '{CurrentUser.AccountName}' logged in successfully with role '{CurrentUser.RoleName}'.");
                            logCmd.ExecuteNonQuery();
                        }

                        // --- AUTO-BACKUP ON LOGIN ---
                        AutoBackupHelper.CheckAutoBackupOnLogin(
                            CurrentUser.AccountName,
                            CurrentUser.RoleName.Equals("Superadmin", StringComparison.OrdinalIgnoreCase)
                        );
                        // ----------------------------

                        mainMenu.Show();
                        this.Hide();
                        return;
                    }
                }

                // If we reach here, either Account ID doesn't exist or password is incorrect
                MessageBox.Show("Invalid Account ID or Password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AccountID.Clear();
                Password.Clear();
                AccountID.Focus();
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