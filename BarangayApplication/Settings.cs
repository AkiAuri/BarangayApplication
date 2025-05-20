using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BCrypt.Net;

namespace BarangayApplication
{
    public partial class Settings : Form
    {
        private const string ChangeCountFile = "change_count.txt";
        private const string LastBackupFile = "last_backup.txt";
        private const string BackupLocationFile = "backup_location.txt";
        private const string DatabaseName = "sybau_database";
        private const string ServerName = @"localhost,1433";
        private const string ConnectionString = "Data Source=" + ServerName + ";Initial Catalog="+ DatabaseName + ";Integrated Security=True";

        public Settings()
        {
            InitializeComponent();
            LoadBackupLocation();
            LoadLastBackup();
            PopulateAdminAccountIDs();
        }
        
        // Backup shit
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

        private void LoadLastBackup()
        {
            if (File.Exists(LastBackupFile))
            {
                lblDateTime.Text = File.ReadAllText(LastBackupFile);
            }
            else
            {
                lblDateTime.Text = "No backup yet.";
            }
        }

        private void SaveLastBackup(DateTime dateTime)
        {
            string text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            File.WriteAllText(LastBackupFile, text);
            lblDateTime.Text = text;
        }

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

        private void btnBackup_Click(object sender, EventArgs e)
        {
            string backupDirectory = BackupLoc.Text.Trim();
            if (string.IsNullOrEmpty(backupDirectory) || !Directory.Exists(backupDirectory))
            {
                MessageBox.Show("Please select a valid backup location first.");
                return;
            }

            string backupFile = Path.Combine(backupDirectory, $"{DatabaseName}_{DateTime.Now:yyyyMMddHHmmss}.bak");
            string query = $"BACKUP DATABASE [{DatabaseName}] TO DISK = '{backupFile}'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Backup successful!\nSaved to: {backupFile}");
                        SaveLastBackup(DateTime.Now); // Save and update the label
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Backup failed: " + ex.Message);
                    }
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Backup Files (*.bak)|*.bak";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string backupFile = openFileDialog.FileName;

            // Set single user mode to avoid open connections
            string setSingleUser = $"ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            string restoreQuery = $"RESTORE DATABASE [{DatabaseName}] FROM DISK = '{backupFile}' WITH REPLACE";
            string setMultiUser = $"ALTER DATABASE [{DatabaseName}] SET MULTI_USER";

            string restoreConnectionString = "Data Source=" + ServerName + ";Initial Catalog=master;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(restoreConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    try
                    {
                        conn.Open();
                        cmd.CommandText = setSingleUser;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = restoreQuery;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = setMultiUser;
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Restore successful!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Restore failed: " + ex.Message);
                    }
                }
            }
        }
        
        //Account shit.
        
        // Define a class for display/value pairing
        public class AccountItem
        {
            public int AccountID { get; set; }
            public string AccountName { get; set; }
            public override string ToString() => AccountName; // This will be shown in the ComboBox
        }
        
        private void PopulateAdminAccountIDs()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT accountID, accountName FROM users WHERE roleID = 2";
                using (var cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            cmbAccountID.Items.Clear();
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
                        MessageBox.Show("Error loading accounts: " + ex.Message);
                    }
                }
            }
        }

        private void cmbAccountID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = cmbAccountID.SelectedItem as AccountItem;
            if (selected == null) return;

            txtAccountName.Text = selected.AccountName ?? "";

            // Always clear the password box on change
            txtNewPassword.Text = "";
        }
        
       private void btnSetPassword_Click(object sender, EventArgs e)
        {
            var selected = cmbAccountID.SelectedItem as AccountItem;
            if (selected == null)
            {
                MessageBox.Show("Please select an account.");
                return;
            }

            string newAccountName = txtAccountName.Text.Trim();
            string newPassword = txtNewPassword.Text;

            // Build the update SQL dynamically
            bool updateName = !string.IsNullOrEmpty(newAccountName) && newAccountName != selected.AccountName;
            bool updatePassword = !string.IsNullOrEmpty(newPassword);

            if (!updateName && !updatePassword)
            {
                MessageBox.Show("No changes to update.");
                return;
            }

            string setClause = "";
            if (updateName) setClause += "accountName = @name";
            if (updatePassword)
            {
                if (setClause.Length > 0) setClause += ", ";
                setClause += "passwordHash = @hash";
            }

            string query = $"UPDATE users SET {setClause} WHERE accountID = @accountID";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                if (updateName)
                    cmd.Parameters.AddWithValue("@name", newAccountName);

                if (updatePassword)
                {
                    // Hash password using bcrypt with work factor 16
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
                        MessageBox.Show($"{what} updated successfully!");
                        // Optionally update the selected AccountItem's name in the ComboBox
                        if (updateName) selected.AccountName = newAccountName;
                    }
                    else
                    {
                        MessageBox.Show("Failed to update account.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            txtNewPassword.Text = "";
        }

        //Unneeded stuff
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}