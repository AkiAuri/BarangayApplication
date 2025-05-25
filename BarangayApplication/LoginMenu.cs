using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BarangayApplication.Models.Repositories;
using BCrypt.Net;
using BarangayApplication.Helpers;

namespace BarangayApplication
{
    /// <summary>
    /// Represents the login form for the application.
    /// Allows users to enter credentials, validate against the security database,
    /// and navigate to the MainMenu upon a successful login.
    /// </summary>
    public partial class LoginMenu : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        MainMenu mainMenu = new MainMenu();

        // Use the LOGBOOK/SECURITY database for authentication!
        private const string SecurityDbName = "ResidentsLogDB";
        private const string SecurityConnString = @"Data Source=.;Initial Catalog=" + SecurityDbName + ";Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        // AppData directory for all settings and logs
        private static readonly string AppDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BarangayApplication");
        private static string LoginErrorLogFile => Path.Combine(AppDataDir, "login_error_log.txt");

        public LoginMenu()
        {
            Directory.CreateDirectory(AppDataDir); // Ensure AppData directory exists
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        // Static class to hold current user's information
        public static class CurrentUser
        {
            public static string AccountID { get; set; }
            public static string AccountName { get; set; }
            public static int RoleID { get; set; }
            public static string RoleName { get; set; }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Password.PasswordChar = ((CheckBox)sender).Checked ? '\0' : '*';
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string accountName = AccountID.Text.Trim();
            string password = Password.Text;

            if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your Account ID and Password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AccountID.Focus();
                return;
            }

            try
            {
                using (var conn = new SqlConnection(SecurityConnString))
                {
                    conn.Open();
                    string query = @"
                        SELECT u.accountID, u.accountName, u.roleID, u.passwordHash, r.roleName
                        FROM users u
                        INNER JOIN UserRoles r ON u.roleID = r.roleID
                        WHERE u.accountName = @accountName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@accountName", accountName);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["passwordHash"].ToString();
                                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                                {
                                    CurrentUser.AccountID = reader["accountID"].ToString();
                                    CurrentUser.AccountName = reader["accountName"].ToString();
                                    CurrentUser.RoleID = Convert.ToInt32(reader["roleID"]);
                                    CurrentUser.RoleName = reader["roleName"].ToString();

                                    // Log successful login in UserLogs table (use a new connection to avoid issues)
                                    LogUserLogin(CurrentUser.AccountID, CurrentUser.AccountName, CurrentUser.RoleName);

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
                        }
                    }
                }

                MessageBox.Show("Incorrect Account ID or Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AccountID.Clear();
                Password.Clear();
                AccountID.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong, please try again later or call CS for assistance.\n\n" + ex.Message,
                    "Error - 101", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AccountID.Clear();
                Password.Clear();
                AccountID.Focus();
            }
        }

        private void LogUserLogin(string accountId, string userName, string roleName)
        {
            try
            {
                using (var conn = new SqlConnection(SecurityConnString))
                {
                    conn.Open();
                    // Get ActionID for 'login'
                    int actionId;
                    using (var actionCmd = new SqlCommand("SELECT ActionID FROM UserActions WHERE ActionName = @name", conn))
                    {
                        actionCmd.Parameters.AddWithValue("@name", "login");
                        object result = actionCmd.ExecuteScalar();
                        if (result == null)
                            throw new Exception("Action 'login' not found in UserActions table.");
                        actionId = Convert.ToInt32(result);
                    }

                    string logQuery = @"
                INSERT INTO UserLogs (Timestamp, AccountID, ActionID, Description)
                VALUES (@Timestamp, @AccountID, @ActionID, @Description)";
                    using (SqlCommand logCmd = new SqlCommand(logQuery, conn))
                    {
                        logCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        logCmd.Parameters.AddWithValue("@AccountID", int.Parse(accountId)); // must be int
                        logCmd.Parameters.AddWithValue("@ActionID", actionId);
                        logCmd.Parameters.AddWithValue("@Description", $"User '{userName}' logged in successfully with role '{roleName}'.");
                        logCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Directory.CreateDirectory(AppDataDir); // Ensure directory exists in case not created yet
                    File.AppendAllText(LoginErrorLogFile,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Failed to log user login for '{userName}': {ex.Message}{Environment.NewLine}");
                }
                catch { }
            }
        }

        // Optional: For testing/demo only. Remove or guard this in production!
        private void Bypass_Click(object sender, EventArgs e)
        {
            mainMenu.Show();
            this.Hide();
        }
    }
}