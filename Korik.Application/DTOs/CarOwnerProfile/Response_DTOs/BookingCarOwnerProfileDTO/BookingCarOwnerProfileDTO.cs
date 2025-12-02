using Korik.Domain;

namespace Korik.Application
{
    public class BookingCarOwnerProfileDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public PreferredLanguage PreferredLanguage { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
