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
    public class UpdateCarOwnerProfileDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }

        [JsonIgnore]
        public string? ProfileImageUrl { get; set; }

        [JsonIgnore]
        public IFormFile? ProfileImage { get; set; }

        public PreferredLanguage PreferredLanguage { get; set; }

        [JsonIgnore]
        public string? ApplicationUserId { get; set; }
    }
}