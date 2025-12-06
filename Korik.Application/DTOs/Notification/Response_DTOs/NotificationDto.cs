using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        [JsonConverter(typeof(JsonNumberEnumConverter<NotificationType>))]
        public NotificationType Type { get; set; }
        
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? BookingId { get; set; }
        
        // Additional fields for frontend compatibility
        public string? Title { get; set; }
        public string? Priority { get; set; }
        public DateTime? ConfirmationDeadline { get; set; }
    }

    // Custom converter to ensure enum is serialized as number
    public class JsonNumberEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var enumValue = reader.GetInt32();
                return (TEnum)(object)enumValue;
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt32(value));
        }
    }
}
