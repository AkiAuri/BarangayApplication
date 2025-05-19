using System;
using System.Collections.Generic;

namespace BarangayApplication.Models
{
    /// <summary>
    /// Represents a resident in the Barangay Application.
    /// </summary>
    public class Residents
    {
        // Resident table fields
        public int ResidentID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Address { get; set; } = string.Empty;
        public string TelCelNo { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string PlaceOfBirth { get; set; } = string.Empty;
        public string CivilStatus { get; set; } = string.Empty;
        public string VoterIDNo { get; set; } = string.Empty;
        public string PollingPlace { get; set; } = string.Empty;
        public string ResidenceType { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public string PaymentFrequency { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }

        // Navigation properties for related tables
        public Employment? Employment { get; set; }
        public Spouse? Spouse { get; set; }
        public Purposes? Purposes { get; set; }
    }

    public class Employment
    {
        public int EmploymentID { get; set; }
        public int ResidentID { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string LengthOfService { get; set; } = string.Empty;
        public string PreviousCompany { get; set; } = string.Empty;
        public string PreviousPosition { get; set; } = string.Empty;
        public string PreviousLengthOfService { get; set; } = string.Empty;
    }

    public class Spouse
    {
        public int SpouseID { get; set; }
        public int ResidentID { get; set; }
        public string SpouseName { get; set; } = string.Empty;
        public string SpousePhone { get; set; } = string.Empty;
        public string SpouseCompany { get; set; } = string.Empty;
        public string SpousePosition { get; set; } = string.Empty;
        public string SpouseLengthOfService { get; set; } = string.Empty;
        public string SpousePreviousCompany { get; set; } = string.Empty;
        public string SpousePreviousPosition { get; set; } = string.Empty;
        public string SpousePreviousLengthOfService { get; set; } = string.Empty;
    }

    public class Purposes
    {
        public int PurposeID { get; set; }
        public int ResidentID { get; set; }
        public bool PurposeResidency { get; set; }
        public bool PurposePostalID { get; set; }
        public bool PurposeLocalEmployment { get; set; }
        public bool PurposeMarriage { get; set; }
        public bool PurposeLoan { get; set; }
        public bool PurposeMeralco { get; set; }
        public bool PurposeBankTransaction { get; set; }
        public bool PurposeTravelAbroad { get; set; }
        public bool PurposeSeniorCitizen { get; set; }
        public bool PurposeSchool { get; set; }
        public bool PurposeMedical { get; set; }
        public bool PurposeBurial { get; set; }
        public string PurposeOthers { get; set; } = string.Empty;
    }
}