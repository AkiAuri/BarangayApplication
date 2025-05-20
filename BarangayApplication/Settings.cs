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

            using (SqlConnection conn = new SqlConnection(ConnectionString))
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
        //Account functions
        private void PopulateAdminAccountIDs()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT accountID FROM users WHERE roleID = 2";
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
                                cmbAccountID.Items.Add(reader["accountID"].ToString());
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
            string accountID = cmbAccountID.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(accountID)) return;
            using (var conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT accountName FROM users WHERE accountID = @accountID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@accountID", accountID);
                    try
                    {
                        conn.Open();
                        var accountName = cmd.ExecuteScalar()?.ToString();
                        if (txtAccountName != null)
                            txtAccountName.Text = accountName ?? "";
                    }
                    catch
                    {
                        if (txtAccountName != null)
                            txtAccountName.Text = "";
                    }
                }
            }
            // Always clear the password box when user changes selection!
            txtNewPassword.Text = "";
        }
        // THIS IS THE UPDATED FUNCTION FOR 16x HASHING
        private void btnSetPassword_Click(object sender, EventArgs e)
        {
            string accountID = cmbAccountID.SelectedItem?.ToString();
            string newPassword = txtNewPassword.Text;
            if (string.IsNullOrEmpty(accountID) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Please select an account and enter a new password.");
                return;
            }
            // Hash password 16 times using bcrypt
            string hash = newPassword;
            for (int i = 0; i < 16; i++)
            {
                hash = BCrypt.Net.BCrypt.HashPassword(hash);
            }
            using (var conn = new SqlConnection(ConnectionString))
            {
                string query = "UPDATE users SET passwordHash = @hash WHERE accountID = @accountID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@accountID", accountID);
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                            MessageBox.Show("Password updated successfully!");
                        else
                            MessageBox.Show("Failed to update password.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
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