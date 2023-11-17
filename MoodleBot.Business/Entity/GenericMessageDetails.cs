using MoodleBot.Models;
using System.Collections.Generic;

namespace MoodleBot.Business.Entity
{
    public class GenericMessageDetails
    {
        public Dictionary<string, BotMessages> MessagesDetails { get; set; }
    }
}
