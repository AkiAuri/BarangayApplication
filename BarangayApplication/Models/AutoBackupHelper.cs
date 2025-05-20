using System;
using System.Data.SqlClient;
using System.IO;

namespace BarangayApplication.Helpers
{
    public static class AutoBackupHelper
    {
        private const string ChangeCountFile = "change_count.txt";
        private const string LastBackupFile = "last_backup.txt";
        private const string BackupLocationFile = "backup_location.txt";
        private const string DatabaseName = "sybau_database";
        private const string ServerName = @"localhost,1433";
        private const string ConnectionString = "Data Source=" + ServerName + ";Initial Catalog="+ DatabaseName +";Integrated Security=True";

        // Call this after every data change (create, update, delete)
        public static void IncrementChangeCountAndAutoBackup()
        {
            int count = 0;
            if (File.Exists(ChangeCountFile))
                int.TryParse(File.ReadAllText(ChangeCountFile), out count);

            count++;
            File.WriteAllText(ChangeCountFile, count.ToString());

            if (count >= 10)
            {
                PerformAutoBackup("Auto-backup: 10 changes reached");
                File.WriteAllText(ChangeCountFile, "0");
            }
        }

        // Call this at login (with user and role info)
        public static void CheckAutoBackupOnLogin(string currentUser, bool isSuperAdmin)
        {
            bool needBackup = false;
            string lastBackupDate = null;
            if (File.Exists(LastBackupFile))
                lastBackupDate = File.ReadAllText(LastBackupFile);

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (lastBackupDate == null || !lastBackupDate.StartsWith(today))
                needBackup = true;

            if (isSuperAdmin)
                needBackup = true;

            if (needBackup)
            {
                PerformAutoBackup("Auto-backup: First login of new day or superadmin login");
                File.WriteAllText(ChangeCountFile, "0");
            }
        }

        private static void PerformAutoBackup(string reason)
        {
            string backupDirectory = "";
            if (File.Exists(BackupLocationFile))
                backupDirectory = File.ReadAllText(BackupLocationFile).Trim();

            if (string.IsNullOrEmpty(backupDirectory) || !Directory.Exists(backupDirectory))
                return; // No valid backup location

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
                        SaveLastBackup(DateTime.Now);
                        // Optionally: log reason
                        File.AppendAllText("autobackup_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {reason}\n");
                    }
                    catch (Exception ex)
                    {
                        // Optionally: log error
                        File.AppendAllText("autobackup_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Auto-backup failed: {ex.Message}\n");
                    }
                }
            }
        }

        private static void SaveLastBackup(DateTime dateTime)
        {
            File.WriteAllText(LastBackupFile, dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}