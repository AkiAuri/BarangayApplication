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
        private readonly string _repoconn = "Data Source=.;Initial Catalog=BrgyDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

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
                FROM SVResidents";

                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        if (_reader.Read())
                        {
                            purposeDistribution["Residency"] = _reader.GetInt32(0);
                            purposeDistribution["Postal ID"] = _reader.GetInt32(1);
                            purposeDistribution["Local Employment"] = _reader.GetInt32(2);
                            purposeDistribution["Marriage"] = _reader.GetInt32(3);
                            purposeDistribution["Loan"] = _reader.GetInt32(4);
                            purposeDistribution["Meralco"] = _reader.GetInt32(5);
                            purposeDistribution["Bank Transaction"] = _reader.GetInt32(6);
                            purposeDistribution["Travel Abroad"] = _reader.GetInt32(7);
                            purposeDistribution["Senior Citizen"] = _reader.GetInt32(8);
                            purposeDistribution["School"] = _reader.GetInt32(9);
                            purposeDistribution["Medical"] = _reader.GetInt32(10);
                            purposeDistribution["Burial"] = _reader.GetInt32(11);
                            purposeDistribution["Others"] = _reader.GetInt32(12);
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
                    string sql = "SELECT Sex, COUNT(*) AS Count FROM SVResidents GROUP BY Sex";

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
                    string sql = "SELECT Age FROM SVResidents";

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
            // Initialize the list to hold residents.
            var residents = new List<Residents>();

            try
            {
                // Create a new SqlConnection using the connection string _repoconn.
                // The 'using' statement ensures that the connection is properly disposed of after use.
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    // Open the connection to the database.
                    _conn.Open();
                    
                    // Define the SQL query that selects all columns from the SVResidents table.
                    // The query orders the results by ApplicantID in descending order.
                    string sql = @"
                            SELECT 
                                ApplicantID, 
                                LastName, 
                                FirstName, 
                                MiddleName, 
                                Address, 
                                TelCelNo, 
                                Sex, 
                                Height, 
                                Weight, 
                                DateOfBirth, 
                                Age, 
                                PlaceOfBirth, 
                                CivilStatus, 
                                VoterIDNo, 
                                PollingPlace,
                                ResidenceType, 
                                PaymentAmount, 
                                PaymentFrequency,
                                Company, 
                                Position, 
                                LengthofService, 
                                PreviousCompany, 
                                PreviousPosition, 
                                PreviousLengthofService,
                                NameOfSpouse, 
                                SpousePhone, 
                                SCompany, 
                                SPosition, 
                                SLengthOfService, 
                                SPreviousCompany, 
                                SPreviousPosition, 
                                SPreviousLengthofService,
                                ChildrenRelativeName, 
                                ChildrenRelativeAge, 
                                ChildrenRelativeOccupation, 
                                ChildrenRelativeRelationship,
                                PurposeResidency, 
                                PurposePostalID, 
                                PurposeLocalEmployment, 
                                PurposeMarriage, 
                                PurposeLoan, 
                                PurposeMeralco, 
                                PurposeBankTransaction, 
                                PurposeTravelAbroad, 
                                PurposeSeniorCitizen, 
                                PurposeSchool, 
                                PurposeMedical, 
                                PurposeBurial, 
                                PurposeOthers
                            FROM SVResidents
                            ORDER BY ApplicantID DESC;";

                    // Create a SqlCommand object with the SQL query and the open connection.
                    // The 'using' statement ensures that the SqlCommand is disposed after use.
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        // Execute the SQL query and retrieve the results using a SqlDataReader.
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            // Loop through each record in the result set.
                            while (_reader.Read())
                            {
                                // Create a new Residents object.
                                Residents resident = new Residents();

                                // Map each column to the corresponding property in the Residents object.
                                // Checks if each column is null using IsDBNull and sets a default value if needed.
                                resident.Id = _reader.GetInt32(_reader.GetOrdinal("ApplicantID"));
                                resident.LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? null : _reader.GetString(_reader.GetOrdinal("LastName"));
                                resident.FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? null : _reader.GetString(_reader.GetOrdinal("FirstName"));
                                resident.MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName"));
                                resident.Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? null : _reader.GetString(_reader.GetOrdinal("Address"));
                                resident.TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? null : _reader.GetString(_reader.GetOrdinal("TelCelNo"));
                                resident.Sex = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? null : _reader.GetString(_reader.GetOrdinal("Sex"));
                                resident.Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height"));
                                resident.Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight"));
                                resident.DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth"));
                                resident.Age = _reader.IsDBNull(_reader.GetOrdinal("Age")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("Age"));
                                resident.PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? null : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth"));
                                resident.CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? null : _reader.GetString(_reader.GetOrdinal("CivilStatus"));
                                resident.VoterIdNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? null : _reader.GetString(_reader.GetOrdinal("VoterIDNo"));
                                resident.PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? null : _reader.GetString(_reader.GetOrdinal("PollingPlace"));
                                resident.ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? null : _reader.GetString(_reader.GetOrdinal("ResidenceType"));
                                resident.PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount"));
                                resident.PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? null : _reader.GetString(_reader.GetOrdinal("PaymentFrequency"));
                                resident.Company = _reader.IsDBNull(_reader.GetOrdinal("Company")) ? null : _reader.GetString(_reader.GetOrdinal("Company"));
                                resident.Position = _reader.IsDBNull(_reader.GetOrdinal("Position")) ? null : _reader.GetString(_reader.GetOrdinal("Position"));
                                resident.LengthofService = _reader.IsDBNull(_reader.GetOrdinal("LengthofService")) ? null : _reader.GetString(_reader.GetOrdinal("LengthofService"));
                                resident.PreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("PreviousCompany")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousCompany"));
                                resident.PreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("PreviousPosition")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousPosition"));
                                resident.PreviousLengthofService = _reader.IsDBNull(_reader.GetOrdinal("PreviousLengthofService")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousLengthofService"));
                                resident.SpouseName = _reader.IsDBNull(_reader.GetOrdinal("NameOfSpouse")) ? null : _reader.GetString(_reader.GetOrdinal("NameOfSpouse"));
                                resident.SpousePhone = _reader.IsDBNull(_reader.GetOrdinal("SpousePhone")) ? null : _reader.GetString(_reader.GetOrdinal("SpousePhone"));
                                resident.SpouseCompany = _reader.IsDBNull(_reader.GetOrdinal("SCompany")) ? null : _reader.GetString(_reader.GetOrdinal("SCompany"));
                                resident.SpousePosition = _reader.IsDBNull(_reader.GetOrdinal("SPosition")) ? null : _reader.GetString(_reader.GetOrdinal("SPosition"));
                                resident.SpouseLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SLengthOfService")) ? null : _reader.GetString(_reader.GetOrdinal("SLengthOfService"));
                                resident.SpousePrevCompany = _reader.IsDBNull(_reader.GetOrdinal("SPreviousCompany")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousCompany"));
                                resident.SpousePrevPosition = _reader.IsDBNull(_reader.GetOrdinal("SPreviousPosition")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousPosition"));
                                resident.SpousePrevLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SPreviousLengthOfService")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousLengthOfService"));
                                resident.ChildrenRelativeName = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeName")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeName"));
                                resident.ChildrenRelativeAge = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeAge")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeAge"));
                                resident.ChildrenRelativeOccupation = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeOccupation")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeOccupation"));
                                resident.ChildrenRelativeRelationship = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeRelationship")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeRelationship"));
                                resident.PurposeResidency = _reader.IsDBNull(_reader.GetOrdinal("PurposeResidency")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeResidency"));
                                resident.PurposePostalID = _reader.IsDBNull(_reader.GetOrdinal("PurposePostalID")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposePostalID"));
                                resident.PurposeLocalEmployment = _reader.IsDBNull(_reader.GetOrdinal("PurposeLocalEmployment")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeLocalEmployment"));
                                resident.PurposeMarriage = _reader.IsDBNull(_reader.GetOrdinal("PurposeMarriage")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMarriage"));
                                resident.PurposeLoan = _reader.IsDBNull(_reader.GetOrdinal("PurposeLoan")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeLoan"));
                                resident.PurposeMeralco = _reader.IsDBNull(_reader.GetOrdinal("PurposeMeralco")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMeralco"));
                                resident.PurposeBankTransaction = _reader.IsDBNull(_reader.GetOrdinal("PurposeBankTransaction")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeBankTransaction"));
                                resident.PurposeTravelAbroad = _reader.IsDBNull(_reader.GetOrdinal("PurposeTravelAbroad")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeTravelAbroad"));
                                resident.PurposeSeniorCitizen = _reader.IsDBNull(_reader.GetOrdinal("PurposeSeniorCitizen")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeSeniorCitizen"));
                                resident.PurposeSchool = _reader.IsDBNull(_reader.GetOrdinal("PurposeSchool")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeSchool"));
                                resident.PurposeMedical = _reader.IsDBNull(_reader.GetOrdinal("PurposeMedical")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMedical"));
                                resident.PurposeBurial = _reader.IsDBNull(_reader.GetOrdinal("PurposeBurial")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeBurial"));
                                resident.PurposeOthers = _reader.IsDBNull(_reader.GetOrdinal("PurposeOthers")) ? null : _reader.GetString(_reader.GetOrdinal("PurposeOthers"));

                                // Add the populated resident object to the residents list.
                                residents.Add(resident);
                            }
                        }
                    }
                }
            }
            // Handle any exceptions that occur during the database operation.
            catch (Exception ex)
            {
                // Log the exception details to the console.
                Console.WriteLine("Exception: " + ex.ToString());
                // Rethrow the exception so higher-level code can handle it as needed.
                throw;
            }
            
            // Return the complete list of residents.
            return residents;
        }
        
        /// <summary>
        /// Retrieves a single applicant resident by the provided applicantId.
        /// The method connects to the database, executes a SELECT query against the SVResidents table 
        /// with a filter on ApplicantID, maps the result to a Residents instance, and returns it.
        /// If no matching record is found, the method returns null.
        /// In case of an exception, the error is logged to the console and rethrown.
        /// </summary>
        /// <param name="applicantId">The ID of the applicant to be retrieved.</param>
        /// <returns>A Residents object if found; otherwise, null.</returns>
        public Residents? GetApplicant(int applicantId)
        {
            try
            {
                // Establish a connection to the database using the connection string _repoconn.
                // The 'using' block ensures that the connection is properly disposed when done.
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    // Open the database connection.
                    _conn.Open();

                    // Define the SQL query to select the applicant record based on the applicantId.
                    // The query selects all required fields from the SVResidents table.
                    string sql = @"
                        SELECT 
                            ApplicantID, 
                            LastName, 
                            FirstName, 
                            MiddleName, 
                            Address, 
                            TelCelNo, 
                            Sex, 
                            Height, 
                            Weight, 
                            DateOfBirth, 
                            Age, 
                            PlaceOfBirth, 
                            CivilStatus, 
                            VoterIDNo, 
                            PollingPlace,
                            ResidenceType, 
                            PaymentAmount, 
                            PaymentFrequency,
                            Company, 
                            Position, 
                            LengthofService, 
                            PreviousCompany, 
                            PreviousPosition, 
                            PreviousLengthOfService,
                            NameOfSpouse, 
                            SpousePhone, 
                            SCompany, 
                            SPosition, 
                            SLengthOfService, 
                            SPreviousCompany, 
                            SPreviousPosition, 
                            SPreviousLengthOfService,
                            ChildrenRelativeName, 
                            ChildrenRelativeAge, 
                            ChildrenRelativeOccupation, 
                            ChildrenRelativeRelationship,
                            PurposeResidency, 
                            PurposePostalID, 
                            PurposeLocalEmployment, 
                            PurposeMarriage, 
                            PurposeLoan, 
                            PurposeMeralco, 
                            PurposeBankTransaction, 
                            PurposeTravelAbroad, 
                            PurposeSeniorCitizen, 
                            PurposeSchool, 
                            PurposeMedical, 
                            PurposeBurial, 
                            PurposeOthers
                        FROM SVResidents
                        WHERE ApplicantID = @applicantId;";

                    // Create a SqlCommand with the SQL query and the connection.
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        // Add the applicantId parameter to the command.
                        _cmd.Parameters.AddWithValue("@applicantId", applicantId);

                        // Execute the command and use a SqlDataReader to read the results.
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            // If a record is returned, map the result to a Residents object.
                            if (_reader.Read())
                            {
                                Residents resident = new Residents();

                                // Map each column from the result set to the corresponding property of the Residents object.
                                // For each field, a null check is performed using IsDBNull to handle potential nulls.
                                resident.Id = _reader.GetInt32(_reader.GetOrdinal("ApplicantID"));
                                resident.LastName = _reader.IsDBNull(_reader.GetOrdinal("LastName")) ? null : _reader.GetString(_reader.GetOrdinal("LastName"));
                                resident.FirstName = _reader.IsDBNull(_reader.GetOrdinal("FirstName")) ? null : _reader.GetString(_reader.GetOrdinal("FirstName"));
                                resident.MiddleName = _reader.IsDBNull(_reader.GetOrdinal("MiddleName")) ? null : _reader.GetString(_reader.GetOrdinal("MiddleName"));
                                resident.Address = _reader.IsDBNull(_reader.GetOrdinal("Address")) ? null : _reader.GetString(_reader.GetOrdinal("Address"));
                                resident.TelCelNo = _reader.IsDBNull(_reader.GetOrdinal("TelCelNo")) ? null : _reader.GetString(_reader.GetOrdinal("TelCelNo"));
                                resident.Sex = _reader.IsDBNull(_reader.GetOrdinal("Sex")) ? null : _reader.GetString(_reader.GetOrdinal("Sex"));
                                resident.Height = _reader.IsDBNull(_reader.GetOrdinal("Height")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Height"));
                                resident.Weight = _reader.IsDBNull(_reader.GetOrdinal("Weight")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("Weight"));
                                resident.DateOfBirth = _reader.IsDBNull(_reader.GetOrdinal("DateOfBirth")) ? DateTime.MinValue : _reader.GetDateTime(_reader.GetOrdinal("DateOfBirth"));
                                resident.Age = _reader.IsDBNull(_reader.GetOrdinal("Age")) ? 0 : _reader.GetInt32(_reader.GetOrdinal("Age"));
                                resident.PlaceOfBirth = _reader.IsDBNull(_reader.GetOrdinal("PlaceOfBirth")) ? null : _reader.GetString(_reader.GetOrdinal("PlaceOfBirth"));
                                resident.CivilStatus = _reader.IsDBNull(_reader.GetOrdinal("CivilStatus")) ? null : _reader.GetString(_reader.GetOrdinal("CivilStatus"));
                                resident.VoterIdNo = _reader.IsDBNull(_reader.GetOrdinal("VoterIDNo")) ? null : _reader.GetString(_reader.GetOrdinal("VoterIDNo"));
                                resident.PollingPlace = _reader.IsDBNull(_reader.GetOrdinal("PollingPlace")) ? null : _reader.GetString(_reader.GetOrdinal("PollingPlace"));
                                resident.ResidenceType = _reader.IsDBNull(_reader.GetOrdinal("ResidenceType")) ? null : _reader.GetString(_reader.GetOrdinal("ResidenceType"));
                                resident.PaymentAmount = _reader.IsDBNull(_reader.GetOrdinal("PaymentAmount")) ? 0.00m : _reader.GetDecimal(_reader.GetOrdinal("PaymentAmount"));
                                resident.PaymentFrequency = _reader.IsDBNull(_reader.GetOrdinal("PaymentFrequency")) ? null : _reader.GetString(_reader.GetOrdinal("PaymentFrequency"));
                                resident.Company = _reader.IsDBNull(_reader.GetOrdinal("Company")) ? null : _reader.GetString(_reader.GetOrdinal("Company"));
                                resident.Position = _reader.IsDBNull(_reader.GetOrdinal("Position")) ? null : _reader.GetString(_reader.GetOrdinal("Position"));
                                resident.LengthofService = _reader.IsDBNull(_reader.GetOrdinal("LengthofService")) ? null : _reader.GetString(_reader.GetOrdinal("LengthofService"));
                                resident.PreviousCompany = _reader.IsDBNull(_reader.GetOrdinal("PreviousCompany")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousCompany"));
                                resident.PreviousPosition = _reader.IsDBNull(_reader.GetOrdinal("PreviousPosition")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousPosition"));
                                resident.PreviousLengthofService = _reader.IsDBNull(_reader.GetOrdinal("PreviousLengthOfService")) ? null : _reader.GetString(_reader.GetOrdinal("PreviousLengthOfService"));
                                resident.SpouseName = _reader.IsDBNull(_reader.GetOrdinal("NameOfSpouse")) ? null : _reader.GetString(_reader.GetOrdinal("NameOfSpouse"));
                                resident.SpousePhone = _reader.IsDBNull(_reader.GetOrdinal("SpousePhone")) ? null : _reader.GetString(_reader.GetOrdinal("SpousePhone"));
                                resident.SpouseCompany = _reader.IsDBNull(_reader.GetOrdinal("SCompany")) ? null : _reader.GetString(_reader.GetOrdinal("SCompany"));
                                resident.SpousePosition = _reader.IsDBNull(_reader.GetOrdinal("SPosition")) ? null : _reader.GetString(_reader.GetOrdinal("SPosition"));
                                resident.SpouseLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SLengthOfService")) ? null : _reader.GetString(_reader.GetOrdinal("SLengthOfService"));
                                resident.SpousePrevCompany = _reader.IsDBNull(_reader.GetOrdinal("SPreviousCompany")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousCompany"));
                                resident.SpousePrevPosition = _reader.IsDBNull(_reader.GetOrdinal("SPreviousPosition")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousPosition"));
                                resident.SpousePrevLengthOfService = _reader.IsDBNull(_reader.GetOrdinal("SPreviousLengthOfService")) ? null : _reader.GetString(_reader.GetOrdinal("SPreviousLengthOfService"));
                                resident.ChildrenRelativeName = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeName")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeName"));
                                resident.ChildrenRelativeAge = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeAge")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeAge"));
                                resident.ChildrenRelativeOccupation = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeOccupation")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeOccupation"));
                                resident.ChildrenRelativeRelationship = _reader.IsDBNull(_reader.GetOrdinal("ChildrenRelativeRelationship")) ? null : _reader.GetString(_reader.GetOrdinal("ChildrenRelativeRelationship"));
                                resident.PurposeResidency = _reader.IsDBNull(_reader.GetOrdinal("PurposeResidency")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeResidency"));
                                resident.PurposePostalID = _reader.IsDBNull(_reader.GetOrdinal("PurposePostalID")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposePostalID"));
                                resident.PurposeLocalEmployment = _reader.IsDBNull(_reader.GetOrdinal("PurposeLocalEmployment")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeLocalEmployment"));
                                resident.PurposeMarriage = _reader.IsDBNull(_reader.GetOrdinal("PurposeMarriage")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMarriage"));
                                resident.PurposeLoan = _reader.IsDBNull(_reader.GetOrdinal("PurposeLoan")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeLoan"));
                                resident.PurposeMeralco = _reader.IsDBNull(_reader.GetOrdinal("PurposeMeralco")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMeralco"));
                                resident.PurposeBankTransaction = _reader.IsDBNull(_reader.GetOrdinal("PurposeBankTransaction")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeBankTransaction"));
                                resident.PurposeTravelAbroad = _reader.IsDBNull(_reader.GetOrdinal("PurposeTravelAbroad")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeTravelAbroad"));
                                resident.PurposeSeniorCitizen = _reader.IsDBNull(_reader.GetOrdinal("PurposeSeniorCitizen")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeSeniorCitizen"));
                                resident.PurposeSchool = _reader.IsDBNull(_reader.GetOrdinal("PurposeSchool")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeSchool"));
                                resident.PurposeMedical = _reader.IsDBNull(_reader.GetOrdinal("PurposeMedical")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeMedical"));
                                resident.PurposeBurial = _reader.IsDBNull(_reader.GetOrdinal("PurposeBurial")) ? false : _reader.GetBoolean(_reader.GetOrdinal("PurposeBurial"));
                                resident.PurposeOthers = _reader.IsDBNull(_reader.GetOrdinal("PurposeOthers")) ? null : _reader.GetString(_reader.GetOrdinal("PurposeOthers"));

                                // Return the populated Residents object.
                                return resident;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details to the console.
                Console.WriteLine("Exception: " + ex.ToString());
                // Rethrow the exception for higher-level error handling.
                throw;
            }

            // If no record was found, return null.
            return null;
        }
        
        /// <summary>
        /// Creates a new resident record in the SVResidents table using the data provided in the Residents object.
        /// This method establishes a database connection, constructs an INSERT SQL command with parameters,
        /// and executes the command to insert the new record.
        /// In case of any exception, it logs the error to the console and rethrows it.
        /// </summary>
        /// <param name="resident">The Residents object containing the data to be inserted.</param>
        public void CreateResident(Residents resident)
        {
            try
            {
                // Establish a connection to the database using the connection string (_repoconn).
                // The using block ensures that the SqlConnection is automatically disposed of once the operation is complete.
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    // Open the database connection.
                    _conn.Open();

                    // Define the SQL query for inserting a new resident record into the SVResidents table.
                    // The query uses parameterized values to prevent SQL injection.
                    string sql = @"
                        INSERT INTO SVResidents (
                            LastName, FirstName, MiddleName, Address, TelCelNo, Sex, Height, 
                            Weight, DateOfBirth, Age, PlaceOfBirth, CivilStatus, VoterIDNo, PollingPlace,
                            ResidenceType, PaymentAmount, PaymentFrequency, Company, Position, LengthofService, 
                            PreviousCompany, PreviousPosition, PreviousLengthOfService, NameOfSpouse, SpousePhone, 
                            SCompany, SPosition, SLengthOfService, SPreviousCompany, SPreviousPosition, SPreviousLengthOfService, 
                            ChildrenRelativeName, ChildrenRelativeAge, ChildrenRelativeOccupation, ChildrenRelativeRelationship, 
                            PurposeResidency, PurposePostalID, PurposeLocalEmployment, PurposeMarriage, PurposeLoan, 
                            PurposeMeralco, PurposeBankTransaction, PurposeTravelAbroad, PurposeSeniorCitizen, PurposeSchool, 
                            PurposeMedical, PurposeBurial, PurposeOthers
                        ) VALUES (
                            @LastName, @FirstName, @MiddleName, @Address, @TelCelNo, @Sex, @Height, 
                            @Weight, @DateOfBirth, @Age, @PlaceOfBirth, @CivilStatus, @VoterIDNo, @PollingPlace,
                            @ResidenceType, @PaymentAmount, @PaymentFrequency, @Company, @Position, @LengthofService, 
                            @PreviousCompany, @PreviousPosition, @PreviousLengthOfService, @NameOfSpouse, @SpousePhone, 
                            @SCompany, @SPosition, @SLengthOfService, @SPreviousCompany, @SPreviousPosition, @SPreviousLengthOfService, 
                            @ChildrenRelativeName, @ChildrenRelativeAge, @ChildrenRelativeOccupation, @ChildrenRelativeRelationship, 
                            @PurposeResidency, @PurposePostalID, @PurposeLocalEmployment, @PurposeMarriage, @PurposeLoan, 
                            @PurposeMeralco, @PurposeBankTransaction, @PurposeTravelAbroad, @PurposeSeniorCitizen, @PurposeSchool, 
                            @PurposeMedical, @PurposeBurial, @PurposeOthers
                        );";

                    // Create a SqlCommand with the SQL insert query and the open connection.
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        // Map each property from the resident object to the corresponding SQL parameter.
                        _cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                        _cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                        _cmd.Parameters.AddWithValue("@MiddleName", resident.MiddleName);
                        _cmd.Parameters.AddWithValue("@Address", resident.Address);
                        _cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                        _cmd.Parameters.AddWithValue("@Sex", resident.Sex);
                        _cmd.Parameters.AddWithValue("@Height", resident.Height);
                        _cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                        _cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                        _cmd.Parameters.AddWithValue("@Age", resident.Age);
                        _cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                        _cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                        _cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIdNo);
                        _cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                        _cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                        _cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                        _cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);
                        _cmd.Parameters.AddWithValue("@Company", resident.Company);
                        _cmd.Parameters.AddWithValue("@Position", resident.Position);
                        _cmd.Parameters.AddWithValue("@LengthofService", resident.LengthofService);
                        _cmd.Parameters.AddWithValue("@PreviousCompany", resident.PreviousCompany);
                        _cmd.Parameters.AddWithValue("@PreviousPosition", resident.PreviousPosition);
                        _cmd.Parameters.AddWithValue("@PreviousLengthOfService", resident.PreviousLengthofService);
                        _cmd.Parameters.AddWithValue("@NameOfSpouse", resident.SpouseName);
                        _cmd.Parameters.AddWithValue("@SpousePhone", resident.SpousePhone);
                        _cmd.Parameters.AddWithValue("@SCompany", resident.SpouseCompany);
                        _cmd.Parameters.AddWithValue("@SPosition", resident.SpousePosition);
                        _cmd.Parameters.AddWithValue("@SLengthOfService", resident.SpouseLengthOfService);
                        _cmd.Parameters.AddWithValue("@SPreviousCompany", resident.SpousePrevCompany);
                        _cmd.Parameters.AddWithValue("@SPreviousPosition", resident.SpousePrevPosition);
                        _cmd.Parameters.AddWithValue("@SPreviousLengthOfService", resident.SpousePrevLengthOfService);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeName", resident.ChildrenRelativeName);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeAge", resident.ChildrenRelativeAge);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeOccupation", resident.ChildrenRelativeOccupation);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeRelationship", resident.ChildrenRelativeRelationship);
                        _cmd.Parameters.AddWithValue("@PurposeResidency", resident.PurposeResidency);
                        _cmd.Parameters.AddWithValue("@PurposePostalID", resident.PurposePostalID);
                        _cmd.Parameters.AddWithValue("@PurposeLocalEmployment", resident.PurposeLocalEmployment);
                        _cmd.Parameters.AddWithValue("@PurposeMarriage", resident.PurposeMarriage);
                        _cmd.Parameters.AddWithValue("@PurposeLoan", resident.PurposeLoan);
                        _cmd.Parameters.AddWithValue("@PurposeMeralco", resident.PurposeMeralco);
                        _cmd.Parameters.AddWithValue("@PurposeBankTransaction", resident.PurposeBankTransaction);
                        _cmd.Parameters.AddWithValue("@PurposeTravelAbroad", resident.PurposeTravelAbroad);
                        _cmd.Parameters.AddWithValue("@PurposeSeniorCitizen", resident.PurposeSeniorCitizen);
                        _cmd.Parameters.AddWithValue("@PurposeSchool", resident.PurposeSchool);
                        _cmd.Parameters.AddWithValue("@PurposeMedical", resident.PurposeMedical);
                        _cmd.Parameters.AddWithValue("@PurposeBurial", resident.PurposeBurial);
                        _cmd.Parameters.AddWithValue("@PurposeOthers", resident.PurposeOthers);

                        // Execute the command to insert the new record into the database.
                        _cmd.ExecuteNonQuery();
                    }
                }
                AddUserLog(CurrentUser.AccountID, "Add", $"Added resident: {resident.FirstName} {resident.LastName}");

            }
            catch (Exception ex)
            {
                // Log the exception details to the console.
                Console.WriteLine("Exception: " + ex.ToString());
                // Rethrow the exception to allow higher-level error handling.
                throw;
            }
        }
        
        /// <summary>
        /// Updates an existing resident record in the SVResidents table using the data provided in the Residents object.
        /// The method establishes a database connection, constructs an UPDATE SQL command with parameters,
        /// and executes the command to modify the record identified by the ApplicantID.
        /// </summary>
        /// <param name="resident">The Residents object containing the updated data, including the ApplicantID.</param>
        public void UpdateResident(Residents resident)
        {
            try
            {
                // Establish a connection to the database using the connection string (_repoconn).
                // The using block ensures that the connection is automatically disposed when the operation is complete.
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    // Open the database connection.
                    _conn.Open();

                    // Define the SQL query for updating an existing resident record in the SVResidents table.
                    // The query updates each column with a corresponding parameter value.
                    // The record to update is identified by the ApplicantID.
                    string sql = @"
                        UPDATE SVResidents
                        SET
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
                            PaymentFrequency = @PaymentFrequency,
                            Company = @Company,
                            Position = @Position,
                            LengthofService = @LengthofService,
                            PreviousCompany = @PreviousCompany,
                            PreviousPosition = @PreviousPosition,
                            PreviousLengthofService = @PreviousLengthofService,
                            NameOfSpouse = @NameOfSpouse,
                            SpousePhone = @SpousePhone,
                            SCompany = @SCompany,
                            SPosition = @SPosition,
                            SLengthOfService = @SLengthOfService,
                            SPreviousCompany = @SPreviousCompany,
                            SPreviousPosition = @SPreviousPosition,
                            SPreviousLengthOfService = @SPreviousLengthOfService,
                            ChildrenRelativeName = @ChildrenRelativeName,
                            ChildrenRelativeAge = @ChildrenRelativeAge,
                            ChildrenRelativeOccupation = @ChildrenRelativeOccupation,
                            ChildrenRelativeRelationship = @ChildrenRelativeRelationship,
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
                        WHERE ApplicantID = @ApplicantID;
                    ";

                    // Create a SqlCommand with the SQL update query and the open connection.
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        // Map each property from the resident object to the corresponding SQL parameter.
                        _cmd.Parameters.AddWithValue("@ApplicantID", resident.Id);
                        _cmd.Parameters.AddWithValue("@LastName", resident.LastName);
                        _cmd.Parameters.AddWithValue("@FirstName", resident.FirstName);
                        _cmd.Parameters.AddWithValue("@MiddleName", resident.MiddleName);
                        _cmd.Parameters.AddWithValue("@Address", resident.Address);
                        _cmd.Parameters.AddWithValue("@TelCelNo", resident.TelCelNo);
                        _cmd.Parameters.AddWithValue("@Sex", resident.Sex);
                        _cmd.Parameters.AddWithValue("@Height", resident.Height);
                        _cmd.Parameters.AddWithValue("@Weight", resident.Weight);
                        _cmd.Parameters.AddWithValue("@DateOfBirth", resident.DateOfBirth);
                        _cmd.Parameters.AddWithValue("@Age", resident.Age);
                        _cmd.Parameters.AddWithValue("@PlaceOfBirth", resident.PlaceOfBirth);
                        _cmd.Parameters.AddWithValue("@CivilStatus", resident.CivilStatus);
                        _cmd.Parameters.AddWithValue("@VoterIDNo", resident.VoterIdNo);
                        _cmd.Parameters.AddWithValue("@PollingPlace", resident.PollingPlace);
                        _cmd.Parameters.AddWithValue("@ResidenceType", resident.ResidenceType);
                        _cmd.Parameters.AddWithValue("@PaymentAmount", resident.PaymentAmount);
                        _cmd.Parameters.AddWithValue("@PaymentFrequency", resident.PaymentFrequency);
                        _cmd.Parameters.AddWithValue("@Company", resident.Company);
                        _cmd.Parameters.AddWithValue("@Position", resident.Position);
                        _cmd.Parameters.AddWithValue("@LengthofService", resident.LengthofService);
                        _cmd.Parameters.AddWithValue("@PreviousCompany", resident.PreviousCompany);
                        _cmd.Parameters.AddWithValue("@PreviousPosition", resident.PreviousPosition);
                        _cmd.Parameters.AddWithValue("@PreviousLengthofService", resident.PreviousLengthofService);
                        _cmd.Parameters.AddWithValue("@NameOfSpouse", resident.SpouseName);
                        _cmd.Parameters.AddWithValue("@SpousePhone", resident.SpousePhone);
                        _cmd.Parameters.AddWithValue("@SCompany", resident.SpouseCompany);
                        _cmd.Parameters.AddWithValue("@SPosition", resident.SpousePosition);
                        _cmd.Parameters.AddWithValue("@SLengthOfService", resident.SpouseLengthOfService);
                        _cmd.Parameters.AddWithValue("@SPreviousCompany", resident.SpousePrevCompany);
                        _cmd.Parameters.AddWithValue("@SPreviousPosition", resident.SpousePrevPosition);
                        _cmd.Parameters.AddWithValue("@SPreviousLengthOfService", resident.SpousePrevLengthOfService);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeName", resident.ChildrenRelativeName);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeAge", resident.ChildrenRelativeAge);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeOccupation", resident.ChildrenRelativeOccupation);
                        _cmd.Parameters.AddWithValue("@ChildrenRelativeRelationship", resident.ChildrenRelativeRelationship);
                        _cmd.Parameters.AddWithValue("@PurposeResidency", resident.PurposeResidency);
                        _cmd.Parameters.AddWithValue("@PurposePostalID", resident.PurposePostalID);
                        _cmd.Parameters.AddWithValue("@PurposeLocalEmployment", resident.PurposeLocalEmployment);
                        _cmd.Parameters.AddWithValue("@PurposeMarriage", resident.PurposeMarriage);
                        _cmd.Parameters.AddWithValue("@PurposeLoan", resident.PurposeLoan);
                        _cmd.Parameters.AddWithValue("@PurposeMeralco", resident.PurposeMeralco);
                        _cmd.Parameters.AddWithValue("@PurposeBankTransaction", resident.PurposeBankTransaction);
                        _cmd.Parameters.AddWithValue("@PurposeTravelAbroad", resident.PurposeTravelAbroad);
                        _cmd.Parameters.AddWithValue("@PurposeSeniorCitizen", resident.PurposeSeniorCitizen);
                        _cmd.Parameters.AddWithValue("@PurposeSchool", resident.PurposeSchool);
                        _cmd.Parameters.AddWithValue("@PurposeMedical", resident.PurposeMedical);
                        _cmd.Parameters.AddWithValue("@PurposeBurial", resident.PurposeBurial);
                        _cmd.Parameters.AddWithValue("@PurposeOthers", resident.PurposeOthers);

                        // Execute the command to update the record in the database.
                        _cmd.ExecuteNonQuery();
                    }
                }
                AddUserLog(CurrentUser.AccountID, "Edit", $"Edited resident: {resident.FirstName} {resident.LastName}");

            }
            catch (Exception ex)
            {
                // Log the exception details to the console for debugging purposes.
                Console.WriteLine("Exception: " + ex.ToString());
                // Rethrow the exception to allow higher-level error handling.
                throw;
            }
        }
        
        /// <summary>
        /// Deletes a resident record from the SVResidents table based on the provided applicantId.
        /// The method establishes a database connection, creates a DELETE SQL command with a parameter,
        /// and executes the command to remove the corresponding record.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant to be deleted.</param>
        public void DeleteResident(int applicantId)
        {
            try
            {
                // Establish a connection to the database using the connection string (_repoconn).
                // The using statement ensures the SqlConnection is disposed of correctly after use.
                using (SqlConnection _conn = new SqlConnection(_repoconn))
                {
                    // Open the database connection.
                    _conn.Open();
            
                    // Define the SQL query that deletes a record from the SVResidents table 
                    // where the ApplicantID matches the specified parameter.
                    string sql = "DELETE FROM SVResidents WHERE ApplicantID = @ApplicantID;";
            
                    // Create a SqlCommand object with the defined SQL query and connection.
                    using (SqlCommand _cmd = new SqlCommand(sql, _conn))
                    {
                        // Add the parameter for ApplicantID using the provided applicantId.
                        _cmd.Parameters.AddWithValue("@ApplicantID", applicantId);
                
                        // Execute the command to delete the record from the database.
                        _cmd.ExecuteNonQuery();
                    }
                }
                AddUserLog(CurrentUser.AccountID, "Archived", $"Archived resident with ID: {applicantId}");

            }
            catch (Exception ex)
            {
                // Log the exception details to the console.
                Console.WriteLine("Exception: " + ex.ToString());
                // Rethrow the exception for handling at a higher level.
                throw;
            }
        }
    }
    
}