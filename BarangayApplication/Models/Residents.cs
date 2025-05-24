using System;
using System.Collections.Generic;

namespace BarangayApplication.Models
{
    /// <summary>
    /// Represents a resident in the Barangay Application.
    /// </summary>
    public class Resident
    {
        public int ResidentID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Address { get; set; } = string.Empty;
        public string TelCelNo { get; set; } = string.Empty;
        public byte SexID { get; set; } // BIT in SQL, use byte in C#
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; } = string.Empty;
        public int CivilStatusID { get; set; }
        public string VoterIDNo { get; set; } = string.Empty;
        public string PollingPlace { get; set; } = string.Empty;
        public int ResidenceTypeID { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }

        // Navigation properties
        public Sex? Sex { get; set; }
        public CivilStatus? CivilStatus { get; set; }
        public ResidenceType? ResidenceType { get; set; }
        public List<Employment> Employments { get; set; } = new();
        public Spouse? Spouse { get; set; }
        public List<ResidentPurpose> Purposes { get; set; } = new();
    }

    public class Sex
    {
        public byte SexID { get; set; }
        public string SexDescription { get; set; } = string.Empty;
    }

    public class CivilStatus
    {
        public int CivilStatusID { get; set; }
        public string CivilStatusDescription { get; set; } = string.Empty;
    }

    public class ResidenceType
    {
        public int ResidenceTypeID { get; set; }
        public string ResidenceTypeName { get; set; } = string.Empty;
    }

    public class Employment
    {
        public int EmploymentID { get; set; }
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

        public List<SpouseEmployment> Employments { get; set; } = new();
    }

    public class SpouseEmployment
    {
        public int SpouseEmploymentID { get; set; }
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
        public string TransactionID { get; set; } = string.Empty; // Primary key
        public int ResidentID { get; set; }
        public int PurposeTypeID { get; set; }
        public string? PurposeOthers { get; set; }

        // Navigation properties
        public PurposeType? PurposeType { get; set; }
    }
}