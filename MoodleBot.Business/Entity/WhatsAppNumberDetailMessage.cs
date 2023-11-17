using MoodleBot.Common.Enums;
using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class WhatsAppNumberDetailMessage
    {
        public string Message { get; set; }

        public bool IsNumberDetailFound { get; set; }

        public string PhoneNumber { get; set; }

        public string NationalFormat { get; set; }

        public string CountryCode { get; set; }

        public Carrier Carrier { get; set; }

        public Dictionary<MessageName, BotMessages>  GenericMessage { get; set; }

        public List<int> ActionIdOptionMapping { get; set; }

        public string SelectedLanguageCode { get; set; }
        public string MobileCountryCode { get; set; }
    }

    public class Carrier
    {
        public string MobileCountryCode { get; set; }

        public string MobileNetworkCode { get; set; }

        public string CompanyName { get; set; }

        public string Type { get; set; }

        public string ErrorCode { get; set; }
    }
}
