using Korik.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateWorkShopProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public int NumbersOfTechnicians { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public WorkShopType WorkShopType { get; set; }

        [JsonIgnore]
        public IFormFile? LicenceImage { get; set; }

        [JsonIgnore]
        public string? LicenceImageUrl { get; set; }

        [JsonIgnore]
        public string? LogoImageUrl { get; set; }

        [JsonIgnore]
        public IFormFile? LogoImage { get; set; }

        [JsonIgnore]
        public string? ApplicationUserId { get; set; }
    }
}