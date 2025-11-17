using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumbersOfTechnicians { get; set; }
        public string PhoneNumber { get; set; }
        public double Rating { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? LicenceImageUrl { get; set; }
        public string? LogoImageUrl { get; set; }
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public WorkShopType WorkShopType { get; set; }
    }
}