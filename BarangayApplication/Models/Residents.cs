using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarangayApplication.Models
{
    /// <summary>
    /// Represents a resident in the Barangay Application.
    /// This class contains properties to hold personal, contact, and employment details,
    /// as well as various boolean flags indicating the resident's purposes for residency.
    /// </summary>
    public class Residents
    {
        // Unique Identifier for the resident.
        public int Id = 0;

        // Personal Name Information.
        public String LastName = "";
        public String FirstName = "";
        public String MiddleName = "";

        // Contact and Address Information.
        public String Address = "";
        public String TelCelNo = "";

        // Demographic Details.
        public String Sex = "";
        public decimal Height;
        public decimal Weight;
        public DateTime DateOfBirth = DateTime.Now;
        public int Age;
        public String PlaceOfBirth = "";
        public String CivilStatus = "";

        // Voting and polling details.
        public String VoterIdNo = "";
        public String PollingPlace = "";

        // Residency and Financial Details.
        public String ResidenceType = "";
        public Decimal PaymentAmount;
        public String PaymentFrequency = "";

        // Employment and Service Information.
        public String Company = "";
        public String Position = "";
        public String LengthofService = "";
        public String PreviousCompany = "";
        public String PreviousPosition = "";
        public String PreviousLengthofService = "";

        // Spouse Information.
        public String SpouseName = "";
        public String SpousePhone = "";
        public String SpouseCompany = "";
        public String SpousePosition = "";
        public String SpouseLengthOfService = "";
        public String SpousePrevCompany = "";
        public String SpousePrevPosition = "";
        public String SpousePrevLengthOfService = "";

        // Children's Relative Information.
        public String ChildrenRelativeName = "";
        public String ChildrenRelativeAge = "";
        public String ChildrenRelativeOccupation = "";
        public String ChildrenRelativeRelationship = "";

        // Purpose Flags for different services or applications.
        public bool PurposeResidency;
        public bool PurposePostalID;
        public bool PurposeLocalEmployment;
        public bool PurposeMarriage;
        public bool PurposeLoan;
        public bool PurposeMeralco;
        public bool PurposeBankTransaction;
        public bool PurposeTravelAbroad;
        public bool PurposeSeniorCitizen;
        public bool PurposeSchool;
        public bool PurposeMedical;
        public bool PurposeBurial;

        // Additional purpose details.
        public String PurposeOthers = "";
    }
}