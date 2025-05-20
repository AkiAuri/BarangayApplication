using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BarangayApplication.Models;
using static BarangayApplication.LoginMenu;
namespace BarangayApplication.Models.Repositories
{
    public class ResidentsRepository
    {
        private readonly string _repoconn = "Data Source=.;Initial Catalog=BarangayDatabase;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

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
                    string sql = @"
                        SELECT 
                            SUM(CASE WHEN PurposeResidency = 1 THEN 1 ELSE 0 END) AS Residency,
                            SUM(CASE WHEN PurposePostalID = 1 THEN 1 ELSE 0 END) AS PostalID,
                            SUM(CASE WHEN PurposeLocalEmployment = 1 THEN 1 ELSE 0 END) AS LocalEmployment,
                            SUM(CASE WHEN PurposeMarriage = 1 THEN 1 ELSE 0 END) AS Marriage,
                            SUM(CASE WHEN PurposeLoan = 1 THEN 1 ELSE 0 END) AS Loan,
                            SUM(CASE WHEN PurposeMeralco = 1 THEN 1 ELSE 0 END) AS Meralco,
                            SUM(CASE WHEN PurposeBankTransaction = 1 THEN 1 ELSE 0 END) AS BankTransaction,
                            SUM(CASE WHEN PurposeTravelAbroad = 1 THEN 1 ELSE 0 END) AS TravelAbroad,
                            SUM(CASE WHEN PurposeSeniorCitizen = 1 THEN 1 ELSE 0 END) AS SeniorCitizen,
                            SUM(CASE WHEN PurposeSchool = 1 THEN 1 ELSE 0 END) AS School,
                            SUM(CASE WHEN PurposeMedical = 1 THEN 1 ELSE 0 END) AS Medical,
                            SUM(CASE WHEN PurposeBurial = 1 THEN 1 ELSE 0 END) AS Burial,
                            SUM(CASE WHEN PurposeOthers IS NOT NULL AND PurposeOthers != '' THEN 1 ELSE 0 END) AS Others
                        FROM Purposes";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        if (_reader.Read())
                        {
                            purposeDistribution["Residency"] = _reader.IsDBNull(0) ? 0 : _reader.GetInt32(0);
                            purposeDistribution["Postal ID"] = _reader.IsDBNull(1) ? 0 : _reader.GetInt32(1);
                            purposeDistribution["Local Employment"] = _reader.IsDBNull(2) ? 0 : _reader.GetInt32(2);
                            purposeDistribution["Marriage"] = _reader.IsDBNull(3) ? 0 : _reader.GetInt32(3);
                            purposeDistribution["Loan"] = _reader.IsDBNull(4) ? 0 : _reader.GetInt32(4);
                            purposeDistribution["Meralco"] = _reader.IsDBNull(5) ? 0 : _reader.GetInt32(5);
                            purposeDistribution["Bank Transaction"] = _reader.IsDBNull(6) ? 0 : _reader.GetInt32(6);
                            purposeDistribution["Travel Abroad"] = _reader.IsDBNull(7) ? 0 : _reader.GetInt32(7);
                            purposeDistribution["Senior Citizen"] = _reader.IsDBNull(8) ? 0 : _reader.GetInt32(8);
                            purposeDistribution["School"] = _reader.IsDBNull(9) ? 0 : _reader.GetInt32(9);
                            purposeDistribution["Medical"] = _reader.IsDBNull(10) ? 0 : _reader.GetInt32(10);
                            purposeDistribution["Burial"] = _reader.IsDBNull(11) ? 0 : _reader.GetInt32(11);
                            purposeDistribution["Others"] = _reader.IsDBNull(12) ? 0 : _reader.GetInt32(12);
                        }
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
                    string sql = "SELECT Age FROM Residents";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            int age = _reader.IsDBNull(0) ? 0 : _reader.GetInt32(0);

                            if (age >= 0 && age <= 30)
                                ageGroups["Youth (18-30)"]++;
                            else if (age >= 31 && age <= 59)
                                ageGroups["Adults (31-59)"]++;
                            else if (age >= 60)
                                ageGroups["Seniors (65+)"]++;
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
        // ...existing code...

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

                    // Check if a similar log entry already exists to prevent duplication
                    string checkSql = @"
                        SELECT COUNT(*) 
                        FROM UserLogs 
                        WHERE UserName = @UserName AND Action = @Action AND Description = @Description AND DATEDIFF(SECOND, Timestamp, @Timestamp) < 5";

                    using (SqlCommand checkCmd = new SqlCommand(checkSql, _conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserName", accountName);
                        checkCmd.Parameters.AddWithValue("@Action", action);
                        checkCmd.Parameters.AddWithValue("@Description", description);
                        checkCmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

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
                        insertCmd.Parameters.AddWithValue("@Description", description);
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

        public List<Residents> GetApplicants()
        {
            var residents = new List<Residents>();

            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    // Query only the Residents table and join Employment, Spouse, and Purposes
                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.Sex, 
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.Age, 
                            r.PlaceOfBirth, 
                            r.CivilStatus, 
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceType, 
                            r.PaymentAmount, 
                            r.PaymentFrequency,

                            -- Employment
                            e.EmploymentID,
                            e.Company, 
                            e.Position, 
                            e.LengthOfService, 
                            e.PreviousCompany, 
                            e.PreviousPosition, 
                            e.PreviousLengthOfService,

                            -- Spouse
                            s.SpouseID,
                            s.SpouseName, 
                            s.SpousePhone, 
                            s.SpouseCompany, 
                            s.SpousePosition, 
                            s.SpouseLengthOfService, 
                            s.SpousePreviousCompany, 
                            s.SpousePreviousPosition, 
                            s.SpousePreviousLengthOfService,

                            -- Purposes
                            p.PurposeID,
                            p.PurposeResidency, 
                            p.PurposePostalID, 
                            p.PurposeLocalEmployment, 
                            p.PurposeMarriage, 
                            p.PurposeLoan, 
                            p.PurposeMeralco, 
                            p.PurposeBankTransaction, 
                            p.PurposeTravelAbroad, 
                            p.PurposeSeniorCitizen, 
                            p.PurposeSchool, 
                            p.PurposeMedical, 
                            p.PurposeBurial, 
                            p.PurposeOthers

                        FROM Residents r
                        LEFT JOIN Employment e ON r.ResidentID = e.ResidentID
                        LEFT JOIN Spouse s ON r.ResidentID = s.ResidentID
                        LEFT JOIN Purposes p ON r.ResidentID = p.ResidentID
                        ORDER BY r.ResidentID DESC;";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            var resident = new Residents
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
                                Age = _reader.IsDBNull(_reader.GetOrdinal("Age")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("Age")),
                                PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth")),
                                CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus")),
                                VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo")),
                                PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace")),
                                ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType")),
                                PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount")),
                                PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? "" : _reader.GetString(_reader.GetOrdinal("PaymentFrequency")),
                            };

                            // Employment info (may be null)
                            if (!_reader.IsDBNull(_reader.GetOrdinal("EmploymentID")))
                            {
                                resident.Employment = new Employment
                                {
                                    EmploymentID = _reader.GetInt32(_reader.GetOrdinal("EmploymentID")),
                                    ResidentID = resident.ResidentID,
                                    Company = _reader.IsDBNull(_reader.GetOrdinal("Company")) ? "" : _reader.GetString(_reader.GetOrdinal("Company")),
                                    Position = _reader.IsDBNull(_reader.GetOrdinal("Position")) ? "" : _reader.GetString(_reader.GetOrdinal("Position")),
                                    LengthOfService = _reader.IsDBNull(_reader.GetOrdinal("LengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("LengthOfService")),
                                    PreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("PreviousCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousCompany")),
                                    PreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("PreviousPosition")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousPosition")),
                                    PreviousLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("PreviousLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousLengthOfService"))
                                };
                            }

                            // Spouse info (may be null)
                            if (!_reader.IsDBNull(_reader.GetOrdinal("SpouseID")))
                            {
                                resident.Spouse = new Spouse
                                {
                                    SpouseID = _reader.GetInt32(_reader.GetOrdinal("SpouseID")),
                                    ResidentID = resident.ResidentID,
                                    SpouseName = _reader.IsDBNull(_reader.GetOrdinal("SpouseName")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseName")),
                                    SpousePhone = _reader.IsDBNull(_reader.GetOrdinal("SpousePhone")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePhone")),
                                    SpouseCompany = _reader.IsDBNull(_reader.GetOrdinal("SpouseCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseCompany")),
                                    SpousePosition = _reader.IsDBNull(_reader.GetOrdinal("SpousePosition")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePosition")),
                                    SpouseLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SpouseLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseLengthOfService")),
                                    SpousePreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousCompany")),
                                    SpousePreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousPosition")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousPosition")),
                                    SpousePreviousLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousLengthOfService"))
                                };
                            }

                            // Purposes info (may be null)
                            if (!_reader.IsDBNull(_reader.GetOrdinal("PurposeID")))
                            {
                                resident.Purposes = new Purposes
                                {
                                    PurposeID = _reader.GetInt32(_reader.GetOrdinal("PurposeID")),
                                    ResidentID = resident.ResidentID,
                                    PurposeResidency = !_reader.IsDBNull(_reader.GetOrdinal("PurposeResidency")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeResidency")),
                                    PurposePostalID = !_reader.IsDBNull(_reader.GetOrdinal("PurposePostalID")) && _reader.GetBoolean(_reader.GetOrdinal("PurposePostalID")),
                                    PurposeLocalEmployment = !_reader.IsDBNull(_reader.GetOrdinal("PurposeLocalEmployment")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeLocalEmployment")),
                                    PurposeMarriage = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMarriage")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMarriage")),
                                    PurposeLoan = !_reader.IsDBNull(_reader.GetOrdinal("PurposeLoan")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeLoan")),
                                    PurposeMeralco = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMeralco")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMeralco")),
                                    PurposeBankTransaction = !_reader.IsDBNull(_reader.GetOrdinal("PurposeBankTransaction")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeBankTransaction")),
                                    PurposeTravelAbroad = !_reader.IsDBNull(_reader.GetOrdinal("PurposeTravelAbroad")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeTravelAbroad")),
                                    PurposeSeniorCitizen = !_reader.IsDBNull(_reader.GetOrdinal("PurposeSeniorCitizen")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeSeniorCitizen")),
                                    PurposeSchool = !_reader.IsDBNull(_reader.GetOrdinal("PurposeSchool")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeSchool")),
                                    PurposeMedical = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMedical")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMedical")),
                                    PurposeBurial = !_reader.IsDBNull(_reader.GetOrdinal("PurposeBurial")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeBurial")),
                                    PurposeOthers = _reader.IsDBNull(_reader.GetOrdinal("PurposeOthers")) ? "" : _reader.GetString(_reader.GetOrdinal("PurposeOthers"))
                                };
                            }

                            residents.Add(resident);
                        }
                    }
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
        public Residents? GetApplicant(int residentId)
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    _conn.Open();

                    string sql = @"
                        SELECT 
                            r.ResidentID, 
                            r.LastName, 
                            r.FirstName, 
                            r.MiddleName, 
                            r.Address, 
                            r.TelCelNo, 
                            r.Sex, 
                            r.Height, 
                            r.Weight, 
                            r.DateOfBirth, 
                            r.Age, 
                            r.PlaceOfBirth, 
                            r.CivilStatus, 
                            r.VoterIDNo, 
                            r.PollingPlace,
                            r.ResidenceType, 
                            r.PaymentAmount, 
                            r.PaymentFrequency,

                            -- Employment
                            e.EmploymentID,
                            e.Company, 
                            e.Position, 
                            e.LengthOfService, 
                            e.PreviousCompany, 
                            e.PreviousPosition, 
                            e.PreviousLengthOfService,

                            -- Spouse
                            s.SpouseID,
                            s.SpouseName, 
                            s.SpousePhone, 
                            s.SpouseCompany, 
                            s.SpousePosition, 
                            s.SpouseLengthOfService, 
                            s.SpousePreviousCompany, 
                            s.SpousePreviousPosition, 
                            s.SpousePreviousLengthOfService,

                            -- Purposes
                            p.PurposeID,
                            p.PurposeResidency, 
                            p.PurposePostalID, 
                            p.PurposeLocalEmployment, 
                            p.PurposeMarriage, 
                            p.PurposeLoan, 
                            p.PurposeMeralco, 
                            p.PurposeBankTransaction, 
                            p.PurposeTravelAbroad, 
                            p.PurposeSeniorCitizen, 
                            p.PurposeSchool, 
                            p.PurposeMedical, 
                            p.PurposeBurial, 
                            p.PurposeOthers

                        FROM Residents r
                        LEFT JOIN Employment e ON r.ResidentID = e.ResidentID
                        LEFT JOIN Spouse s ON r.ResidentID = s.ResidentID
                        LEFT JOIN Purposes p ON r.ResidentID = p.ResidentID
                        WHERE r.ResidentID = @residentId;";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        _cmd.Parameters.AddWithValue("@residentId", residentId);

                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            if (_reader.Read())
                            {
                                Residents resident = new Residents();

                                // Residents fields
                                resident.ResidentID = _reader.GetInt32(_reader.GetOrdinal("ResidentID"));
                                resident.LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? "" : _reader.GetString(_reader.GetOrdinal("LastName"));
                                resident.FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? "" : _reader.GetString(_reader.GetOrdinal("FirstName"));
                                resident.MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName"));
                                resident.Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? "" : _reader.GetString(_reader.GetOrdinal("Address"));
                                resident.TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? "" : _reader.GetString(_reader.GetOrdinal("TelCelNo"));
                                resident.Sex = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? "" : _reader.GetString(_reader.GetOrdinal("Sex"));
                                resident.Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height"));
                                resident.Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight"));
                                resident.DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth"));
                                resident.Age = _reader.IsDBNull(_reader.GetOrdinal("Age")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("Age"));
                                resident.PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? "" : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth"));
                                resident.CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? "" : _reader.GetString(_reader.GetOrdinal("CivilStatus"));
                                resident.VoterIDNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? "" : _reader.GetString(_reader.GetOrdinal("VoterIDNo"));
                                resident.PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? "" : _reader.GetString(_reader.GetOrdinal("PollingPlace"));
                                resident.ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? "" : _reader.GetString(_reader.GetOrdinal("ResidenceType"));
                                resident.PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount"));
                                resident.PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? "" : _reader.GetString(_reader.GetOrdinal("PaymentFrequency"));

                                // Employment (if exists)
                                if (!_reader.IsDBNull(_reader.GetOrdinal("EmploymentID")))
                                {
                                    resident.Employment = new Employment
                                    {
                                        EmploymentID = _reader.GetInt32(_reader.GetOrdinal("EmploymentID")),
                                        ResidentID = resident.ResidentID,
                                        Company = _reader.IsDBNull(_reader.GetOrdinal("Company")) ? "" : _reader.GetString(_reader.GetOrdinal("Company")),
                                        Position = _reader.IsDBNull(_reader.GetOrdinal("Position")) ? "" : _reader.GetString(_reader.GetOrdinal("Position")),
                                        LengthOfService = _reader.IsDBNull(_reader.GetOrdinal("LengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("LengthOfService")),
                                        PreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("PreviousCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousCompany")),
                                        PreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("PreviousPosition")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousPosition")),
                                        PreviousLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("PreviousLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("PreviousLengthOfService"))
                                    };
                                }

                                // Spouse (if exists)
                                if (!_reader.IsDBNull(_reader.GetOrdinal("SpouseID")))
                                {
                                    resident.Spouse = new Spouse
                                    {
                                        SpouseID = _reader.GetInt32(_reader.GetOrdinal("SpouseID")),
                                        ResidentID = resident.ResidentID,
                                        SpouseName = _reader.IsDBNull(_reader.GetOrdinal("SpouseName")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseName")),
                                        SpousePhone = _reader.IsDBNull(_reader.GetOrdinal("SpousePhone")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePhone")),
                                        SpouseCompany = _reader.IsDBNull(_reader.GetOrdinal("SpouseCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseCompany")),
                                        SpousePosition = _reader.IsDBNull(_reader.GetOrdinal("SpousePosition")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePosition")),
                                        SpouseLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SpouseLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("SpouseLengthOfService")),
                                        SpousePreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousCompany")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousCompany")),
                                        SpousePreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousPosition")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousPosition")),
                                        SpousePreviousLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SpousePreviousLengthOfService")) ? "" : _reader.GetString(_reader.GetOrdinal("SpousePreviousLengthOfService"))
                                    };
                                }

                                // Purposes (if exists)
                                if (!_reader.IsDBNull(_reader.GetOrdinal("PurposeID")))
                                {
                                    resident.Purposes = new Purposes
                                    {
                                        PurposeID = _reader.GetInt32(_reader.GetOrdinal("PurposeID")),
                                        ResidentID = resident.ResidentID,
                                        PurposeResidency = !_reader.IsDBNull(_reader.GetOrdinal("PurposeResidency")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeResidency")),
                                        PurposePostalID = !_reader.IsDBNull(_reader.GetOrdinal("PurposePostalID")) && _reader.GetBoolean(_reader.GetOrdinal("PurposePostalID")),
                                        PurposeLocalEmployment = !_reader.IsDBNull(_reader.GetOrdinal("PurposeLocalEmployment")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeLocalEmployment")),
                                        PurposeMarriage = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMarriage")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMarriage")),
                                        PurposeLoan = !_reader.IsDBNull(_reader.GetOrdinal("PurposeLoan")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeLoan")),
                                        PurposeMeralco = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMeralco")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMeralco")),
                                        PurposeBankTransaction = !_reader.IsDBNull(_reader.GetOrdinal("PurposeBankTransaction")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeBankTransaction")),
                                        PurposeTravelAbroad = !_reader.IsDBNull(_reader.GetOrdinal("PurposeTravelAbroad")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeTravelAbroad")),
                                        PurposeSeniorCitizen = !_reader.IsDBNull(_reader.GetOrdinal("PurposeSeniorCitizen")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeSeniorCitizen")),
                                        PurposeSchool = !_reader.IsDBNull(_reader.GetOrdinal("PurposeSchool")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeSchool")),
                                        PurposeMedical = !_reader.IsDBNull(_reader.GetOrdinal("PurposeMedical")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeMedical")),
                                        PurposeBurial = !_reader.IsDBNull(_reader.GetOrdinal("PurposeBurial")) && _reader.GetBoolean(_reader.GetOrdinal("PurposeBurial")),
                                        PurposeOthers = _reader.IsDBNull(_reader.GetOrdinal("PurposeOthers")) ? "" : _reader.GetString(_reader.GetOrdinal("PurposeOthers"))
                                    };
                                }

                                return resident;
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

            return null;
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
        public void CreateResident(Residents resident)
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
                                    Weight, DateOfBirth, Age, PlaceOfBirth, CivilStatus, VoterIDNo, PollingPlace,
                                    ResidenceType, PaymentAmount, PaymentFrequency
                                ) VALUES (
                                    @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @Sex, @Height, 
                                    @Weight, @DateOfBirth, @Age, @PlaceOfBirth, @CivilStatus, @VoterIDNo, @PollingPlace,
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
                                cmd.Parameters.AddWithValue("@Age", resident.Age);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                                cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                                cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);

                                residentId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Insert into Employment table (if data provided)
                            if (resident.Employment != null)
                            {
                                string sqlEmployment = @"
                                    INSERT INTO Employment (
                                        ResidentID, Company, Position, LengthOfService, PreviousCompany, PreviousPosition, PreviousLengthOfService
                                    ) VALUES (
                                        @ResidentID, @Company, @Position, @LengthOfService, @PreviousCompany, @PreviousPosition, @PreviousLengthOfService
                                    );";
                                using (SqlCommand cmd = new SqlCommand(sqlEmployment, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.Parameters.AddWithValue("@Company", resident.Employment.Company);
                                    cmd.Parameters.AddWithValue("@Position", resident.Employment.Position);
                                    cmd.Parameters.AddWithValue("@LengthOfService", resident.Employment.LengthOfService);
                                    cmd.Parameters.AddWithValue("@PreviousCompany", resident.Employment.PreviousCompany);
                                    cmd.Parameters.AddWithValue("@PreviousPosition", resident.Employment.PreviousPosition);
                                    cmd.Parameters.AddWithValue("@PreviousLengthOfService", resident.Employment.PreviousLengthOfService);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 3. Insert into Spouse table (if data provided)
                            if (resident.Spouse != null)
                            {
                                string sqlSpouse = @"
                                    INSERT INTO Spouse (
                                        ResidentID, SpouseName, SpousePhone, SpouseCompany, SpousePosition, SpouseLengthOfService,
                                        SpousePreviousCompany, SpousePreviousPosition, SpousePreviousLengthOfService
                                    ) VALUES (
                                        @ResidentID, @SpouseName, @SpousePhone, @SpouseCompany, @SpousePosition, @SpouseLengthOfService,
                                        @SpousePreviousCompany, @SpousePreviousPosition, @SpousePreviousLengthOfService
                                    );";
                                using (SqlCommand cmd = new SqlCommand(sqlSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    cmd.Parameters.AddWithValue("@SpousePhone", resident.Spouse.SpousePhone);
                                    cmd.Parameters.AddWithValue("@SpouseCompany", resident.Spouse.SpouseCompany);
                                    cmd.Parameters.AddWithValue("@SpousePosition", resident.Spouse.SpousePosition);
                                    cmd.Parameters.AddWithValue("@SpouseLengthOfService", resident.Spouse.SpouseLengthOfService);
                                    cmd.Parameters.AddWithValue("@SpousePreviousCompany", resident.Spouse.SpousePreviousCompany);
                                    cmd.Parameters.AddWithValue("@SpousePreviousPosition", resident.Spouse.SpousePreviousPosition);
                                    cmd.Parameters.AddWithValue("@SpousePreviousLengthOfService", resident.Spouse.SpousePreviousLengthOfService);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 4. Insert into Purposes table (if data provided)
                            if (resident.Purposes != null)
                            {
                                string sqlPurposes = @"
                                    INSERT INTO Purposes (
                                        ResidentID, PurposeResidency, PurposePostalID, PurposeLocalEmployment, PurposeMarriage,
                                        PurposeLoan, PurposeMeralco, PurposeBankTransaction, PurposeTravelAbroad, PurposeSeniorCitizen,
                                        PurposeSchool, PurposeMedical, PurposeBurial, PurposeOthers
                                    ) VALUES (
                                        @ResidentID, @PurposeResidency, @PurposePostalID, @PurposeLocalEmployment, @PurposeMarriage,
                                        @PurposeLoan, @PurposeMeralco, @PurposeBankTransaction, @PurposeTravelAbroad, @PurposeSeniorCitizen,
                                        @PurposeSchool, @PurposeMedical, @PurposeBurial, @PurposeOthers
                                    );";
                                using (SqlCommand cmd = new SqlCommand(sqlPurposes, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                    cmd.Parameters.AddWithValue("@PurposeResidency", resident.Purposes.PurposeResidency);
                                    cmd.Parameters.AddWithValue("@PurposePostalID", resident.Purposes.PurposePostalID);
                                    cmd.Parameters.AddWithValue("@PurposeLocalEmployment", resident.Purposes.PurposeLocalEmployment);
                                    cmd.Parameters.AddWithValue("@PurposeMarriage", resident.Purposes.PurposeMarriage);
                                    cmd.Parameters.AddWithValue("@PurposeLoan", resident.Purposes.PurposeLoan);
                                    cmd.Parameters.AddWithValue("@PurposeMeralco", resident.Purposes.PurposeMeralco);
                                    cmd.Parameters.AddWithValue("@PurposeBankTransaction", resident.Purposes.PurposeBankTransaction);
                                    cmd.Parameters.AddWithValue("@PurposeTravelAbroad", resident.Purposes.PurposeTravelAbroad);
                                    cmd.Parameters.AddWithValue("@PurposeSeniorCitizen", resident.Purposes.PurposeSeniorCitizen);
                                    cmd.Parameters.AddWithValue("@PurposeSchool", resident.Purposes.PurposeSchool);
                                    cmd.Parameters.AddWithValue("@PurposeMedical", resident.Purposes.PurposeMedical);
                                    cmd.Parameters.AddWithValue("@PurposeBurial", resident.Purposes.PurposeBurial);
                                    cmd.Parameters.AddWithValue("@PurposeOthers", resident.Purposes.PurposeOthers);
                                    cmd.ExecuteNonQuery();
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
                AddUserLog(CurrentUser.AccountID, "Add", $"Added resident: {resident.FirstName} {resident.LastName}");
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
        public void UpdateResident(Residents resident)
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
                                    Age = @Age,
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
                                cmd.Parameters.AddWithValue("@Age", resident.Age);
                                cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                                cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                                cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIDNo);
                                cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                                cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                                cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                                cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Update Employment (if provided)
                            if (resident.Employment != null)
                            {
                                string sqlEmployment = @"
                                    UPDATE Employment SET
                                        Company = @Company,
                                        Position = @Position,
                                        LengthOfService = @LengthOfService,
                                        PreviousCompany = @PreviousCompany,
                                        PreviousPosition = @PreviousPosition,
                                        PreviousLengthOfService = @PreviousLengthOfService
                                    WHERE ResidentID = @ResidentID;
                                ";
                                using (var cmd = new SqlCommand(sqlEmployment, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                    cmd.Parameters.AddWithValue("@Company", resident.Employment.Company);
                                    cmd.Parameters.AddWithValue("@Position", resident.Employment.Position);
                                    cmd.Parameters.AddWithValue("@LengthOfService", resident.Employment.LengthOfService);
                                    cmd.Parameters.AddWithValue("@PreviousCompany", resident.Employment.PreviousCompany);
                                    cmd.Parameters.AddWithValue("@PreviousPosition", resident.Employment.PreviousPosition);
                                    cmd.Parameters.AddWithValue("@PreviousLengthOfService", resident.Employment.PreviousLengthOfService);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 3. Update Spouse (if provided)
                            if (resident.Spouse != null)
                            {
                                string sqlSpouse = @"
                                    UPDATE Spouse SET
                                        SpouseName = @SpouseName,
                                        SpousePhone = @SpousePhone,
                                        SpouseCompany = @SpouseCompany,
                                        SpousePosition = @SpousePosition,
                                        SpouseLengthOfService = @SpouseLengthOfService,
                                        SpousePreviousCompany = @SpousePreviousCompany,
                                        SpousePreviousPosition = @SpousePreviousPosition,
                                        SpousePreviousLengthOfService = @SpousePreviousLengthOfService
                                    WHERE ResidentID = @ResidentID;
                                ";
                                using (var cmd = new SqlCommand(sqlSpouse, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                    cmd.Parameters.AddWithValue("@SpouseName", resident.Spouse.SpouseName);
                                    cmd.Parameters.AddWithValue("@SpousePhone", resident.Spouse.SpousePhone);
                                    cmd.Parameters.AddWithValue("@SpouseCompany", resident.Spouse.SpouseCompany);
                                    cmd.Parameters.AddWithValue("@SpousePosition", resident.Spouse.SpousePosition);
                                    cmd.Parameters.AddWithValue("@SpouseLengthOfService", resident.Spouse.SpouseLengthOfService);
                                    cmd.Parameters.AddWithValue("@SpousePreviousCompany", resident.Spouse.SpousePreviousCompany);
                                    cmd.Parameters.AddWithValue("@SpousePreviousPosition", resident.Spouse.SpousePreviousPosition);
                                    cmd.Parameters.AddWithValue("@SpousePreviousLengthOfService", resident.Spouse.SpousePreviousLengthOfService);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // 4. Update Purposes (if provided)
                            if (resident.Purposes != null)
                            {
                                string sqlPurposes = @"
                                    UPDATE Purposes SET
                                        PurposeResidency = @PurposeResidency,
                                        PurposePostalID = @PurposePostalID,
                                        PurposeLocalEmployment = @PurposeLocalEmployment,
                                        PurposeMarriage = @PurposeMarriage,
                                        PurposeLoan = @PurposeLoan,
                                        PurposeMeralco = @PurposeMeralco,
                                        PurposeBankTransaction = @PurposeBankTransaction,
                                        PurposeTravelAbroad = @PurposeTravelAbroad,
                                        PurposeSeniorCitizen = @PurposeSeniorCitizen,
                                        PurposeSchool = @PurposeSchool,
                                        PurposeMedical = @PurposeMedical,
                                        PurposeBurial = @PurposeBurial,
                                        PurposeOthers = @PurposeOthers
                                    WHERE ResidentID = @ResidentID;
                                ";
                                using (var cmd = new SqlCommand(sqlPurposes, _conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                                    cmd.Parameters.AddWithValue("@PurposeResidency", resident.Purposes.PurposeResidency);
                                    cmd.Parameters.AddWithValue("@PurposePostalID", resident.Purposes.PurposePostalID);
                                    cmd.Parameters.AddWithValue("@PurposeLocalEmployment", resident.Purposes.PurposeLocalEmployment);
                                    cmd.Parameters.AddWithValue("@PurposeMarriage", resident.Purposes.PurposeMarriage);
                                    cmd.Parameters.AddWithValue("@PurposeLoan", resident.Purposes.PurposeLoan);
                                    cmd.Parameters.AddWithValue("@PurposeMeralco", resident.Purposes.PurposeMeralco);
                                    cmd.Parameters.AddWithValue("@PurposeBankTransaction", resident.Purposes.PurposeBankTransaction);
                                    cmd.Parameters.AddWithValue("@PurposeTravelAbroad", resident.Purposes.PurposeTravelAbroad);
                                    cmd.Parameters.AddWithValue("@PurposeSeniorCitizen", resident.Purposes.PurposeSeniorCitizen);
                                    cmd.Parameters.AddWithValue("@PurposeSchool", resident.Purposes.PurposeSchool);
                                    cmd.Parameters.AddWithValue("@PurposeMedical", resident.Purposes.PurposeMedical);
                                    cmd.Parameters.AddWithValue("@PurposeBurial", resident.Purposes.PurposeBurial);
                                    cmd.Parameters.AddWithValue("@PurposeOthers", resident.Purposes.PurposeOthers);
                                    cmd.ExecuteNonQuery();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Deletes a resident record and all related Employment, Spouse, and Purposes records from the database based on the provided residentId.
        /// The method establishes a database connection, creates DELETE SQL commands with parameters for each related table,
        /// and executes the commands within a transaction to ensure referential integrity.
        /// </summary>
        /// <param name="residentId">The unique identifier of the resident to be deleted.</param>
        public void DeleteResident(int residentId)
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
                            // Delete from child tables first, then from Residents
                            string sqlEmployment = "DELETE FROM Employment WHERE ResidentID = @ResidentID;";
                            string sqlSpouse = "DELETE FROM Spouse WHERE ResidentID = @ResidentID;";
                            string sqlPurposes = "DELETE FROM Purposes WHERE ResidentID = @ResidentID;";
                            string sqlResident = "DELETE FROM Residents WHERE ResidentID = @ResidentID;";

                            using (SqlCommand cmd = new SqlCommand(sqlEmployment, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand(sqlSpouse, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand(sqlPurposes, _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ResidentID", residentId);
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand(sqlResident, _conn, transaction))
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw;
            }
        }
    }
    
}