using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BCrypt.Net;

namespace BarangayApplication
{
    public partial class Settings : Form
    {
        // AppData directory for all settings and backup info
        private static readonly string AppDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BarangayApplication");
        private const string LastBackupFilePrefix = "last_backup_";
        private const string BackupLocationFileName = "backup_location.txt";
        private static string BackupLocationFile => Path.Combine(AppDataDir, BackupLocationFileName);
        private static string LastBackupFile(string dbName) => Path.Combine(AppDataDir, $"{LastBackupFilePrefix}{dbName}.txt");

        private static readonly string[] DatabaseNames = { "ResidentsDB", "ResidentsArchiveDB", "ResidentsLogDB" };
        private static readonly string[] DatabaseDisplayNames = { "Main", "Archive", "Logbook" };
        private const string ServerName = @".";
        public const string ConnectionStringFormat = "Data Source=" + ServerName + ";Initial Catalog={0};Integrated Security=True";
        private const string SecurityDbName = "ResidentsLogDB";
        private const string SecurityConnString = "Data Source=.;Initial Catalog=" + SecurityDbName + ";Integrated Security=True";

        public Settings()
        {
            Directory.CreateDirectory(AppDataDir); // Ensure directory exists
            InitializeComponent();
            LoadBackupLocation();
            LoadLastBackupAll();
            cbxRestoreChoice.Items.Clear();
            cbxRestoreChoice.Items.AddRange(DatabaseDisplayNames);
            cbxRestoreChoice.SelectedIndex = 0; // Optional: Select the first item by default
            PopulateAdminAccountIDs();
        }

        // Backup location load/save as before
        private void LoadBackupLocation()
        {
            if (File.Exists(BackupLocationFile))
            {
                BackupLoc.Text = File.ReadAllText(BackupLocationFile);
            }
        }
        private void SaveBackupLocation(string path)
        {
            File.WriteAllText(BackupLocationFile, path);
        }

        // Last backup load/save (per-db)
        private void LoadLastBackupAll()
        {
            lblDateTimeMain.Text = LoadLastBackup("ResidentsDB");
            lblDateTimeArchive.Text = LoadLastBackup("ResidentsArchiveDB");
            lblDateTimeLogbook.Text = LoadLastBackup("ResidentsLogDB");
        }
        private string LoadLastBackup(string dbName)
        {
            string file = LastBackupFile(dbName);
            if (File.Exists(file))
                return File.ReadAllText(file);
            return "No backup yet.";
        }
        private void SaveLastBackup(string dbName, DateTime dateTime)
        {
            string text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            File.WriteAllText(LastBackupFile(dbName), text);

            // Update label if this is from the UI thread
            if (dbName == "ResidentsDB") lblDateTimeMain.Text = text;
            else if (dbName == "ResidentsArchiveDB") lblDateTimeArchive.Text = text;
            else if (dbName == "ResidentsLogDB") lblDateTimeLogbook.Text = text;
        }

        // Choose backup location
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDlg = new FolderBrowserDialog())
            {
                folderDlg.Description = "Select Backup Location";
                folderDlg.ShowNewFolderButton = true;

                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    BackupLoc.Text = folderDlg.SelectedPath;
                    SaveBackupLocation(folderDlg.SelectedPath);
                }
            }
        }

        // Backup all databases
        private void btnBackup_Click(object sender, EventArgs e)
        {
            // Show confirmation popup
            var confirmForm = new BackupRestoreConfirmation("backup all databases");
            if (confirmForm.ShowDialog() != DialogResult.OK || !confirmForm.IsConfirmed)
            {
                // User cancelled
                return;
            }

            string backupDirectory = BackupLoc.Text.Trim();
            if (string.IsNullOrEmpty(backupDirectory) || !Directory.Exists(backupDirectory))
            {
                MessageBox.Show("Please select a valid backup location first.");
                return;
            }

            bool allSuccess = true;
            string message = "";

            for (int i = 0; i < DatabaseNames.Length; i++)
            {
                string dbName = DatabaseNames[i];
                string displayName = DatabaseDisplayNames[i];

                string monthFolder = Path.Combine(backupDirectory, $"{displayName}Backup", DateTime.Now.ToString("MMMM"));
                Directory.CreateDirectory(monthFolder);
                string bakFile = Path.Combine(monthFolder, $"Backup_{displayName}_{DateTime.Now:ddMMyyyy}.bak");
                string query = $"BACKUP DATABASE [{dbName}] TO DISK = '{bakFile}'";

                string connString = string.Format(ConnectionStringFormat, dbName);
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            SaveLastBackup(dbName, DateTime.Now);
                            message += $"{displayName} backup successful:\n{bakFile}\n\n";
                        }
                        catch (Exception ex)
                        {
                            message += $"{displayName} backup failed: {ex.Message}\n\n";
                            allSuccess = false;
                        }
                    }
                }
            }
            MessageBox.Show(message.Trim());
        }

        // Restore a chosen database
        private void btnRestore_Click(object sender, EventArgs e)
        {
            int dbIndex = cbxRestoreChoice.SelectedIndex;
            if (dbIndex < 0)
            {
                MessageBox.Show("Please select a database to restore.");
                return;
            }
            string dbName = DatabaseNames[dbIndex];
            string displayName = DatabaseDisplayNames[dbIndex];

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Backup Files (*.bak)|*.bak";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            string backupFile = openFileDialog.FileName;

            // Simple validation: check filename contains displayName or dbName
            if (backupFile.IndexOf(displayName, StringComparison.OrdinalIgnoreCase) < 0
                && backupFile.IndexOf(dbName, StringComparison.OrdinalIgnoreCase) < 0)
            {
                MessageBox.Show($"Selected file does not appear to be a {displayName} backup file.");
                return;
            }

            // Show confirmation popup
            var confirmForm = new BackupRestoreConfirmation($"restore the {displayName} database");
            if (confirmForm.ShowDialog() != DialogResult.OK || !confirmForm.IsConfirmed)
            {
                // User cancelled
                return;
            }

            string setSingleUser = $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            string restoreQuery = $"RESTORE DATABASE [{dbName}] FROM DISK = '{backupFile}' WITH REPLACE";
            string setMultiUser = $"ALTER DATABASE [{dbName}] SET MULTI_USER";
            string restoreConnectionString = "Data Source=" + ServerName + ";Initial Catalog=master;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(restoreConnectionString))
                {
                    conn.Open();
                    // 1. Set single user mode
                    using (SqlCommand cmd = new SqlCommand(setSingleUser, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // 2. Restore database
                    using (SqlCommand cmd = new SqlCommand(restoreQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // 3. Set multi user mode
                    using (SqlCommand cmd = new SqlCommand(setMultiUser, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"{displayName} restore successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{displayName} restore failed: {ex.Message}");
            }
        }
        
        //Account stuff, unchanged...
        public class AccountItem
        {
            public int AccountID { get; set; }
            public string AccountName { get; set; }
            public override string ToString() => AccountName;
        }
        
        private void PopulateAdminAccountIDs()
        {
            cmbAccountID.Items.Clear();
            using (var conn = new SqlConnection(SecurityConnString))
            using (var cmd = new SqlCommand("SELECT accountID, accountName FROM users WHERE roleID = 2", conn))
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbAccountID.Items.Add(new AccountItem
                            {
                                AccountID = reader.GetInt32(0),
                                AccountName = reader.GetString(1)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading admin accounts: " + ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbAccountID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountID.SelectedItem is AccountItem selected)
            {
                txtAccountName.Text = selected.AccountName;
            }
            else
            {
                txtAccountName.Text = "";
            }
            txtNewPassword.Text = "";
        }
        
       private void btnSetPassword_Click(object sender, EventArgs e)
       {
            if (!(cmbAccountID.SelectedItem is AccountItem selected))
            {
                MessageBox.Show("Please select an account.");
                return;
            }

            string newAccountName = txtAccountName.Text.Trim();
            string newPassword = txtNewPassword.Text;

            bool updateName = !string.IsNullOrEmpty(newAccountName) && newAccountName != selected.AccountName;
            bool updatePassword = !string.IsNullOrEmpty(newPassword);

            if (!updateName && !updatePassword)
            {
                MessageBox.Show("No changes to update.");
                return;
            }

            // Build dynamic SQL and parameters
            string setClause = "";
            if (updateName) setClause += "accountName = @name";
            if (updatePassword)
            {
                if (setClause.Length > 0) setClause += ", ";
                setClause += "passwordHash = @hash";
            }
            string query = $"UPDATE users SET {setClause} WHERE accountID = @accountID";

            using (var conn = new SqlConnection(SecurityConnString))
            using (var cmd = new SqlCommand(query, conn))
            {
                if (updateName)
                    cmd.Parameters.AddWithValue("@name", newAccountName);

                if (updatePassword)
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 16);
                    cmd.Parameters.AddWithValue("@hash", hash);
                }

                cmd.Parameters.AddWithValue("@accountID", selected.AccountID);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        string what = (updateName && updatePassword) ? "Account name and password" :
                                      updateName ? "Account name" : "Password";
                        MessageBox.Show($"{what} updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (updateName)
                        {
                            selected.AccountName = newAccountName;
                            int idx = cmbAccountID.SelectedIndex;
                            cmbAccountID.Items[idx] = selected;
                            cmbAccountID.SelectedIndex = idx;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to update account.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating account: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            txtNewPassword.Text = "";
       }

        private void label6_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
    }
}