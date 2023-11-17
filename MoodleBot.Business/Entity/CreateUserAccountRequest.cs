using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class CreateUserAccountRequest
    {
        public Dictionary<string, string> UserDetails { get; set; }
        public string WhatsAppId { get; set; }
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string MobileCountryCode { get; set; }
    }
}
