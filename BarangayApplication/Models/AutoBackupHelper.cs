using System;
using System.Data.SqlClient;
using System.IO;

namespace BarangayApplication.Helpers
{
    public static class AutoBackupHelper
    {
        private const string ChangeCountFile = "change_count.txt";
        private const string BackupLocationFile = "backup_location.txt";
        private static readonly string[] DatabaseNames = { "ResidentsDB", "ResidentsArchiveDB", "ResidentsLogDB" };
        private static readonly string[] DatabaseDisplayNames = { "Main", "Archive", "Logbook" };
        private const string ServerName = @".";
        private const string LastBackupFilePrefix = "last_backup_";

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
            foreach (var dbName in DatabaseNames)
            {
                string lastBackupDate = null;
                string lastBackupFile = $"{LastBackupFilePrefix}{dbName}.txt";
                if (File.Exists(lastBackupFile))
                    lastBackupDate = File.ReadAllText(lastBackupFile);

                string today = DateTime.Now.ToString("yyyy-MM-dd");
                if (lastBackupDate == null || !lastBackupDate.StartsWith(today))
                    needBackup = true;
            }

            if (isSuperAdmin)
                needBackup = true;

            if (needBackup)
            {
                PerformAutoBackup("Auto-backup: First login of new day or superadmin login");
                File.WriteAllText(ChangeCountFile, "0");
            }
        }

        // No max backup limit -- simply backup as requested
        private static void PerformAutoBackup(string reason)
        {
            string backupDirectory = "";
            if (File.Exists(BackupLocationFile))
                backupDirectory = File.ReadAllText(BackupLocationFile).Trim();

            if (string.IsNullOrEmpty(backupDirectory) || !Directory.Exists(backupDirectory))
                return; // No valid backup location

            for (int i = 0; i < DatabaseNames.Length; i++)
            {
                string dbName = DatabaseNames[i];
                string displayName = DatabaseDisplayNames[i];

                string monthFolder = Path.Combine(backupDirectory, $"{displayName}Backup", DateTime.Now.ToString("MMMM"));
                Directory.CreateDirectory(monthFolder);

                string bakFile = Path.Combine(monthFolder, $"AutoBackup_{displayName}_{DateTime.Now:ddMMyyyy_HHmmss}.bak");
                string query = $"BACKUP DATABASE [{dbName}] TO DISK = '{bakFile}'";

                string connString = $"Data Source={ServerName};Initial Catalog={dbName};Integrated Security=True";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            SaveLastBackup(dbName, DateTime.Now);
                            File.AppendAllText("autobackup_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {reason} ({displayName})\n");
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("autobackup_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Auto-backup failed for {displayName}: {ex.Message}\n");
                        }
                    }
                }
            }
        }

        private static void SaveLastBackup(string dbName, DateTime dateTime)
        {
            File.WriteAllText($"{LastBackupFilePrefix}{dbName}.txt", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}