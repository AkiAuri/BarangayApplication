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
        private readonly string _repoconn = "Data Source=localhost,1433;Initial Catalog=sybau_database;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        /// <summary>
        /// Gets the list of applicant residents. This method connects to the database,
        /// executes a SELECT query against the SVResidents table, reads each resident's data,
        /// and returns a list of Residents objects. In case of an exception, it writes the
        /// error details to the console and rethrows the exception.
        /// </summary>
        /// <returns>A List of Residents representing the applicants retrieved from the database.</returns>
        /// 
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    // This query counts purposes by joining ResidentPurposes and PurposeTypes, grouping by PurposeName
                    string sql = @"
                        SELECT 
                            pt.PurposeName, COUNT(rp.ResidentPurposeID) AS Count
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

                            // Normalize purpose names to match dictionary keys
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
                                    // For "Others" we sum all ResidentPurposes with PurposeType 'Others' AND a non-null PurposeOthers
                                    purposeDistribution["Others"] = count; 
                                    break;
                                default: break;
                            }
                        }
                    }

                    // Additionally, for "Others", count only those with non-null/non-empty PurposeOthers
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    string sql = "SELECT Sex, COUNT(*) AS Count FROM Residents GROUP BY Sex";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            string sexRaw = _reader.IsDBNull(0) ? "" : _reader.GetString(0).Trim();
                            int count = _reader.IsDBNull(1) ? 0 : _reader.GetInt32(1);

                            string sex;
                            if (string.Equals(sexRaw, "M", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(sexRaw, "Male", StringComparison.OrdinalIgnoreCase))
                                sex = "Male";
                            else if (string.Equals(sexRaw, "F", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(sexRaw, "Female", StringComparison.OrdinalIgnoreCase))
                                sex = "Female";
                            else
                                sex = "Other";

                            if (genderDistribution.ContainsKey(sex))
                                genderDistribution[sex] += count;
                            else
                                genderDistribution["Other"] += count;
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    // No more Age column, so calculate age from DateOfBirth
                    string sql = "SELECT DateOfBirth FROM Residents";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            if (_reader.IsDBNull(0)) continue;
                            DateTime dob = _reader.GetDateTime(0);

                            // Calculate age
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

        // Method to get the logged-in user's account name based on accountID
        private string GetLoggedInUserName(string currentAccountId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    string sql = "SELECT accountName FROM Users WHERE accountID = @CurrentAccountId";

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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    string sql = @"
                        SELECT 
                            FORMAT(Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                            UserName AS [USER], 
                            Action AS [ACTION], 
                            Description AS [DESCRIPTION]
                        FROM UserLogs
                        WHERE Action = @Action
                        ORDER BY Timestamp DESC";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@Action", action);

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

        // Method to add a user log entry with accountName
        public void AddUserLog(string currentAccountId, string action, string description)
        {
            try
            {
                string accountName = GetLoggedInUserName(currentAccountId);

                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    // Normalize description (trim and lower for comparison)
                    string normDescription = description?.Trim();

                    // Check if a similar log entry already exists to prevent duplication within the last minute
                    string checkSql = @"
                        SELECT COUNT(*) 
                        FROM UserLogs 
                        WHERE UserName = @UserName 
                          AND Action = @Action 
                          AND Description = @Description
                          AND DATEDIFF(SECOND, Timestamp, @Now) BETWEEN 0 AND 60";

                    using (SqlCommand checkCmd = new SqlCommand(checkSql, _conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserName", accountName);
                        checkCmd.Parameters.AddWithValue("@Action", action);
                        checkCmd.Parameters.AddWithValue("@Description", normDescription);
                        checkCmd.Parameters.AddWithValue("@Now", DateTime.Now);

                        int existingCount = (int)checkCmd.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            // Log entry already exists, skip adding
                            return;
                        }
                    }

                    // Insert the new log entry
                    string insertSql = @"
                       INSERT INTO UserLogs (UserName, Action, Description, Timestamp)
                       VALUES (@UserName, @Action, @Description, @Timestamp)";

                    using (SqlCommand insertCmd = new SqlCommand(insertSql, _conn))
                    {
                        insertCmd.Parameters.AddWithValue("@UserName", accountName);
                        insertCmd.Parameters.AddWithValue("@Action", action);
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
        public DataTable GetLogsForPage(int offset, int rowsPerPage) //for populating the logbook
        {
            var dataTable = new DataTable();
            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    string sql = @"
                SELECT TOP (@RowsPerPage)
                    FORMAT(Timestamp, 'yyyy-MM-dd hh:mm:ss tt') AS [DATE & TIME],
                    UserName AS [USER], 
                    Action AS [ACTION], 
                    Description AS [DESCRIPTION]
                FROM UserLogs
                ORDER BY Timestamp DESC";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
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

        public int GetTotalLogCount() //for log counts
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
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
        // end of logbook codes

        public List<Resident> GetApplicants()
        {
            var residents = new List<Resident>();

            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    // Query main resident info only (other data is retrieved via additional queries)
                    string sql = @"
                                SELECT 
                                    ResidentID, 
                                    LastName, 
                                    FirstName, 
                                    MiddleName, 
                                    Address, 
                                    TelCelNo, 
                                    Sex, 
                                    Height, 
                                    Weight, 
                                    DateOfBirth, 
                                    PlaceOfBirth, 
                                    CivilStatus, 
                                    VoterIDNo, 
                                    PollingPlace,
                                    ResidenceType, 
                                    PaymentAmount, 
                                    PaymentFrequency
                                FROM Residents
                                WHERE isArchived = 0
                                ORDER BY ResidentID DESC;";
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
                                Sex = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex")),
                                Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus")),
                                VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType")),
                                PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount")),
                                PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? "" : _reader.GetString(_reader.GetOrdinal("PaymentFrequency")),
                            };

                            residents.Add(resident);
                        }
                    }

                    // Bulk load other info for all residents (use dictionaries for efficient lookup)
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

                    // Previous Employment
                    var prevEmpDict = new Dictionary<int, List<PreviousEmployment>>();
                    if (residentIds.Count > 0)
                    {
                        string prevEmpSql = $"SELECT PreviousEmploymentID, ResidentID, Company, Position, LengthOfService FROM PreviousEmployment WHERE ResidentID IN ({string.Join(",", residentIds)})";
                        using (SqlCommand prevEmpCmd = new SqlCommand(prevEmpSql, _conn))
                        using (SqlDataReader prevEmpReader = prevEmpCmd.ExecuteReader())
                        {
                            while (prevEmpReader.Read())
                            {
                                int resId = prevEmpReader.GetInt32(prevEmpReader.GetOrdinal("ResidentID"));
                                var prevEmp = new PreviousEmployment
                                {
                                    PreviousEmploymentID = prevEmpReader.GetInt32(prevEmpReader.GetOrdinal("PreviousEmploymentID")),
                                    ResidentID = resId,
                                    Company = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("Company")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("Company")),
                                    Position = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("Position")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("Position")),
                                    LengthOfService = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("LengthOfService")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("LengthOfService"))
                                };
                                if (!prevEmpDict.ContainsKey(resId))
                                    prevEmpDict[resId] = new List<PreviousEmployment>();
                                prevEmpDict[resId].Add(prevEmp);
                            }
                        }
                    }
                    foreach (var r in residents)
                        if (prevEmpDict.ContainsKey(r.ResidentID))
                            r.PreviousEmployments = prevEmpDict[r.ResidentID];

                    // Spouse, SpouseEmployment, SpousePreviousEmployment
                    var spouseDict = new Dictionary<int, Spouse>();
                    if (residentIds.Count > 0)
                    {
                        string spouseSql = $"SELECT SpouseID, ResidentID, SpouseName, SpousePhone FROM Spouse WHERE ResidentID IN ({string.Join(",", residentIds)})";
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
                                    SpousePhone = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpousePhone")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpousePhone")),
                                };
                                spouseDict[resId] = sp;
                            }
                        }

                        // SpouseEmployment
                        string spouseEmpSql = $"SELECT SpouseEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpouseEmployment WHERE SpouseID IN (SELECT SpouseID FROM Spouse WHERE ResidentID IN ({string.Join(",", residentIds)}))";
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

                        // SpousePreviousEmployment
                        string spousePrevEmpSql = $"SELECT SpousePrevEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpousePreviousEmployment WHERE SpouseID IN (SELECT SpouseID FROM Spouse WHERE ResidentID IN ({string.Join(",", residentIds)}))";
                        using (SqlCommand spousePrevEmpCmd = new SqlCommand(spousePrevEmpSql, _conn))
                        using (SqlDataReader spousePrevEmpReader = spousePrevEmpCmd.ExecuteReader())
                        {
                            var spousePrevEmpDict = new Dictionary<int, List<SpousePreviousEmployment>>();
                            while (spousePrevEmpReader.Read())
                            {
                                int spouseId = spousePrevEmpReader.GetInt32(spousePrevEmpReader.GetOrdinal("SpouseID"));
                                var spPrevEmp = new SpousePreviousEmployment
                                {
                                    SpousePrevEmploymentID = spousePrevEmpReader.GetInt32(spousePrevEmpReader.GetOrdinal("SpousePrevEmploymentID")),
                                    SpouseID = spouseId,
                                    Company = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("Company")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("Company")),
                                    Position = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("Position")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("Position")),
                                    LengthOfService = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("LengthOfService")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("LengthOfService"))
                                };
                                if (!spousePrevEmpDict.ContainsKey(spouseId))
                                    spousePrevEmpDict[spouseId] = new List<SpousePreviousEmployment>();
                                spousePrevEmpDict[spouseId].Add(spPrevEmp);
                            }
                            foreach (var sp in spouseDict.Values)
                                if (spousePrevEmpDict.ContainsKey(sp.SpouseID))
                                    sp.PreviousEmployments = spousePrevEmpDict[sp.SpouseID];
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
                            SELECT rp.ResidentPurposeID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName 
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
                                    ResidentPurposeID = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentPurposeID")),
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    // 1. Main Resident info
                    Resident? resident = null;
                    string sql = @"
                                SELECT 
                                    ResidentID, 
                                    LastName, 
                                    FirstName, 
                                    MiddleName, 
                                    Address, 
                                    TelCelNo, 
                                    Sex, 
                                    Height, 
                                    Weight, 
                                    DateOfBirth, 
                                    PlaceOfBirth, 
                                    CivilStatus, 
                                    VoterIDNo, 
                                    PollingPlace,
                                    ResidenceType, 
                                    PaymentAmount, 
                                    PaymentFrequency
                                FROM Residents
                                WHERE isArchived = 0
                                ORDER BY ResidentID DESC;";

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
                                    Sex = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex")),
                                    Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height")),
                                    Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight")),
                                    DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth")),
                                    PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                    CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus")),
                                    VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                    PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                    ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType")),
                                    PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount")),
                                    PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? "" : _reader.GetString(_reader.GetOrdinal("PaymentFrequency")),
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

                    // 3. Previous Employment
                    string prevEmpSql = "SELECT PreviousEmploymentID, ResidentID, Company, Position, LengthOfService FROM PreviousEmployment WHERE ResidentID = @residentId";
                    using (SqlCommand prevEmpCmd = new SqlCommand(prevEmpSql, _conn))
                    {
                        prevEmpCmd.Parameters.AddWithValue("@residentId", residentId);
                        using (SqlDataReader prevEmpReader = prevEmpCmd.ExecuteReader())
                        {
                            while (prevEmpReader.Read())
                            {
                                var prevEmp = new PreviousEmployment
                                {
                                    PreviousEmploymentID = prevEmpReader.GetInt32(prevEmpReader.GetOrdinal("PreviousEmploymentID")),
                                    ResidentID = prevEmpReader.GetInt32(prevEmpReader.GetOrdinal("ResidentID")),
                                    Company = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("Company")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("Company")),
                                    Position = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("Position")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("Position")),
                                    LengthOfService = prevEmpReader.IsDBNull(prevEmpReader.GetOrdinal("LengthOfService")) ? "" : prevEmpReader.GetString(prevEmpReader.GetOrdinal("LengthOfService"))
                                };
                                resident.PreviousEmployments.Add(prevEmp);
                            }
                        }
                    }

                    // 4. Spouse (if exists) - read spouse FIRST, then query spouse employments if spouse exists
                    Spouse? spouse = null;
                    int spouseId = 0;
                    string spouseSql = "SELECT SpouseID, ResidentID, SpouseName, SpousePhone FROM Spouse WHERE ResidentID = @residentId";
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
                                    SpouseName = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpouseName")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpouseName")),
                                    SpousePhone = spouseReader.IsDBNull(spouseReader.GetOrdinal("SpousePhone")) ? "" : spouseReader.GetString(spouseReader.GetOrdinal("SpousePhone")),
                                };
                            }
                        }
                    }

                    // 4a. Spouse Employment
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

                        // 4b. Spouse Previous Employment
                        string spousePrevEmpSql = "SELECT SpousePrevEmploymentID, SpouseID, Company, Position, LengthOfService FROM SpousePreviousEmployment WHERE SpouseID = @spouseId";
                        using (SqlCommand spousePrevEmpCmd = new SqlCommand(spousePrevEmpSql, _conn))
                        {
                            spousePrevEmpCmd.Parameters.AddWithValue("@spouseId", spouseId);
                            using (SqlDataReader spousePrevEmpReader = spousePrevEmpCmd.ExecuteReader())
                            {
                                while (spousePrevEmpReader.Read())
                                {
                                    var spPrevEmp = new SpousePreviousEmployment
                                    {
                                        SpousePrevEmploymentID = spousePrevEmpReader.GetInt32(spousePrevEmpReader.GetOrdinal("SpousePrevEmploymentID")),
                                        SpouseID = spousePrevEmpReader.GetInt32(spousePrevEmpReader.GetOrdinal("SpouseID")),
                                        Company = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("Company")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("Company")),
                                        Position = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("Position")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("Position")),
                                        LengthOfService = spousePrevEmpReader.IsDBNull(spousePrevEmpReader.GetOrdinal("LengthOfService")) ? "" : spousePrevEmpReader.GetString(spousePrevEmpReader.GetOrdinal("LengthOfService"))
                                    };
                                    spouse.PreviousEmployments.Add(spPrevEmp);
                                }
                            }
                        }

                        resident.Spouse = spouse;
                    }

                    // 5. Purposes (ResidentPurposes, PurposeTypes)
                    string purposesSql = @"
                        SELECT rp.ResidentPurposeID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName
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
                                    ResidentPurposeID = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentPurposeID")),
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
        /// using the data provided in the Residents object.
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    using (var transaction = _conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert into Residents table
                            string sqlResidents = @"
                                INSERT INTO Residents (
                                    LastName, FirstName, MiddleName, Address, TelCelNo, Sex, Height, 
                                    Weight, DateOfBirth, PlaceOfBirth, CivilStatus, VoterIDNo, PollingPlace,
                                    ResidenceType, PaymentAmount, PaymentFrequency
                                ) VALUES (
                                    @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @Sex, @Height, 
                                    @Weight, @DateOfBirth, @PlaceOfBirth, @CivilStatus, @VoterIDNo, @PollingPlace,
                                    @ResidenceType, @PaymentAmount, @PaymentFrequency
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
                                cmd.Parameters.AddWithValue("@Sex", resident.Sex);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                                cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                                cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);

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

                            // 3. Insert into PreviousEmployment table(s) (if data provided)
                            if (resident.PreviousEmployments != null && resident.PreviousEmployments.Count > 0)
                            {
                                string sqlPrevEmployment = @"
                                    INSERT INTO PreviousEmployment (
                                        ResidentID, Company, Position, LengthOfService
                                    ) VALUES (
                                        @ResidentID, @Company, @Position, @LengthOfService
                                    );";
                                foreach (var prevEmp in resident.PreviousEmployments)
                                {
                                    using (SqlCommand cmd = new SqlCommand(sqlPrevEmployment, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                        cmd.Parameters.AddWithValue("@Company", prevEmp.Company);
                                        cmd.Parameters.AddWithValue("@Position", prevEmp.Position);
                                        cmd.Parameters.AddWithValue("@LengthOfService", prevEmp.LengthOfService);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // 4. Insert into Spouse table (if data provided)
                            int? spouseId = null;
                            if (resident.Spouse != null)
                            {
                                string sqlSpouse = @"
                                    INSERT INTO Spouse (
                                        ResidentID, SpouseName, SpousePhone
                                    ) VALUES (
                                        @ResidentID, @SpouseName, @SpousePhone
                                    );
                                    SELECT SCOPE_IDENTITY();";
                                using (SqlCommand cmd = new SqlCommand(sqlSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    cmd.Parameters.AddWithValue("@SpousePhone", resident.Spouse.SpousePhone);
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
                                // Spouse Previous Employment(s)
                                if (resident.Spouse.PreviousEmployments != null && resident.Spouse.PreviousEmployments.Count > 0)
                                {
                                    string sqlSpousePrevEmp = @"
                                        INSERT INTO SpousePreviousEmployment (
                                            SpouseID, Company, Position, LengthOfService
                                        ) VALUES (
                                            @SpouseID, @Company, @Position, @LengthOfService
                                        );";
                                    foreach (var spPrevEmp in resident.Spouse.PreviousEmployments)
                                    {
                                        using (SqlCommand cmd = new SqlCommand(sqlSpousePrevEmp, _conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@SpouseID", spouseId);
                                            cmd.Parameters.AddWithValue("@Company", spPrevEmp.Company);
                                            cmd.Parameters.AddWithValue("@Position", spPrevEmp.Position);
                                            cmd.Parameters.AddWithValue("@LengthOfService", spPrevEmp.LengthOfService);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 5. Insert into ResidentPurposes (if data provided)
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
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    using (var transaction = _conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Update Residents
                            string sqlResidents = @"
                                UPDATE Residents SET
                                    LastName = @LastName,
                                    FirstName = @FirstName,
                                    MiddleName = @MiddleName,
                                    Address = @Address,
                                    TelCelNo = @TelCelNo,
                                    Sex = @Sex,
                                    Height = @Height,
                                    Weight = @Weight,
                                    DateOfBirth = @DateOfBirth,
                                    PlaceOfBirth = @PlaceOfBirth,
                                    CivilStatus = @CivilStatus,
                                    VoterIDNo = @VoterIDNo,
                                    PollingPlace = @PollingPlace,
                                    ResidenceType = @ResidenceType,
                                    PaymentAmount = @PaymentAmount,
                                    PaymentFrequency = @PaymentFrequency
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
                                cmd.Parameters.AddWithValue("@Sex", resident.Sex);
                                cmd.Parameters.AddWithValue("@Height", resident.Height);
                                cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                                cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                                cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                                cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);
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

                            // 3. Update PreviousEmployment (delete all and re-insert)
                            string deletePrevEmployment = "DELETE FROM PreviousEmployment WHERE ResidentID = @ResidentID";
                            using (var cmd = new SqlCommand(deletePrevEmployment, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                cmd.ExecuteNonQuery();
                            }
                            if (resident.PreviousEmployments != null && resident.PreviousEmployments.Count > 0)
                            {
                                string insertPrevEmployment = @"
                                    INSERT INTO PreviousEmployment (ResidentID, Company, Position, LengthOfService)
                                    VALUES (@ResidentID, @Company, @Position, @LengthOfService);";
                                foreach (var prevEmp in resident.PreviousEmployments)
                                {
                                    using (var cmd = new SqlCommand(insertPrevEmployment, _conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                        cmd.Parameters.AddWithValue("@Company", prevEmp.Company);
                                        cmd.Parameters.AddWithValue("@Position", prevEmp.Position);
                                        cmd.Parameters.AddWithValue("@LengthOfService", prevEmp.LengthOfService);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // 4. Update Spouse and related tables

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

                            // Step 2: Delete SpouseEmployment and SpousePreviousEmployment for old SpouseID (if any)
                            if (oldSpouseId.HasValue)
                            {
                                string deleteSpouseEmp = "DELETE FROM SpouseEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(deleteSpouseEmp, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@SpouseID", oldSpouseId.Value);
                                    cmd.ExecuteNonQuery();
                                }

                                string deleteSpousePrevEmp = "DELETE FROM SpousePreviousEmployment WHERE SpouseID = @SpouseID";
                                using (var cmd = new SqlCommand(deleteSpousePrevEmp, _conn, transaction))
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
                                    INSERT INTO Spouse (ResidentID, SpouseName, SpousePhone)
                                    VALUES (@ResidentID, @SpouseName, @SpousePhone);
                                    SELECT SCOPE_IDENTITY();";
                                using (var cmd = new SqlCommand(insertSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    cmd.Parameters.AddWithValue("@SpousePhone", resident.Spouse.SpousePhone);
                                    spouseId = Convert.ToInt32(cmd.ExecuteScalar());
                                }

                                // SpouseEmployment (delete all and re-insert)
                                // No need to delete here; already deleted above for oldSpouseId

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

                                // SpousePreviousEmployment (delete all and re-insert)
                                // No need to delete here; already deleted above for oldSpouseId

                                if (resident.Spouse.PreviousEmployments != null && resident.Spouse.PreviousEmployments.Count > 0)
                                {
                                    string insertSpousePrevEmp = @"
                                        INSERT INTO SpousePreviousEmployment (SpouseID, Company, Position, LengthOfService)
                                        VALUES (@SpouseID, @Company, @Position, @LengthOfService);";
                                    foreach (var spPrevEmp in resident.Spouse.PreviousEmployments)
                                    {
                                        using (var cmd = new SqlCommand(insertSpousePrevEmp, _conn, transaction))
                                        {
                                            cmd.Parameters.AddWithValue("@SpouseID", spouseId);
                                            cmd.Parameters.AddWithValue("@Company", spPrevEmp.Company);
                                            cmd.Parameters.AddWithValue("@Position", spPrevEmp.Position);
                                            cmd.Parameters.AddWithValue("@LengthOfService", spPrevEmp.LengthOfService);
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // 5. Update ResidentPurposes (delete all and re-insert)
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
        /// Archives a resident record by setting isArchived=1 based on the provided residentId.
        /// Related Employment, Spouse, and Purposes records are left intact.
        /// The method establishes a database connection, creates an UPDATE SQL command with a parameter,
        /// and executes the command within a transaction to ensure referential integrity.
        /// </summary>
        /// <param name="residentId">The unique identifier of the resident to be archived.</param>
        public void ArchiveResident(int residentId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();
                    using (var transaction = _conn.BeginTransaction())
                    {
                        try
                        {
                            // Only update the isArchived column for the resident
                            string sqlArchive = "UPDATE Residents SET isArchived = 1 WHERE ResidentID = @ResidentID;";
                            using (SqlCommand cmd = new SqlCommand(sqlArchive, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                AddUserLog(CurrentUser.AccountID, "Archived", $"Archived resident with ID: {residentId}");
                AutoBackupHelper.IncrementChangeCountAndAutoBackup();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        public List<Resident> GetArchivedResidents()
        {
            var residents = new List<Resident>();
            using (SqlConnection conn = new SqlConnection(_repoconn))
            {
                conn.Open();
                string query = "SELECT * FROM Residents WHERE isArchived = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var resident = new Resident
                        {
                            ResidentID = reader["ResidentID"] != DBNull.Value ? Convert.ToInt32(reader["ResidentID"]) : 0,
                            LastName = reader["LastName"]?.ToString(),
                            FirstName = reader["FirstName"]?.ToString(),
                            MiddleName = reader["MiddleName"]?.ToString(),
                            Address = reader["Address"]?.ToString(),
                            TelCelNo = reader["TelCelNo"]?.ToString(),
                            Sex = reader["Sex"]?.ToString(),
                            DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : DateTime.MinValue,
                            PlaceOfBirth = reader["PlaceOfBirth"]?.ToString(),
                            CivilStatus = reader["CivilStatus"]?.ToString(),
                            VoterIDNo = reader["VoterIDNo"]?.ToString(),
                            PollingPlace = reader["PollingPlace"]?.ToString(),
                            ResidenceType = reader["ResidenceType"]?.ToString(),
                            PaymentAmount = reader["PaymentAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PaymentAmount"]) : 0,
                            PaymentFrequency = reader["PaymentFrequency"]?.ToString(),
                            Height = reader["Height"] != DBNull.Value ? Convert.ToDecimal(reader["Height"]) : 0,
                            Weight = reader["Weight"] != DBNull.Value ? Convert.ToDecimal(reader["Weight"]) : 0,
                            isArchived = reader["isArchived"] != DBNull.Value ? Convert.ToBoolean(reader["isArchived"]) : false
                        };
                        residents.Add(resident);
                    }
                }

                // I FUCKING FORGOT TO ADD PURPOSES
                if (residents.Count > 0)
                {
                    var residentIds = residents.Select(r => r.ResidentID).ToList();
                    string purposesSql = $@"
                        SELECT rp.ResidentPurposeID, rp.ResidentID, rp.PurposeTypeID, rp.PurposeOthers, pt.PurposeName
                        FROM ResidentPurposes rp
                        INNER JOIN PurposeTypes pt ON rp.PurposeTypeID = pt.PurposeTypeID
                        WHERE rp.ResidentID IN ({string.Join(",", residentIds)})
                    ";
                    using (SqlCommand purposesCmd = new SqlCommand(purposesSql, conn))
                    using (SqlDataReader purposesReader = purposesCmd.ExecuteReader())
                    {
                        var purposesDict = new Dictionary<int, List<ResidentPurpose>>();
                        while (purposesReader.Read())
                        {
                            int resId = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentID"));
                            var purpose = new ResidentPurpose
                            {
                                ResidentPurposeID = purposesReader.GetInt32(purposesReader.GetOrdinal("ResidentPurposeID")),
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
                        foreach (var r in residents)
                            if (purposesDict.ContainsKey(r.ResidentID))
                                r.Purposes = purposesDict[r.ResidentID];
                    }
                }
                // Hate.A
            }
            return residents;
        }
        public void RestoreResident(int residentId)
        {
            using (var conn = new SqlConnection(_repoconn))
            using (var cmd = new SqlCommand("UPDATE Residents SET isArchived = 0 WHERE ResidentID = @ID", conn))
            {
                cmd.Parameters.AddWithValue("@ID", residentId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}