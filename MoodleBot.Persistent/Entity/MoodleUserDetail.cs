using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleUserDetail
    {
        [JsonProperty("userid")]
        public int Userid { get; set; }

        [JsonProperty("auth")]
        public string Authentication { get; set; }

        [JsonProperty("confirmed")]
        public string Confirmed { get; set; }

        [JsonProperty("policyagreed")]
        public string PolicyAgreed { get; set; }

        [JsonProperty("deleted")]
        public string Deleted { get; set; }

        [JsonProperty("suspended")]
        public string Suspended { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("idnumber")]
        public string Idnumber { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emailstop")]
        public string EmailStop { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("firstaccess")]
        public string FirstAccess { get; set; }

        [JsonProperty("lastaccess")]
        public string LastAccess { get; set; }

        [JsonProperty("lastlogin")]
        public string LastLogin { get; set; }

        [JsonProperty("currentlogin")]
        public string CurrentLogin { get; set; }

        [JsonProperty("lastip")]
        public string Lastip { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionformat")]
        public string DescriptionFormat { get; set; }

        [JsonProperty("mailformat")]
        public string MailFormat { get; set; }

        [JsonProperty("maildigest")]
        public string Maildigest { get; set; }

        [JsonProperty("maildisplay")]
        public string MailDisplay { get; set; }

        [JsonProperty("autosubscribe")]
        public string Autosubscribe { get; set; }

        [JsonProperty("trackforums")]
        public string TrackForums { get; set; }

        [JsonProperty("timecreated")]
        public string TimeCreated { get; set; }

        [JsonProperty("timemodified")]
        public string TimeModified { get; set; }

        [JsonProperty("imagealt")]
        public string Imagealt { get; set; }

        [JsonProperty("middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("alternatename")]
        public string AlternateName { get; set; }

        [JsonProperty("health_unit_id")]
        public object HealthUnitId { get; set; }

        [JsonProperty("municipality")]
        public object Municipality { get; set; }

        [JsonProperty("province")]
        public object Province { get; set; }

        [JsonProperty("profession_type")]
        public string ProfessionType { get; set; }

        [JsonProperty("professional_number")]
        public string ProfessionalNumber {get;set;}
        
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("mobile_phone")]
        public string MobilePhone { get; set; }

        [JsonProperty("whatsappid")]
        public object WhatsappId { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("isdeleted")]
        public string IsDeleted { get; set; }

        [JsonProperty("gender_id")]
        public string GenderId { get; set; }

        [JsonProperty("dateofbirth")]
        public object DateOfBirth { get; set; }
    }

    public class MoodleUserRequest : MoodleAPIBaseParameter
    {
        public MoodleUserRequest(IConfiguration configuration) : base(configuration)
        {

        }

        [JsonProperty("mobile_phone", NullValueHandling = NullValueHandling.Ignore)]
        public string MobilePhone { get; set; }

        [JsonProperty("whatsappid", NullValueHandling = NullValueHandling.Ignore)]
        public string WhatsAppId { get; set; }
    }
}
