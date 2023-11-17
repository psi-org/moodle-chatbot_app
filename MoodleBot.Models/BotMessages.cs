using MoodleBot.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class BotMessages : BaseEntity
    {
        public string MessageName { get; set; }
        public MessageType TypeId { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string Message { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string LanguageISOCode { get; set; }
    }
}
