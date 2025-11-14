using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ProfileWithCarDTO
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int EngineCapacity { get; set; }
        public int CurrentMileage { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
    }

    public class CarOwnerProfileWithCarDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string? ProfileImageUrl { get; set; }

        public PreferredLanguage PreferredLanguage { get; set; }

        public List<ProfileWithCarDTO> Cars { get; set; } = new List<ProfileWithCarDTO>();
    }
}