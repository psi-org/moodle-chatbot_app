using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class WhatsAppNumberDetails
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("national_format")]
        public string NationalFormat { get; set; }

        [JsonProperty("country_code")]
        public string CountryRegionCode { get; set; }

        [JsonProperty("Carrier")]
        public Carrier Carrier { get; set; }
        
        public bool IsSuccess { get; set; }

        public string CountryCode { get; set; }
    }

    public class Carrier
    {
        [JsonProperty("mobile_country_code")]
        public string MobileCountryCode { get; set; }

        [JsonProperty("mobile_network_code")]
        public string MobileNetworkCode { get; set; }

        [JsonProperty("name")]
        public string CompanyName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }
    }
}
