using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BarangayApplication.Helpers;
using BarangayApplication.Models;
using static BarangayApplication.LoginMenu;
namespace BarangayApplication.Models.Repositories
{
    public class ResidentsRepository
    {
        private readonly string _mainConn = "Data Source=localhost,1433;Initial Catalog=ResidentsDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private readonly string _archiveConn = "Data Source=localhost,1433;Initial Catalog=ResidentsArchiveDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private readonly string _logConn = "Data Source=localhost,1433;Initial Catalog=ResidentsLogDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        /// <summary>
        /// Gets the distribution of purposes for all resident purposes.
        /// </summary>
        public Dictionary<string, int> GetPurposeDistribution()
        {
            var purposeDistribution = new Dictionary<string, int>
            {
                { "Residency", 0 },
                { "Postal ID", 0 },
                { "Local Employment", 0 },
                { "Marriage", 0 },
                { "Loan", 0 },
                { "Meralco", 0 },
                { "Bank Transaction", 0 },
                { "Travel Abroad", 0 },
                { "Senior Citizen", 0 },
                { "School", 0 },
                { "Medical", 0 },
                { "Burial", 0 },
                { "Others", 0 }
            };

            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();
                    // Query uses TransactionID as PK
                    string sql = @"
                        SELECT 
                            pt.PurposeName, COUNT(rp.TransactionID) AS Count
                        FROM ResidentPurposes rp
                        INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                        GROUP BY pt.PurposeName
                    ";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            string purpose = _reader.IsDBNull(0) ? "" : _reader.GetString(0);
                            int count = _reader.IsDBNull(1) ? 0 : _reader.GetInt32(1);
                            // Normalize to match dictionary keys
                            switch (purpose)
                            {
                                case "Residency": purposeDistribution["Residency"] = count; break;
                                case "PostalID": purposeDistribution["Postal ID"] = count; break;
                                case "LocalEmployment": purposeDistribution["Local Employment"] = count; break;
                                case "Marriage": purposeDistribution["Marriage"] = count; break;
                                case "Loan": purposeDistribution["Loan"] = count; break;
                                case "Meralco": purposeDistribution["Meralco"] = count; break;
                                case "BankTransaction": purposeDistribution["Bank Transaction"] = count; break;
                                case "TravelAbroad": purposeDistribution["Travel Abroad"] = count; break;
                                case "SeniorCitizen": purposeDistribution["Senior Citizen"] = count; break;
                                case "School": purposeDistribution["School"] = count; break;
                                case "Medical": purposeDistribution["Medical"] = count; break;
                                case "Burial": purposeDistribution["Burial"] = count; break;
                                case "Others":
                                    // Will be overwritten by the next query for "Others"
                                    break;
                                default: break;
                            }
                        }
                    }

                    // For "Others", count only those with non-null/non-empty PurposeOthers
                    string othersSql = @"
                        SELECT COUNT(*) 
                        FROM ResidentPurposes rp
                        INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                        WHERE pt.PurposeName = 'Others' AND rp.PurposeOthers IS NOT NULL AND LTRIM(RTRIM(rp.PurposeOthers)) <> ''
                    ";
                    using (SqlCommand _cmd = new SqlCommand(othersSql, _conn))
                    {
                        int othersCount = (int)_cmd.ExecuteScalar();
                        purposeDistribution["Others"] = othersCount;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return purposeDistribution;
        }

        public Dictionary<string, int> GetGenderDistribution()
        {
            var genderDistribution = new Dictionary<string, int>
            {
                { "Male", 0 },
                { "Female", 0 },
                { "Other", 0 }
            };

            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();
                    // Join with Sexes for normalized lookup value
                    string sql = @"
                SELECT s.SexDescription, COUNT(*) AS Count
                FROM Residents r
                INNER JOIN Sexes s ON r.SexID = s.SexID
                GROUP BY s.SexDescription
            ";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            string sex = _reader.IsDBNull(0) ? "" : _reader.GetString(0).Trim();
                            int count = _reader.IsDBNull(1) ? 0 : _reader.GetInt32(1);

                            // Normalize SexDescription to dictionary key
                            switch (sex.ToLower())
                            {
                                case "male":
                                    genderDistribution["Male"] += count;
                                    break;
                                case "female":
                                    genderDistribution["Female"] += count;
                                    break;
                                default:
                                    genderDistribution["Other"] += count;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return genderDistribution;
        }

        public Dictionary<string, int> GetAgeGroupCounts()
        {
            var ageGroups = new Dictionary<string, int>
            {
                { "Youth (18-30)", 0 },
                { "Adults (31-59)", 0 },
                { "Seniors (60+)", 0 }
            };

            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();
                    string sql = "SELECT DateOfBirth FROM Residents";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            if (_reader.IsDBNull(0)) continue;
                            DateTime dob = _reader.GetDateTime(0);

                            int age = DateTime.Now.Year - dob.Year;
                            if (DateTime.Now < dob.AddYears(age)) age--;

                            if (age >= 18 && age <= 30)
                                ageGroups["Youth (18-30)"]++;
                            else if (age >= 31 && age <= 59)
                                ageGroups["Adults (31-59)"]++;
                            else if (age >= 60)
                                ageGroups["Seniors (60+)"]++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return ageGroups;
        }

        public string GetLoggedInUserName(int currentAccountId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql = "SELECT accountName FROM users WHERE accountID = @CurrentAccountId";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@CurrentAccountId", currentAccountId);

                        object result = _cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "Unknown User";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while fetching logged-in user: " + ex.ToString());
                throw;
            }
        }
        //for filtering logs by action
        public DataTable GetFilteredLogs(string action)
        {
            var dataTable = new DataTable();
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql;
                    // Always compare in uppercase for consistency
                    string upperAction = action.ToUpper();

                    if (upperAction == "ARCHIVE" || upperAction == "ARCHIVED")
                    {
                        sql = @"
                            SELECT 
                                FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                u.accountName AS [USER], 
                                a.ActionName AS [ACTION], 
                                ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) LIKE 'ARCHIVE%'
                            ORDER BY ul.Timestamp DESC";
                    }
                    else if (upperAction == "RESTORE" || upperAction == "RESTORED")
                    {
                        sql = @"
                            SELECT 
                                FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                u.accountName AS [USER], 
                                a.ActionName AS [ACTION], 
                                ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) LIKE 'RESTORE%'
                            ORDER BY ul.Timestamp DESC";
                    }
                    else
                    {
                        sql = @"
                            SELECT 
                                FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                u.accountName AS [USER], 
                                a.ActionName AS [ACTION], 
                                ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) = @Action
                            ORDER BY ul.Timestamp DESC";
                    }

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        if (!(upperAction == "ARCHIVE" || upperAction == "ARCHIVED" || upperAction == "RESTORE" || upperAction == "RESTORED"))
                            _cmd.Parameters.AddWithValue("@Action", upperAction);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(_cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return dataTable;
        }
        
        public DataTable GetLogsByUser(string userName)
        {
            var dataTable = new DataTable();
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql = @"
                SELECT
                    FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                    u.accountName AS [USER], 
                    a.ActionName AS [ACTION], 
                    ul.Description AS [DESCRIPTION]
                FROM UserLogs ul
                INNER JOIN users u ON ul.AccountID = u.accountID
                INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                WHERE u.accountName = @UserName
                ORDER BY ul.Timestamp DESC";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@UserName", userName);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(_cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return dataTable;
        }
        public DataTable GetLogsByUserAndAction(string userName, string action)
        {
            var dataTable = new DataTable();
            try
            {
                using (var conn = new SqlConnection(_logConn))
                {
                    conn.Open();
                    string sql;
                    string upperAction = action.ToUpper();

                    if ((upperAction == "ARCHIVE" || upperAction == "ARCHIVED") && !string.IsNullOrEmpty(userName))
                    {
                        sql = @"
                            SELECT FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                   u.accountName AS [USER],
                                   a.ActionName AS [ACTION],
                                   ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) LIKE 'ARCHIVE%' AND u.accountName = @UserName
                            ORDER BY ul.Timestamp DESC";
                    }
                    else if ((upperAction == "RESTORE" || upperAction == "RESTORED") && !string.IsNullOrEmpty(userName))
                    {
                        sql = @"
                            SELECT FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                   u.accountName AS [USER],
                                   a.ActionName AS [ACTION],
                                   ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) LIKE 'RESTORE%' AND u.accountName = @UserName
                            ORDER BY ul.Timestamp DESC";
                    }
                    else
                    {
                        sql = @"
                            SELECT FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                                   u.accountName AS [USER],
                                   a.ActionName AS [ACTION],
                                   ul.Description AS [DESCRIPTION]
                            FROM UserLogs ul
                            INNER JOIN users u ON ul.AccountID = u.accountID
                            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                            WHERE UPPER(a.ActionName) = @Action AND u.accountName = @UserName
                            ORDER BY ul.Timestamp DESC";
                    }

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        if (!(upperAction == "ARCHIVE" || upperAction == "ARCHIVED" || upperAction == "RESTORE" || upperAction == "RESTORED"))
                            cmd.Parameters.AddWithValue("@Action", upperAction);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return dataTable;
        }

        public void AddUserLog(int accountId, string action, string description)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();

                    // 1. Get ActionID for the action name (case-insensitive)
                    string getActionIdSql = "SELECT ActionID FROM UserActions WHERE LOWER(ActionName) = LOWER(@ActionName)";
                    int actionId;
                    using (SqlCommand getActionIdCmd = new SqlCommand(getActionIdSql, _conn))
                    {
                        getActionIdCmd.Parameters.AddWithValue("@ActionName", action.Trim());
                        object actionIdObj = getActionIdCmd.ExecuteScalar();
                        if (actionIdObj == null)
                        {
                            throw new Exception($"Action '{action}' is not recognized in UserActions table.");
                        }
                        actionId = Convert.ToInt32(actionIdObj);
                    }

                    // 2. Normalize description (trim)
                    string normDescription = description?.Trim();

                    // 3. Check for duplicate within the last minute
                    string checkSql = @"
                        SELECT COUNT(*) 
                        FROM UserLogs 
                        WHERE AccountID = @AccountID 
                          AND ActionID = @ActionID 
                          AND Description = @Description
                          AND DATEDIFF(SECOND, Timestamp, @Now) BETWEEN 0 AND 60";

                    using (SqlCommand checkCmd = new SqlCommand(checkSql, _conn))
                    {
                        checkCmd.Parameters.AddWithValue("@AccountID", accountId);
                        checkCmd.Parameters.AddWithValue("@ActionID", actionId);
                        checkCmd.Parameters.AddWithValue("@Description", normDescription);
                        checkCmd.Parameters.AddWithValue("@Now", DateTime.Now);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            // Log entry already exists, skip adding
                            return;
                        }
                    }

                    // 4. Insert the new log entry
                    string insertSql = @"
                        INSERT INTO UserLogs (AccountID, ActionID, Description, Timestamp)
                        VALUES (@AccountID, @ActionID, @Description, @Timestamp)";

                    using (SqlCommand insertCmd = new SqlCommand(insertSql, _conn))
                    {
                        insertCmd.Parameters.AddWithValue("@AccountID", accountId);
                        insertCmd.Parameters.AddWithValue("@ActionID", actionId);
                        insertCmd.Parameters.AddWithValue("@Description", normDescription);
                        insertCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        public DataTable GetLogsForPage(int offset, int rowsPerPage)
        {
            var dataTable = new DataTable();
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql = @"
                SELECT
                    FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                    u.accountName AS [USER],
                    a.ActionName AS [ACTION],
                    ul.Description AS [DESCRIPTION]
                FROM UserLogs ul
                INNER JOIN users u ON ul.AccountID = u.accountID
                INNER JOIN UserActions a ON ul.ActionID = a.ActionID
                ORDER BY ul.Timestamp DESC
                OFFSET @Offset ROWS FETCH NEXT @RowsPerPage ROWS ONLY";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@Offset", offset);
                        _cmd.Parameters.AddWithValue("@RowsPerPage", rowsPerPage);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(_cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return dataTable;
        }

        public int GetTotalLogCount() //For counting logs <3
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql = "SELECT COUNT(*) FROM UserLogs";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        return (int)_cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        public List<Tuple<int, int>> GetAvailableLogMonths()
        {
            var result = new List<Tuple<int, int>>();
            using (var conn = new SqlConnection(_logConn))
            {
                conn.Open();
                string sql = @"SELECT DISTINCT YEAR(Timestamp) AS Y, MONTH(Timestamp) AS M FROM UserLogs ORDER BY Y, M";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int year = reader.GetInt32(0);
                        int month = reader.GetInt32(1);
                        result.Add(new Tuple<int, int>(year, month));
                    }
                }
            }
            return result;
        }
        
        public DataTable GetLogsForMonth(int year, int month)
        {
            var table = new DataTable();
            using (var conn = new SqlConnection(_logConn))
            {
                conn.Open();
                string sql = @"
            SELECT FORMAT(ul.Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                   u.accountName AS [USER], 
                   a.ActionName AS [ACTION], 
                   ul.Description AS [DESCRIPTION]
            FROM UserLogs ul
            INNER JOIN users u ON ul.AccountID = u.accountID
            INNER JOIN UserActions a ON ul.ActionID = a.ActionID
            WHERE YEAR(ul.Timestamp) = @Year AND MONTH(ul.Timestamp) = @Month
            ORDER BY ul.Timestamp ASC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", month);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }
        // end of logbook codes
        
        //small overview code
        
        public int GetMonthlyResidentAddCount()
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_logConn))
                {
                    _conn.Open();
                    string sql = @"
                SELECT COUNT(*)
                FROM UserLogs ul
                INNER JOIN UserActions ua ON ul.ActionID = ua.ActionID
                WHERE UPPER(ua.ActionName) = 'ADD'
                  AND MONTH(ul.Timestamp) = @Month
                  AND YEAR(ul.Timestamp) = @Year";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@Month", DateTime.Now.Month);
                        _cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);

                        return (int)_cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        //not really cause i am tired, it's 3:38am...

        public List<Resident> GetApplicants()
        {
            var residents = new List<Resident>();

            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();

                    // Query main resident info with joined lookup tables for descriptions and include IDs for navigation properties
                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.SexID,
                            s.SexDescription AS Sex,
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.PlaceOfBirth, 
                            r.CivilStatusID,
                            cs.CivilStatusDescription AS CivilStatus,
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceTypeID,
                            rt.ResidenceTypeName AS ResidenceType
                        FROM Residents r
                        INNER JOIN Sexes s ON r.SexID = s.SexID
                        INNER JOIN CivilStatuses cs ON r.CivilStatusID = cs.CivilStatusID
                        INNER JOIN ResidenceTypes rt ON r.ResidenceTypeID = rt.ResidenceTypeID
                        ORDER BY r.ResidentID DESC;
                    ";
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            var resident = new Resident
                            {
                                ResidentID = _reader.GetInt32(_reader.GetOrdinal("ResidentID")),
                                LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? "" : _reader.GetString(_reader.GetOrdinal("LastName")),
                                FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? "" : _reader.GetString(_reader.GetOrdinal("FirstName")),
                                MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName")),
                                Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? "" : _reader.GetString(_reader.GetOrdinal("Address")),
                                TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? "" : _reader.GetString(_reader.GetOrdinal("TelCelNo")),
                                SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                Sex = new Sex
                                {
                                    SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                    SexDescription = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex"))
                                },
                                Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                CivilStatus = new CivilStatus
                                {
                                    CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                    CivilStatusDescription = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus"))
                                },
                                VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                ResidenceType = new ResidenceType
                                {
                                    ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                    ResidenceTypeName = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType"))
                                }
                            };

                            residents.Add(resident);
                        }
                    }

                    var residentIds = residents.Select(r => r.ResidentID).ToList();

                    // Employment
                    var employmentDict = new Dictionary<int, List<Employment>>();
                    if (residentIds.Count > 0)
                    {
                        string empSql = $"SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID IN ({string.Join(",", residentIds)})";
                        using (SqlCommand empCmd = new SqlCommand(empSql, _conn))
                        using (SqlDataReader empReader = empCmd.ExecuteReader())
                        {
                            while (empReader.Read())
                            {
                                int resId = empReader.GetInt32(empReader.GetOrdinal("ResidentID"));
                                var emp = new Employment
                                {
                                    EmploymentID = empReader.GetInt32(empReader.GetOrdinal("EmploymentID")),
                                    ResidentID = resId,
                                    Company = empReader.IsDBNull(empReader.GetOrdinal("Company")) ? "" : empReader.GetString(empReader.GetOrdinal("Company")),
                                    Position = empReader.IsDBNull(empReader.GetOrdinal("Position")) ? "" : empReader.GetString(empReader.GetOrdinal("Position")),
                                    LengthOfService = empReader.IsDBNull(empReader.GetOrdinal("LengthOfService")) ? "" : empReader.GetString(empReader.GetOrdinal("LengthOfService"))
                                };
                                if (!employmentDict.ContainsKey(resId))
                                    employmentDict[resId] = new List<Employment>();
                                employmentDict[resId].Add(emp);
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (employmentDict.ContainsKey(r.ResidentID))
                            r.Employments = employmentDict[r.ResidentID];

                    // Spouse and SpouseEmployment
                    var spouseDict = new Dictionary<int, Spouse>();
                    if (residentIds.Count > 0)
                    {
                        string spouseSql = $"SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID IN ({string.Join(",", residentIds)})";
                        using (SqlCommand spouseCmd = new SqlCommand(spouseSql, _conn))
                        using (SqlDataReader spouseReader = spouseCmd.ExecuteReader())
                        {
                            while (spouseReader.Read())
                            {
                                int resId = spouseReader.GetInt32(spouseReader.GetOrdinal("ResidentID"));
                                var sp = new Spouse
                                {
                                    SpouseID = spouseReader.GetInt32(spouseReader.GetOrdinal("SpouseID")),
                                    ResidentID = resId,
                                    SpouseName = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpouseName")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpouseName")),
                                };
                                spouseDict[resId] = sp;
                            }
                        }

                        // SpouseEmployment
                        if (spouseDict.Values.Any())
                        {
                            string spouseIds = string.Join(",", spouseDict.Values.Select(sp => sp.SpouseID));
                            string spouseEmpSql = $"SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID IN ({spouseIds})";
                            using (SqlCommand spouseEmpCmd = new SqlCommand(spouseEmpSql, _conn))
                            using (SqlDataReader spouseEmpReader = spouseEmpCmd.ExecuteReader())
                            {
                                var spouseEmpDict = new Dictionary<int, List<SpouseEmployment>>();
                                while (spouseEmpReader.Read())
                                {
                                    int spouseId = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseID"));
                                    var spEmp = new SpouseEmployment
                                    {
                                        SpouseEmploymentID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseEmploymentID")),
                                        SpouseID = spouseId,
                                        Company = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Company")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Company")),
                                        Position = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Position")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Position")),
                                        LengthOfService = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("LengthOfService")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("LengthOfService"))
                                    };
                                    if (!spouseEmpDict.ContainsKey(spouseId))
                                        spouseEmpDict[spouseId] = new List<SpouseEmployment>();
                                    spouseEmpDict[spouseId].Add(spEmp);
                                }
                                foreach (var sp in spouseDict.Values)
                                    if (spouseEmpDict.ContainsKey(sp.SpouseID))
                                        sp.Employments = spouseEmpDict[sp.SpouseID];
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (spouseDict.ContainsKey(r.ResidentID))
                            r.Spouse = spouseDict[r.ResidentID];

                    // Purposes (ResidentPurposes, PurposeTypes)
                    var purposesDict = new Dictionary<int, List<ResidentPurpose>>();
                    if (residentIds.Count > 0)
                    {
                        string purposesSql = $@"
                            SELECT rp.TransactionID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName 
                            FROM ResidentPurposes rp
                            INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                            WHERE rp.ResidentID IN ({string.Join(",", residentIds)})
                        ";
                        using (SqlCommand purposesCmd = new SqlCommand(purposesSql, _conn))
                        using (SqlDataReader purposesReader = purposesCmd.ExecuteReader())
                        {
                            while (purposesReader.Read())
                            {
                                int resId = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentID"));
                                var purpose = new ResidentPurpose
                                {
                                    TransactionID = purposesReader.GetString(purposesReader.GetOrdinal("TransactionID")),
                                    ResidentID = resId,
                                    PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                    PurposeOthers = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeOthers")) ? null : purposesReader.GetString(purposesReader.GetOrdinal("PurposeOthers")),
                                    PurposeType = new PurposeType
                                    {
                                        PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                        PurposeName = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeName")) ? "" : purposesReader.GetString(purposesReader.GetOrdinal("PurposeName"))
                                    }
                                };
                                if (!purposesDict.ContainsKey(resId))
                                    purposesDict[resId] = new List<ResidentPurpose>();
                                purposesDict[resId].Add(purpose);
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (purposesDict.ContainsKey(r.ResidentID))
                            r.Purposes = purposesDict[r.ResidentID];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return residents;
        }
        
        public List<Resident> GetArchivedApplicants()
        {
            var residents = new List<Resident>();

            try
            {
                using (SqlConnection _conn = new SqlConnection(_archiveConn))
                {
                    _conn.Open();

                    // Query main resident info with joined lookup tables for descriptions and include IDs for navigation properties
                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.SexID,
                            s.SexDescription AS Sex,
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.PlaceOfBirth, 
                            r.CivilStatusID,
                            cs.CivilStatusDescription AS CivilStatus,
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceTypeID,
                            rt.ResidenceTypeName AS ResidenceType
                        FROM Residents r
                        INNER JOIN Sexes s ON r.SexID = s.SexID
                        INNER JOIN CivilStatuses cs ON r.CivilStatusID = cs.CivilStatusID
                        INNER JOIN ResidenceTypes rt ON r.ResidenceTypeID = rt.ResidenceTypeID
                        ORDER BY r.ResidentID DESC;
                    ";
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            var resident = new Resident
                            {
                                ResidentID = _reader.GetInt32(_reader.GetOrdinal("ResidentID")),
                                LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? "" : _reader.GetString(_reader.GetOrdinal("LastName")),
                                FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? "" : _reader.GetString(_reader.GetOrdinal("FirstName")),
                                MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName")),
                                Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? "" : _reader.GetString(_reader.GetOrdinal("Address")),
                                TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? "" : _reader.GetString(_reader.GetOrdinal("TelCelNo")),
                                SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                Sex = new Sex
                                {
                                    SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                    SexDescription = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex"))
                                },
                                Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                CivilStatus = new CivilStatus
                                {
                                    CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                    CivilStatusDescription = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus"))
                                },
                                VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                ResidenceType = new ResidenceType
                                {
                                    ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                    ResidenceTypeName = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType"))
                                }
                            };

                            residents.Add(resident);
                        }
                    }

                    var residentIds = residents.Select(r => r.ResidentID).ToList();

                    // Employment
                    var employmentDict = new Dictionary<int, List<Employment>>();
                    if (residentIds.Count > 0)
                    {
                        string empSql = $"SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID IN ({string.Join(",", residentIds)})";
                        using (SqlCommand empCmd = new SqlCommand(empSql, _conn))
                        using (SqlDataReader empReader = empCmd.ExecuteReader())
                        {
                            while (empReader.Read())
                            {
                                int resId = empReader.GetInt32(empReader.GetOrdinal("ResidentID"));
                                var emp = new Employment
                                {
                                    EmploymentID = empReader.GetInt32(empReader.GetOrdinal("EmploymentID")),
                                    ResidentID = resId,
                                    Company = empReader.IsDBNull(empReader.GetOrdinal("Company")) ? "" : empReader.GetString(empReader.GetOrdinal("Company")),
                                    Position = empReader.IsDBNull(empReader.GetOrdinal("Position")) ? "" : empReader.GetString(empReader.GetOrdinal("Position")),
                                    LengthOfService = empReader.IsDBNull(empReader.GetOrdinal("LengthOfService")) ? "" : empReader.GetString(empReader.GetOrdinal("LengthOfService"))
                                };
                                if (!employmentDict.ContainsKey(resId))
                                    employmentDict[resId] = new List<Employment>();
                                employmentDict[resId].Add(emp);
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (employmentDict.ContainsKey(r.ResidentID))
                            r.Employments = employmentDict[r.ResidentID];

                    // Spouse and SpouseEmployment
                    var spouseDict = new Dictionary<int, Spouse>();
                    if (residentIds.Count > 0)
                    {
                        string spouseSql = $"SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID IN ({string.Join(",", residentIds)})";
                        using (SqlCommand spouseCmd = new SqlCommand(spouseSql, _conn))
                        using (SqlDataReader spouseReader = spouseCmd.ExecuteReader())
                        {
                            while (spouseReader.Read())
                            {
                                int resId = spouseReader.GetInt32(spouseReader.GetOrdinal("ResidentID"));
                                var sp = new Spouse
                                {
                                    SpouseID = spouseReader.GetInt32(spouseReader.GetOrdinal("SpouseID")),
                                    ResidentID = resId,
                                    SpouseName = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpouseName")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpouseName")),
                                };
                                spouseDict[resId] = sp;
                            }
                        }

                        // SpouseEmployment
                        if (spouseDict.Values.Any())
                        {
                            string spouseIds = string.Join(",", spouseDict.Values.Select(sp => sp.SpouseID));
                            string spouseEmpSql = $"SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID IN ({spouseIds})";
                            using (SqlCommand spouseEmpCmd = new SqlCommand(spouseEmpSql, _conn))
                            using (SqlDataReader spouseEmpReader = spouseEmpCmd.ExecuteReader())
                            {
                                var spouseEmpDict = new Dictionary<int, List<SpouseEmployment>>();
                                while (spouseEmpReader.Read())
                                {
                                    int spouseId = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseID"));
                                    var spEmp = new SpouseEmployment
                                    {
                                        SpouseEmploymentID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseEmploymentID")),
                                        SpouseID = spouseId,
                                        Company = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Company")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Company")),
                                        Position = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Position")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Position")),
                                        LengthOfService = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("LengthOfService")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("LengthOfService"))
                                    };
                                    if (!spouseEmpDict.ContainsKey(spouseId))
                                        spouseEmpDict[spouseId] = new List<SpouseEmployment>();
                                    spouseEmpDict[spouseId].Add(spEmp);
                                }
                                foreach (var sp in spouseDict.Values)
                                    if (spouseEmpDict.ContainsKey(sp.SpouseID))
                                        sp.Employments = spouseEmpDict[sp.SpouseID];
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (spouseDict.ContainsKey(r.ResidentID))
                            r.Spouse = spouseDict[r.ResidentID];

                    // Purposes (ResidentPurposes, PurposeTypes)
                    var purposesDict = new Dictionary<int, List<ResidentPurpose>>();
                    if (residentIds.Count > 0)
                    {
                        string purposesSql = $@"
                            SELECT rp.TransactionID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName 
                            FROM ResidentPurposes rp
                            INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                            WHERE rp.ResidentID IN ({string.Join(",", residentIds)})
                        ";
                        using (SqlCommand purposesCmd = new SqlCommand(purposesSql, _conn))
                        using (SqlDataReader purposesReader = purposesCmd.ExecuteReader())
                        {
                            while (purposesReader.Read())
                            {
                                int resId = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentID"));
                                var purpose = new ResidentPurpose
                                {
                                    TransactionID = purposesReader.GetString(purposesReader.GetOrdinal("TransactionID")),
                                    ResidentID = resId,
                                    PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                    PurposeOthers = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeOthers")) ? null : purposesReader.GetString(purposesReader.GetOrdinal("PurposeOthers")),
                                    PurposeType = new PurposeType
                                    {
                                        PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                        PurposeName = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeName")) ? "" : purposesReader.GetString(purposesReader.GetOrdinal("PurposeName"))
                                    }
                                };
                                if (!purposesDict.ContainsKey(resId))
                                    purposesDict[resId] = new List<ResidentPurpose>();
                                purposesDict[resId].Add(purpose);
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (purposesDict.ContainsKey(r.ResidentID))
                            r.Purposes = purposesDict[r.ResidentID];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }

            return residents;
        }
        
        /// <summary>
        /// Retrieves a single applicant resident by the provided residentId.
        /// The method connects to the database, executes a SELECT query against the Residents table 
        /// with a filter on ResidentID, joins Employment, Spouse, and Purposes, maps the result to a Residents instance, and returns it.
        /// If no matching record is found, the method returns null.
        /// In case of an exception, the error is logged to the console and rethrown.
        /// </summary>
        /// <param name="residentId">The ID of the resident to be retrieved.</param>
        /// <returns>A Residents object if found; otherwise, null.</returns>
        public Resident? GetApplicant(int residentId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();

                    // 1. Main Resident info
                    Resident? resident = null;
                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.SexID,
                            s.SexDescription AS Sex,
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.PlaceOfBirth, 
                            r.CivilStatusID,
                            cs.CivilStatusDescription AS CivilStatus,
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceTypeID,
                            rt.ResidenceTypeName AS ResidenceType
                        FROM Residents r
                        INNER JOIN Sexes s ON r.SexID = s.SexID
                        INNER JOIN CivilStatuses cs ON r.CivilStatusID = cs.CivilStatusID
                        INNER JOIN ResidenceTypes rt ON r.ResidenceTypeID = rt.ResidenceTypeID
                        WHERE r.ResidentID = @residentId
                        ORDER BY r.ResidentID DESC;";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@residentId", residentId);

                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            if (_reader.Read())
                            {
                                resident = new Resident
                                {
                                    ResidentID = _reader.GetInt32(_reader.GetOrdinal("ResidentID")),
                                    LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? "" : _reader.GetString(_reader.GetOrdinal("LastName")),
                                    FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? "" : _reader.GetString(_reader.GetOrdinal("FirstName")),
                                    MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName")),
                                    Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? "" : _reader.GetString(_reader.GetOrdinal("Address")),
                                    TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? "" : _reader.GetString(_reader.GetOrdinal("TelCelNo")),
                                    SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                    Sex = new Sex
                                    {
                                        SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                        SexDescription = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex"))
                                    },
                                    Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                    Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                    DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                    PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                    CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                    CivilStatus = new CivilStatus
                                    {
                                        CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                        CivilStatusDescription = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus"))
                                    },
                                    VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                    PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                    ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                    ResidenceType = new ResidenceType
                                    {
                                        ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                        ResidenceTypeName = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType"))
                                    }
                                };
                            }
                        }
                    }

                    if (resident == null)
                        return null;

                    // 2. Related Employment
                    string empSql = "SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID = @residentId";
                    using (SqlCommand empCmd = new SqlCommand(empSql, _conn))
                    {
                        empCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader empReader = empCmd.ExecuteReader())
                        {
                            while (empReader.Read())
                            {
                                var emp = new Employment
                                {
                                    EmploymentID = empReader.GetInt32(empReader.GetOrdinal("EmploymentID")),
                                    ResidentID = empReader.GetInt32(empReader.GetOrdinal("ResidentID")),
                                    Company = empReader.IsDBNull(empReader.GetOrdinal("Company")) ? "" : empReader.GetString(empReader.GetOrdinal("Company")),
                                    Position = empReader.IsDBNull(empReader.GetOrdinal("Position")) ? "" : empReader.GetString(empReader.GetOrdinal("Position")),
                                    LengthOfService = empReader.IsDBNull(empReader.GetOrdinal("LengthOfService")) ? "" : empReader.GetString(empReader.GetOrdinal("LengthOfService"))
                                };
                                resident.Employments.Add(emp);
                            }
                        }
                    }

                    // 3. Spouse (if exists) - read spouse FIRST, then query spouse employments if spouse exists
                    Spouse? spouse = null;
                    int spouseId = 0;
                    string spouseSql = "SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID = @residentId";
                    using (SqlCommand spouseCmd = new SqlCommand(spouseSql, _conn))
                    {
                        spouseCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader spouseReader = spouseCmd.ExecuteReader())
                        {
                            if (spouseReader.Read())
                            {
                                spouseId = spouseReader.GetInt32(spouseReader.GetOrdinal("SpouseID"));
                                spouse = new Spouse
                                {
                                    SpouseID = spouseId,
                                    ResidentID = spouseReader.GetInt32(spouseReader.GetOrdinal("ResidentID")),
                                    SpouseName = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpouseName")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpouseName"))
                                };
                            }
                        }
                    }

                    // 3a. Spouse Employment
                    if (spouse != null)
                    {
                        string spouseEmpSql = "SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID = @spouseId";
                        using (SqlCommand spouseEmpCmd = new SqlCommand(spouseEmpSql, _conn))
                        {
                            spouseEmpCmd.Parameters.AddWithValue("@spouseId", spouseId);
                            using (SqlDataReader spouseEmpReader = spouseEmpCmd.ExecuteReader())
                            {
                                while (spouseEmpReader.Read())
                                {
                                    var spEmp = new SpouseEmployment
                                    {
                                        SpouseEmploymentID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseEmploymentID")),
                                        SpouseID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseID")),
                                        Company = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Company")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Company")),
                                        Position = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Position")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Position")),
                                        LengthOfService = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("LengthOfService")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("LengthOfService"))
                                    };
                                    spouse.Employments.Add(spEmp);
                                }
                            }
                        }
                        resident.Spouse = spouse;
                    }

                    // 4. Purposes (ResidentPurposes, PurposeTypes)
                    string purposesSql = @"
                        SELECT rp.TransactionID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName
                        FROM ResidentPurposes rp
                        INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                        WHERE rp.ResidentID = @residentId";
                    using (SqlCommand purposesCmd = new SqlCommand(purposesSql, _conn))
                    {
                        purposesCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader purposesReader = purposesCmd.ExecuteReader())
                        {
                            while (purposesReader.Read())
                            {
                                var purpose = new ResidentPurpose
                                {
                                    TransactionID = purposesReader.GetString(purposesReader.GetOrdinal("TransactionID")),
                                    ResidentID = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentID")),
                                    PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                    PurposeOthers = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeOthers")) ? null : purposesReader.GetString(purposesReader.GetOrdinal("PurposeOthers")),
                                    PurposeType = new PurposeType
                                    {
                                        PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                        PurposeName = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeName")) ? "" : purposesReader.GetString(purposesReader.GetOrdinal("PurposeName"))
                                    }
                                };
                                resident.Purposes.Add(purpose);
                            }
                        }
                    }

                    return resident;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Retrieves a single archived applicant resident by the provided residentId from the archive database.
        /// The method connects to the archive database, executes a SELECT query against the Residents table 
        /// with a filter on ResidentID, joins Employment, Spouse, and Purposes, maps the result to a Residents instance, and returns it.
        /// If no matching record is found, the method returns null.
        /// In case of an exception, the error is logged to the console and rethrown.
        /// </summary>
        /// <param name="residentId">The ID of the resident to be retrieved.</param>
        /// <returns>A Residents object if found; otherwise, null.</returns>
        public Resident? GetArchivedApplicant(int residentId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_archiveConn))
                {
                    _conn.Open();

                    // 1. Main Resident info
                    Resident? resident = null;
                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.SexID,
                            s.SexDescription AS Sex,
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.PlaceOfBirth, 
                            r.CivilStatusID,
                            cs.CivilStatusDescription AS CivilStatus,
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceTypeID,
                            rt.ResidenceTypeName AS ResidenceType
                        FROM Residents r
                        INNER JOIN Sexes s ON r.SexID = s.SexID
                        INNER JOIN CivilStatuses cs ON r.CivilStatusID = cs.CivilStatusID
                        INNER JOIN ResidenceTypes rt ON r.ResidenceTypeID = rt.ResidenceTypeID
                        WHERE r.ResidentID = @residentId
                        ORDER BY r.ResidentID DESC;";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@residentId", residentId);

                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            if (_reader.Read())
                            {
                                resident = new Resident
                                {
                                    ResidentID = _reader.GetInt32(_reader.GetOrdinal("ResidentID")),
                                    LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? "" : _reader.GetString(_reader.GetOrdinal("LastName")),
                                    FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? "" : _reader.GetString(_reader.GetOrdinal("FirstName")),
                                    MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName")),
                                    Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? "" : _reader.GetString(_reader.GetOrdinal("Address")),
                                    TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? "" : _reader.GetString(_reader.GetOrdinal("TelCelNo")),
                                    SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                    Sex = new Sex
                                    {
                                        SexID = _reader.IsDBNull(_reader.GetOrdinal("SexID")) ? (byte)0 : _reader.GetByte(_reader.GetOrdinal("SexID")),
                                        SexDescription = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex"))
                                    },
                                    Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                    Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                    DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                    PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                    CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                    CivilStatus = new CivilStatus
                                    {
                                        CivilStatusID = _reader.IsDBNull(_reader.GetOrdinal("CivilStatusID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("CivilStatusID")),
                                        CivilStatusDescription = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus"))
                                    },
                                    VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                    PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                    ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                    ResidenceType = new ResidenceType
                                    {
                                        ResidenceTypeID = _reader.IsDBNull(_reader.GetOrdinal("ResidenceTypeID")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("ResidenceTypeID")),
                                        ResidenceTypeName = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType"))
                                    }
                                };
                            }
                        }
                    }

                    if (resident == null)
                        return null;

                    // 2. Related Employment
                    string empSql = "SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID = @residentId";
                    using (SqlCommand empCmd = new SqlCommand(empSql, _conn))
                    {
                        empCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader empReader = empCmd.ExecuteReader())
                        {
                            while (empReader.Read())
                            {
                                var emp = new Employment
                                {
                                    EmploymentID = empReader.GetInt32(empReader.GetOrdinal("EmploymentID")),
                                    ResidentID = empReader.GetInt32(empReader.GetOrdinal("ResidentID")),
                                    Company = empReader.IsDBNull(empReader.GetOrdinal("Company")) ? "" : empReader.GetString(empReader.GetOrdinal("Company")),
                                    Position = empReader.IsDBNull(empReader.GetOrdinal("Position")) ? "" : empReader.GetString(empReader.GetOrdinal("Position")),
                                    LengthOfService = empReader.IsDBNull(empReader.GetOrdinal("LengthOfService")) ? "" : empReader.GetString(empReader.GetOrdinal("LengthOfService"))
                                };
                                resident.Employments.Add(emp);
                            }
                        }
                    }

                    // 3. Spouse (if exists) - read spouse FIRST, then query spouse employments if spouse exists
                    Spouse? spouse = null;
                    int spouseId = 0;
                    string spouseSql = "SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID = @residentId";
                    using (SqlCommand spouseCmd = new SqlCommand(spouseSql, _conn))
                    {
                        spouseCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader spouseReader = spouseCmd.ExecuteReader())
                        {
                            if (spouseReader.Read())
                            {
                                spouseId = spouseReader.GetInt32(spouseReader.GetOrdinal("SpouseID"));
                                spouse = new Spouse
                                {
                                    SpouseID = spouseId,
                                    ResidentID = spouseReader.GetInt32(spouseReader.GetOrdinal("ResidentID")),
                                    SpouseName = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpouseName")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpouseName"))
                                };
                            }
                        }
                    }

                    // 3a. Spouse Employment
                    if (spouse != null)
                    {
                        string spouseEmpSql = "SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID = @spouseId";
                        using (SqlCommand spouseEmpCmd = new SqlCommand(spouseEmpSql, _conn))
                        {
                            spouseEmpCmd.Parameters.AddWithValue("@spouseId", spouseId);
                            using (SqlDataReader spouseEmpReader = spouseEmpCmd.ExecuteReader())
                            {
                                while (spouseEmpReader.Read())
                                {
                                    var spEmp = new SpouseEmployment
                                    {
                                        SpouseEmploymentID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseEmploymentID")),
                                        SpouseID = spouseEmpReader.GetInt32(spouseEmpReader.GetOrdinal("SpouseID")),
                                        Company = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Company")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Company")),
                                        Position = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("Position")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("Position")),
                                        LengthOfService = spouseEmpReader.IsDBNull(spouseEmpReader.GetOrdinal("LengthOfService")) ? "" : spouseEmpReader.GetString(spouseEmpReader.GetOrdinal("LengthOfService"))
                                    };
                                    spouse.Employments.Add(spEmp);
                                }
                            }
                        }
                        resident.Spouse = spouse;
                    }

                    // 4. Purposes (ResidentPurposes, PurposeTypes)
                    string purposesSql = @"
                        SELECT rp.TransactionID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName
                        FROM ResidentPurposes rp
                        INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                        WHERE rp.ResidentID = @residentId";
                    using (SqlCommand purposesCmd = new SqlCommand(purposesSql, _conn))
                    {
                        purposesCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader purposesReader = purposesCmd.ExecuteReader())
                        {
                            while (purposesReader.Read())
                            {
                                var purpose = new ResidentPurpose
                                {
                                    TransactionID = purposesReader.GetString(purposesReader.GetOrdinal("TransactionID")),
                                    ResidentID = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentID")),
                                    PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                    PurposeOthers = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeOthers")) ? null : purposesReader.GetString(purposesReader.GetOrdinal("PurposeOthers")),
                                    PurposeType = new PurposeType
                                    {
                                        PurposeTypeID = purposesReader.GetInt32(purposesReader.GetOrdinal("PurposeTypeID")),
                                        PurposeName = purposesReader.IsDBNull(purposesReader.GetOrdinal("PurposeName")) ? "" : purposesReader.GetString(purposesReader.GetOrdinal("PurposeName"))
                                    }
                                };
                                resident.Purposes.Add(purpose);
                            }
                        }
                    }

                    return resident;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Creates a new resident record in the Residents table and related Employment, Spouse, and Purposes tables
        /// using the data provided in the Residents object, for the normalized schema.
        /// This method establishes a database connection, constructs separate INSERT SQL commands with parameters
        /// for each relevant table, and executes them within a transaction to ensure data consistency.
        /// In case of any exception, it logs the error to the console, rolls back the transaction, and rethrows the exception.
        /// </summary>
        /// <param name="resident">
        /// The Residents object containing the data to be inserted, including optional Employment, Spouse, and Purposes details.
        /// </param>
        public void CreateResident(Resident resident)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();

                    using (var transaction = _conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert into Residents table (normalized: use foreign key IDs)
                            string sqlResidents = @"
                                INSERT INTO Residents (
                                    LastName, FirstName, MiddleName, Address, TelCelNo, SexID, Height, 
                                    Weight, DateOfBirth, PlaceOfBirth, CivilStatusID, VoterIDNo, PollingPlace,
                                    ResidenceTypeID
                                ) VALUES (
                                    @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @SexID, @Height, 
                                    @Weight, @DateOfBirth, @PlaceOfBirth, @CivilStatusID, @VoterIDNo, @PollingPlace,
                                    @ResidenceTypeID
                                );
                                SELECT SCOPE_IDENTITY();";

                            int residentId;
                            using (SqlCommand cmd = new SqlCommand(sqlResidents, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                                cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                                cmd.Parameters.AddWithValue("@MiddleName", (object?)resident.MiddleName ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Address", resident.Address);
                                cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                                cmd.Parameters.AddWithValue("@SexID", resident.SexID);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatusID", resident.CivilStatusID);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceTypeID", resident.ResidenceTypeID);

                                residentId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Insert into Employment table(s) (if data provided)
                            if (resident.Employments != null && resident.Employments.Count > 0)
                            {
                                string sqlEmployment = @"
                                    INSERT INTO Employment (
                                        ResidentID, Company, Position, LengthOfService
                                    ) VALUES (
                                        @ResidentID, @Company, @Position, @LengthOfService
                                    );";
                                foreach (var emp in resident.Employments)
                                {
                                    using (SqlCommand cmd = new SqlCommand(sqlEmployment, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                        cmd.Parameters.AddWithValue("@Company", emp.Company);
                                        cmd.Parameters.AddWithValue("@Position", emp.Position);
                                        cmd.Parameters.AddWithValue("@LengthOfService", emp.LengthOfService);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // 3. Insert into Spouse table (if data provided)
                            int? spouseId = null;
                            if (resident.Spouse != null)
                            {
                                string sqlSpouse = @"
                                    INSERT INTO Spouse (
                                        ResidentID, SpouseName
                                    ) VALUES (
                                        @ResidentID, @SpouseName
                                    );
                                    SELECT SCOPE_IDENTITY();";
                                using (SqlCommand cmd = new SqlCommand(sqlSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    spouseId = Convert.ToInt32(cmd.ExecuteScalar());
                                }

                                // Spouse Employment(s)
                                if (resident.Spouse.Employments != null && resident.Spouse.Employments.Count > 0)
                                {
                                    string sqlSpouseEmp = @"
                                        INSERT INTO SpouseEmployment (
                                            SpouseID, Company, Position, LengthOfService
                                        ) VALUES (
                                            @SpouseID, @Company, @Position, @LengthOfService
                                        );";
                                    foreach (var spEmp in resident.Spouse.Employments)
                                    {
                                        using (SqlCommand cmd = new SqlCommand(sqlSpouseEmp, _conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@SpouseID", spouseId);
                                            cmd.Parameters.AddWithValue("@Company", spEmp.Company);
                                            cmd.Parameters.AddWithValue("@Position", spEmp.Position);
                                            cmd.Parameters.AddWithValue("@LengthOfService", spEmp.LengthOfService);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 4. Insert into ResidentPurposes (if data provided)
                            if (resident.Purposes != null && resident.Purposes.Count > 0)
                            {
                                string sqlPurpose = @"
                                    INSERT INTO ResidentPurposes (
                                        ResidentID, PurposeTypeID, PurposeOthers
                                    ) VALUES (
                                        @ResidentID, @PurposeTypeID, @PurposeOthers
                                    );";
                                foreach (var rp in resident.Purposes)
                                {
                                    using (SqlCommand cmd = new SqlCommand(sqlPurpose, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                        cmd.Parameters.AddWithValue("@PurposeTypeID", rp.PurposeTypeID);
                                        cmd.Parameters.AddWithValue("@PurposeOthers", (object?)rp.PurposeOthers ?? DBNull.Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                // Add log if needed
                AddUserLog(CurrentUser.AccountID, "Add", $"Added resident: {resident.FirstName} {resident.LastName}");
                AutoBackupHelper.IncrementChangeCountAndAutoBackup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Updates an existing resident record in the Residents table and related Employment, Spouse, and Purposes tables
        /// using the data provided in the Residents object.
        /// This method establishes a database connection, constructs individual UPDATE SQL commands with parameters
        /// for each relevant table, and executes them within a transaction to ensure data consistency.
        /// The record to update is identified by ResidentID.
        /// In case of any exception, it logs the error to the console, rolls back the transaction, and rethrows the exception.
        /// </summary>
        /// <param name="resident">
        /// The Residents object containing the updated data, including the ResidentID, and optional updated Employment, Spouse, and Purposes details.
        /// </param>
        public void UpdateResident(Resident resident)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_mainConn))
                {
                    _conn.Open();
                    using (var transaction = _conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Update Residents (normalized: use foreign key IDs)
                            string sqlResidents = @"
                                UPDATE Residents SET
                                    LastName = @LastName,
                                    FirstName = @FirstName,
                                    MiddleName = @MiddleName,
                                    Address = @Address,
                                    TelCelNo = @TelCelNo,
                                    SexID = @SexID,
                                    Height = @Height,
                                    Weight = @Weight,
                                    DateOfBirth = @DateOfBirth,
                                    PlaceOfBirth = @PlaceOfBirth,
                                    CivilStatusID = @CivilStatusID,
                                    VoterIDNo = @VoterIDNo,
                                    PollingPlace = @PollingPlace,
                                    ResidenceTypeID = @ResidenceTypeID
                                WHERE ResidentID = @ResidentID;
                            ";
                            using (var cmd = new SqlCommand(sqlResidents, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                                cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                                cmd.Parameters.AddWithValue("@MiddleName", (object?)resident.MiddleName ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Address", resident.Address);
                                cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                                cmd.Parameters.AddWithValue("@SexID", resident.SexID);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatusID", resident.CivilStatusID);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceTypeID", resident.ResidenceTypeID);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Update Employment (delete all and re-insert for this Resident)
                            string deleteEmployment = "DELETE FROM Employment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteEmployment, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.ExecuteNonQuery();
                            }
                            if (resident.Employments != null && resident.Employments.Count > 0)
                            {
                                string insertEmployment = @"
                                    INSERT INTO Employment (ResidentID, Company, Position, LengthOfService)
                                    VALUES (@ResidentID, @Company, @Position, @LengthOfService);";
                                foreach (var emp in resident.Employments)
                                {
                                    using (var cmd = new SqlCommand(insertEmployment, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                        cmd.Parameters.AddWithValue("@Company", emp.Company);
                                        cmd.Parameters.AddWithValue("@Position", emp.Position);
                                        cmd.Parameters.AddWithValue("@LengthOfService", emp.LengthOfService);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // 3. Update Spouse and related tables
                            // Step 1: Get old SpouseID (if any)
                            int? oldSpouseId = null;
                            string getOldSpouseId = "SELECT SpouseID FROM Spouse WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(getOldSpouseId, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    oldSpouseId = Convert.ToInt32(result);
                            }

                            // Step 2: Delete SpouseEmployment for old SpouseID (if any)
                            if (oldSpouseId.HasValue)
                            {
                                string deleteSpouseEmp = "DELETE FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(deleteSpouseEmp, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", oldSpouseId.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Step 3: Delete Spouse (now no referenced rows exist)
                            string deleteSpouse = "DELETE FROM Spouse WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteSpouse, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.ExecuteNonQuery();
                            }

                            // Step 4: Re-insert Spouse (if any) and related employments
                            int? spouseId = null;
                            if (resident.Spouse != null)
                            {
                                string insertSpouse = @"
                                    INSERT INTO Spouse (ResidentID, SpouseName)
                                    VALUES (@ResidentID, @SpouseName);
                                    SELECT SCOPE_IDENTITY();";
                                using (var cmd = new SqlCommand(insertSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    spouseId = Convert.ToInt32(cmd.ExecuteScalar());
                                }

                                // SpouseEmployment (delete all and re-insert)
                                if (resident.Spouse.Employments != null && resident.Spouse.Employments.Count > 0)
                                {
                                    string insertSpouseEmp = @"
                                        INSERT INTO SpouseEmployment (SpouseID, Company, Position, LengthOfService)
                                        VALUES (@SpouseID, @Company, @Position, @LengthOfService);";
                                    foreach (var spEmp in resident.Spouse.Employments)
                                    {
                                        using (var cmd = new SqlCommand(insertSpouseEmp, _conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@SpouseID", spouseId);
                                            cmd.Parameters.AddWithValue("@Company", spEmp.Company);
                                            cmd.Parameters.AddWithValue("@Position", spEmp.Position);
                                            cmd.Parameters.AddWithValue("@LengthOfService", spEmp.LengthOfService);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 4. Update ResidentPurposes (delete all and re-insert)
                            string deletePurposes = "DELETE FROM ResidentPurposes WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deletePurposes, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.ExecuteNonQuery();
                            }
                            if (resident.Purposes != null && resident.Purposes.Count > 0)
                            {
                                string insertPurpose = @"
                                    INSERT INTO ResidentPurposes (ResidentID, PurposeTypeID, PurposeOthers)
                                    VALUES (@ResidentID, @PurposeTypeID, @PurposeOthers);";
                                foreach (var rp in resident.Purposes)
                                {
                                    using (var cmd = new SqlCommand(insertPurpose, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                        cmd.Parameters.AddWithValue("@PurposeTypeID", rp.PurposeTypeID);
                                        cmd.Parameters.AddWithValue("@PurposeOthers", (object?)rp.PurposeOthers ?? DBNull.Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                AddUserLog(CurrentUser.AccountID, "Edit", $"Edited resident: {resident.FirstName} {resident.LastName}");
                AutoBackupHelper.IncrementChangeCountAndAutoBackup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Archives a resident record by moving all related data to the ResidentsArchiveDB and deleting from the main ResidentsDB.
        /// This method establishes connections to both databases and uses transactions to ensure data consistency.
        /// Uses SET IDENTITY_INSERT for tables with explicit Identity (PK) values.
        /// </summary>
        /// <param name="residentId">The unique identifier of the resident to be archived.</param>
        public void ArchiveResident(int residentId)
        {
            try
            {
                using (SqlConnection mainConn = new SqlConnection(_mainConn))
                using (SqlConnection archiveConn = new SqlConnection(_archiveConn))
                {
                    mainConn.Open();
                    archiveConn.Open();

                    using (var mainTrans = mainConn.BeginTransaction())
                    using (var archiveTrans = archiveConn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Copy Resident (including all FKs) to archive
                            string selectResident = @"
                                SELECT ResidentID, LastName, FirstName, MiddleName, Address, TelCelNo, SexID, DateOfBirth, PlaceOfBirth,
                                       CivilStatusID, VoterIDNo, PollingPlace, ResidenceTypeID, Height, Weight
                                FROM Residents WHERE ResidentID = @ResidentID";
                            Resident? resident = null;
                            using (var cmd = new SqlCommand(selectResident, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        resident = new Resident
                                        {
                                            ResidentID = reader.GetInt32(reader.GetOrdinal("ResidentID")),
                                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                            MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                                            Address = reader.GetString(reader.GetOrdinal("Address")),
                                            TelCelNo = reader.GetString(reader.GetOrdinal("TelCelNo")),
                                            SexID = (byte)reader.GetByte(reader.GetOrdinal("SexID")),
                                            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                            PlaceOfBirth = reader.GetString(reader.GetOrdinal("PlaceOfBirth")),
                                            CivilStatusID = reader.GetInt32(reader.GetOrdinal("CivilStatusID")),
                                            VoterIDNo = reader.GetString(reader.GetOrdinal("VoterIDNo")),
                                            PollingPlace = reader.GetString(reader.GetOrdinal("PollingPlace")),
                                            ResidenceTypeID = reader.GetInt32(reader.GetOrdinal("ResidenceTypeID")),
                                            Height = reader.GetDecimal(reader.GetOrdinal("Height")),
                                            Weight = reader.GetDecimal(reader.GetOrdinal("Weight"))
                                        };
                                    }
                                }
                            }
                            if (resident == null)
                                throw new Exception("Resident not found in main database.");

                            // SET IDENTITY_INSERT Residents ON
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Residents ON;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }
                            string insertResident = @"
                                INSERT INTO Residents (ResidentID, LastName, FirstName, MiddleName, Address, TelCelNo, SexID, DateOfBirth, PlaceOfBirth,
                                                      CivilStatusID, VoterIDNo, PollingPlace, ResidenceTypeID, Height, Weight)
                                VALUES (@ResidentID, @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @SexID, @DateOfBirth, @PlaceOfBirth,
                                        @CivilStatusID, @VoterIDNo, @PollingPlace, @ResidenceTypeID, @Height, @Weight)";
                            using (var cmd = new SqlCommand(insertResident, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                                cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                                cmd.Parameters.AddWithValue("@MiddleName", (object?)resident.MiddleName ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Address", resident.Address);
                                cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                                cmd.Parameters.AddWithValue("@SexID", resident.SexID);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatusID", resident.CivilStatusID);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceTypeID", resident.ResidenceTypeID);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Residents OFF;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }

                            // 2. Copy Employment
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Employment ON;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }
                            string selectEmployment = "SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectEmployment, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string insertEmployment = @"
                                            INSERT INTO Employment (EmploymentID, ResidentID, Company, Position, LengthOfService)
                                            VALUES (@EmploymentID, @ResidentID, @Company, @Position, @LengthOfService)";
                                        using (var insertCmd = new SqlCommand(insertEmployment, archiveConn, archiveTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@EmploymentID", reader.GetInt32(reader.GetOrdinal("EmploymentID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@Company", reader.GetString(reader.GetOrdinal("Company")));
                                            insertCmd.Parameters.AddWithValue("@Position", reader.GetString(reader.GetOrdinal("Position")));
                                            insertCmd.Parameters.AddWithValue("@LengthOfService", reader.GetString(reader.GetOrdinal("LengthOfService")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Employment OFF;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }

                            // 3. Copy Spouse
                            int? spouseId = null;
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Spouse ON;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }
                            string selectSpouse = "SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectSpouse, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        spouseId = reader.GetInt32(reader.GetOrdinal("SpouseID"));
                                        string insertSpouse = @"
                                            INSERT INTO Spouse (SpouseID, ResidentID, SpouseName)
                                            VALUES (@SpouseID, @ResidentID, @SpouseName)";
                                        using (var insertCmd = new SqlCommand(insertSpouse, archiveConn, archiveTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@SpouseID", reader.GetInt32(reader.GetOrdinal("SpouseID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@SpouseName", reader.GetString(reader.GetOrdinal("SpouseName")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Spouse OFF;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }

                            // 4. Copy SpouseEmployment (if spouse exists)
                            if (spouseId.HasValue)
                            {
                                using (var cmd = new SqlCommand("SET IDENTITY_INSERT SpouseEmployment ON;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }
                                string selectSpouseEmp = "SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(selectSpouseEmp, mainConn, mainTrans))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", spouseId.Value);
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string insertSpouseEmp = @"
                                                INSERT INTO SpouseEmployment (SpouseEmploymentID, SpouseID, Company, Position, LengthOfService)
                                                VALUES (@SpouseEmploymentID, @SpouseID, @Company, @Position, @LengthOfService)";
                                            using (var insertCmd = new SqlCommand(insertSpouseEmp, archiveConn, archiveTrans))
                                            {
                                                insertCmd.Parameters.AddWithValue("@SpouseEmploymentID", reader.GetInt32(reader.GetOrdinal("SpouseEmploymentID")));
                                                insertCmd.Parameters.AddWithValue("@SpouseID", reader.GetInt32(reader.GetOrdinal("SpouseID")));
                                                insertCmd.Parameters.AddWithValue("@Company", reader.GetString(reader.GetOrdinal("Company")));
                                                insertCmd.Parameters.AddWithValue("@Position", reader.GetString(reader.GetOrdinal("Position")));
                                                insertCmd.Parameters.AddWithValue("@LengthOfService", reader.GetString(reader.GetOrdinal("LengthOfService")));
                                                insertCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                using (var cmd = new SqlCommand("SET IDENTITY_INSERT SpouseEmployment OFF;", archiveConn, archiveTrans)) { cmd.ExecuteNonQuery(); }
                            }

                            // 5. Copy ResidentPurposes
                            string selectPurposes = "SELECT TransactionID, ResidentID, PurposeTypeID, PurposeOthers FROM ResidentPurposes WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectPurposes, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string insertPurpose = @"
                                            INSERT INTO ResidentPurposes (TransactionID, ResidentID, PurposeTypeID, PurposeOthers)
                                            VALUES (@TransactionID, @ResidentID, @PurposeTypeID, @PurposeOthers)";
                                        using (var insertCmd = new SqlCommand(insertPurpose, archiveConn, archiveTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@TransactionID", reader.GetString(reader.GetOrdinal("TransactionID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@PurposeTypeID", reader.GetInt32(reader.GetOrdinal("PurposeTypeID")));
                                            insertCmd.Parameters.AddWithValue("@PurposeOthers", reader.IsDBNull(reader.GetOrdinal("PurposeOthers")) ? (object)DBNull.Value : reader.GetString(reader.GetOrdinal("PurposeOthers")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 6. Delete from all main tables (order: child first to avoid FK violation)
                            string deleteEmployment = "DELETE FROM Employment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteEmployment, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            if (spouseId.HasValue)
                            {
                                string deleteSpouseEmp = "DELETE FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(deleteSpouseEmp, mainConn, mainTrans))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", spouseId.Value);
                                    cmd.ExecuteNonQuery();
                                }
                                string deleteSpouse = "DELETE FROM Spouse WHERE ResidentID = @ResidentID";
                                using (var cmd = new SqlCommand(deleteSpouse, mainConn, mainTrans))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            string deletePurposes = "DELETE FROM ResidentPurposes WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deletePurposes, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            string deleteResident = "DELETE FROM Residents WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteResident, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }

                            // Commit both transactions
                            archiveTrans.Commit();
                            mainTrans.Commit();
                        }
                        catch
                        {
                            archiveTrans.Rollback();
                            mainTrans.Rollback();
                            throw;
                        }
                    }
                }
                AutoBackupHelper.IncrementChangeCountAndAutoBackup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Restores a resident from the archive DB (ResidentsArchiveDB) to the main DB (ResidentsDB), moving all related data.
        /// All operations are transactional for consistency.
        /// </summary>
        /// <param name="residentId">The ResidentID of the resident to restore.</param>
        public void RestoreResident(int residentId)
        {
            try
            {
                using (SqlConnection archiveConn = new SqlConnection(_archiveConn))
                using (SqlConnection mainConn = new SqlConnection(_mainConn))
                {
                    archiveConn.Open();
                    mainConn.Open();

                    using (var archiveTrans = archiveConn.BeginTransaction())
                    using (var mainTrans = mainConn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Resident
                            Resident? resident = null;
                            string selectResident = @"
                                SELECT ResidentID, LastName, FirstName, MiddleName, Address, TelCelNo, SexID, DateOfBirth, PlaceOfBirth,
                                       CivilStatusID, VoterIDNo, PollingPlace, ResidenceTypeID, Height, Weight
                                FROM Residents WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectResident, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        resident = new Resident
                                        {
                                            ResidentID = reader.GetInt32(reader.GetOrdinal("ResidentID")),
                                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                            MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                                            Address = reader.GetString(reader.GetOrdinal("Address")),
                                            TelCelNo = reader.GetString(reader.GetOrdinal("TelCelNo")),
                                            SexID = (byte)reader.GetByte(reader.GetOrdinal("SexID")),
                                            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                            PlaceOfBirth = reader.GetString(reader.GetOrdinal("PlaceOfBirth")),
                                            CivilStatusID = reader.GetInt32(reader.GetOrdinal("CivilStatusID")),
                                            VoterIDNo = reader.GetString(reader.GetOrdinal("VoterIDNo")),
                                            PollingPlace = reader.GetString(reader.GetOrdinal("PollingPlace")),
                                            ResidenceTypeID = reader.GetInt32(reader.GetOrdinal("ResidenceTypeID")),
                                            Height = reader.GetDecimal(reader.GetOrdinal("Height")),
                                            Weight = reader.GetDecimal(reader.GetOrdinal("Weight"))
                                        };
                                    }
                                }
                            }
                            if (resident == null)
                                throw new Exception("Resident not found in archive database.");

                            // SET IDENTITY_INSERT Residents ON
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Residents ON;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }
                            string insertResident = @"
                                INSERT INTO Residents (ResidentID, LastName, FirstName, MiddleName, Address, TelCelNo, SexID, DateOfBirth, PlaceOfBirth,
                                                      CivilStatusID, VoterIDNo, PollingPlace, ResidenceTypeID, Height, Weight)
                                VALUES (@ResidentID, @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @SexID, @DateOfBirth, @PlaceOfBirth,
                                        @CivilStatusID, @VoterIDNo, @PollingPlace, @ResidenceTypeID, @Height, @Weight)";
                            using (var cmd = new SqlCommand(insertResident, mainConn, mainTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                                cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                                cmd.Parameters.AddWithValue("@MiddleName", (object?)resident.MiddleName ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Address", resident.Address);
                                cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                                cmd.Parameters.AddWithValue("@SexID", resident.SexID);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatusID", resident.CivilStatusID);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceTypeID", resident.ResidenceTypeID);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Residents OFF;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }

                            // 2. Employment
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Employment ON;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }
                            string selectEmployment = "SELECT EmploymentID, ResidentID, Company, Position, LengthOfService FROM Employment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectEmployment, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string insertEmployment = @"
                                            INSERT INTO Employment (EmploymentID, ResidentID, Company, Position, LengthOfService)
                                            VALUES (@EmploymentID, @ResidentID, @Company, @Position, @LengthOfService)";
                                        using (var insertCmd = new SqlCommand(insertEmployment, mainConn, mainTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@EmploymentID", reader.GetInt32(reader.GetOrdinal("EmploymentID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@Company", reader.GetString(reader.GetOrdinal("Company")));
                                            insertCmd.Parameters.AddWithValue("@Position", reader.GetString(reader.GetOrdinal("Position")));
                                            insertCmd.Parameters.AddWithValue("@LengthOfService", reader.GetString(reader.GetOrdinal("LengthOfService")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Employment OFF;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }

                            // 3. Spouse and SpouseEmployment
                            int? spouseId = null;
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Spouse ON;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }
                            string selectSpouse = "SELECT SpouseID, ResidentID, SpouseName FROM Spouse WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectSpouse, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        spouseId = reader.GetInt32(reader.GetOrdinal("SpouseID"));
                                        string insertSpouse = @"
                                            INSERT INTO Spouse (SpouseID, ResidentID, SpouseName)
                                            VALUES (@SpouseID, @ResidentID, @SpouseName)";
                                        using (var insertCmd = new SqlCommand(insertSpouse, mainConn, mainTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@SpouseID", reader.GetInt32(reader.GetOrdinal("SpouseID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@SpouseName", reader.GetString(reader.GetOrdinal("SpouseName")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            using (var cmd = new SqlCommand("SET IDENTITY_INSERT Spouse OFF;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }

                            if (spouseId.HasValue)
                            {
                                using (var cmd = new SqlCommand("SET IDENTITY_INSERT SpouseEmployment ON;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }
                                string selectSpouseEmp = "SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(selectSpouseEmp, archiveConn, archiveTrans))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", spouseId.Value);
                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string insertSpouseEmp = @"
                                                INSERT INTO SpouseEmployment (SpouseEmploymentID, SpouseID, Company, Position, LengthOfService)
                                                VALUES (@SpouseEmploymentID, @SpouseID, @Company, @Position, @LengthOfService)";
                                            using (var insertCmd = new SqlCommand(insertSpouseEmp, mainConn, mainTrans))
                                            {
                                                insertCmd.Parameters.AddWithValue("@SpouseEmploymentID", reader.GetInt32(reader.GetOrdinal("SpouseEmploymentID")));
                                                insertCmd.Parameters.AddWithValue("@SpouseID", reader.GetInt32(reader.GetOrdinal("SpouseID")));
                                                insertCmd.Parameters.AddWithValue("@Company", reader.GetString(reader.GetOrdinal("Company")));
                                                insertCmd.Parameters.AddWithValue("@Position", reader.GetString(reader.GetOrdinal("Position")));
                                                insertCmd.Parameters.AddWithValue("@LengthOfService", reader.GetString(reader.GetOrdinal("LengthOfService")));
                                                insertCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                using (var cmd = new SqlCommand("SET IDENTITY_INSERT SpouseEmployment OFF;", mainConn, mainTrans)) { cmd.ExecuteNonQuery(); }
                            }

                            // 4. ResidentPurposes
                            string selectPurposes = "SELECT TransactionID, ResidentID, PurposeTypeID, PurposeOthers FROM ResidentPurposes WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(selectPurposes, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string insertPurpose = @"
                                            INSERT INTO ResidentPurposes (TransactionID, ResidentID, PurposeTypeID, PurposeOthers)
                                            VALUES (@TransactionID, @ResidentID, @PurposeTypeID, @PurposeOthers)";
                                        using (var insertCmd = new SqlCommand(insertPurpose, mainConn, mainTrans))
                                        {
                                            insertCmd.Parameters.AddWithValue("@TransactionID", reader.GetString(reader.GetOrdinal("TransactionID")));
                                            insertCmd.Parameters.AddWithValue("@ResidentID", reader.GetInt32(reader.GetOrdinal("ResidentID")));
                                            insertCmd.Parameters.AddWithValue("@PurposeTypeID", reader.GetInt32(reader.GetOrdinal("PurposeTypeID")));
                                            insertCmd.Parameters.AddWithValue("@PurposeOthers", reader.IsDBNull(reader.GetOrdinal("PurposeOthers")) ? (object)DBNull.Value : reader.GetString(reader.GetOrdinal("PurposeOthers")));
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 5. Delete from archive (order: child first)
                            string deleteEmployment = "DELETE FROM Employment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteEmployment, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            if (spouseId.HasValue)
                            {
                                string deleteSpouseEmp = "DELETE FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(deleteSpouseEmp, archiveConn, archiveTrans))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", spouseId.Value);
                                    cmd.ExecuteNonQuery();
                                }
                                string deleteSpouse = "DELETE FROM Spouse WHERE ResidentID = @ResidentID";
                                using (var cmd = new SqlCommand(deleteSpouse, archiveConn, archiveTrans))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            string deletePurposes = "DELETE FROM ResidentPurposes WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deletePurposes, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            string deleteResident = "DELETE FROM Residents WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deleteResident, archiveConn, archiveTrans))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }

                            // Commit both transactions
                            mainTrans.Commit();
                            archiveTrans.Commit();
                        }
                        catch
                        {
                            mainTrans.Rollback();
                            archiveTrans.Rollback();
                            throw;
                        }
                    }
                }
                // Optionally, add logging or backup triggers here
                // AddUserLog(CurrentUser.AccountID, "Restore", $"Restored resident: {residentId}");
                // AutoBackupHelper.IncrementChangeCountAndAutoBackup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
    }
}