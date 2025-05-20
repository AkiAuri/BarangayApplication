using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BarangayApplication
{
    public partial class Settings : Form
    {
        private const string ChangeCountFile = "change_count.txt";
        private const string LastBackupFile = "last_backup.txt";
        private const string BackupLocationFile = "backup_location.txt";
        private const string DatabaseName = "sybau_database";
// You may want to pull this from a central config
        private const string ServerName = @"localhost,1433";
        private const string ConnectionString = "Data Source=" + ServerName + ";Initial Catalog=master;Integrated Security=True";

        public Settings()
        {
            InitializeComponent();
            LoadBackupLocation();
            LoadLastBackup();
        }

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

        //Unneeded
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