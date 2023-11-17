using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleUpdateUserDetail
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("userid")]
        public long UserId { get; set; }
    }

    public class MoodleUpdateUserRequest : MoodleAPIBaseParameter
    {
        public MoodleUpdateUserRequest(IConfiguration configuration) : base(configuration)
        {

        }

        [JsonProperty("userid")]
        public int Userid { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("dateofbirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("profession_type")]
        public int HealthWorkerType { get; set; }

        [JsonProperty("professional_number")]
        public string HealthWorkerNumber { get; set; }
    }
}
