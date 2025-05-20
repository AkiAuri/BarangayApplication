using System;
using System.Collections.Generic;

namespace BarangayApplication.Models
{
    /// <summary>
    /// Represents a resident in the Barangay Application.
    /// </summary>
    public class Resident
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
        public string PlaceOfBirth { get; set; } = string.Empty;
        public string CivilStatus { get; set; } = string.Empty;
        public string VoterIDNo { get; set; } = string.Empty;
        public string PollingPlace { get; set; } = string.Empty;
        public string ResidenceType { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public string PaymentFrequency { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public bool isArchived { get; set; }

        // Navigation properties for related tables
        public List<Employment> Employments { get; set; } = new();
        public List<PreviousEmployment> PreviousEmployments { get; set; } = new();
        public Spouse? Spouse { get; set; }
        public List<ResidentPurpose> Purposes { get; set; } = new();
    }

    public class Employment
    {
        public int EmploymentID { get; set; }
        public int ResidentID { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string LengthOfService { get; set; } = string.Empty;
    }

    public class PreviousEmployment
    {
        public int PreviousEmploymentID { get; set; }
        public int ResidentID { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string LengthOfService { get; set; } = string.Empty;
    }

    public class Spouse
    {
        public int SpouseID { get; set; }
        public int ResidentID { get; set; }
        public string SpouseName { get; set; } = string.Empty;
        public string SpousePhone { get; set; } = string.Empty;
        public List<SpouseEmployment> Employments { get; set; } = new();
        public List<SpousePreviousEmployment> PreviousEmployments { get; set; } = new();
    }

    public class SpouseEmployment
    {
        public int SpouseEmploymentID { get; set; }
        public int SpouseID { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string LengthOfService { get; set; } = string.Empty;
    }

    public class SpousePreviousEmployment
    {
        public int SpousePrevEmploymentID { get; set; }
        public int SpouseID { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string LengthOfService { get; set; } = string.Empty;
    }

    public class PurposeType
    {
        public int PurposeTypeID { get; set; }
        public string PurposeName { get; set; } = string.Empty;
    }

    public class ResidentPurpose
    {
        public int ResidentPurposeID { get; set; }
        public int ResidentID { get; set; }
        public int PurposeTypeID { get; set; }
        public string? PurposeOthers { get; set; }

        // Navigation properties
        public PurposeType? PurposeType { get; set; }
    }
}