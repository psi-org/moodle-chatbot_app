using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoodleBot.Persistent.Entity
{
    public class APIResponse<TResult>
    {
        public TResult Data { get; set; }
        public Error Error { get; set; }
        public bool Success { get; set; }
    }

    public class Error
    {
        [JsonProperty("exception")]
        public string Exception { get; set; }

        [JsonProperty("errorcode")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
