using System.Text.Json.Serialization;

using System.Text.Json.Serialization;

namespace MauiStoreApp.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // ✅ Supabase يرجّع string للـ uid

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonIgnore]
        public string DisplayName =>
            !string.IsNullOrEmpty(Username)
                ? Username
                : Email?.Split('@')[0];

        [JsonIgnore]
        public string AvatarInitial =>
            !string.IsNullOrEmpty(Email)
                ? Email.Substring(0, 1).ToUpper()
                : "U";
    }
}
