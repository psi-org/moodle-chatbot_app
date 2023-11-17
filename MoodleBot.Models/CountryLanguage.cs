using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class CountryLanguage : BaseEntity
    {
        [Column(TypeName = "nvarchar(20)")]
        public string ISO { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string TLD { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string CallingCode { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string LanguageName { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string LanguageISO { get; set; }
    }
}
