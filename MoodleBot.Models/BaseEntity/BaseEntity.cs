using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class BaseEntity
    {
        public long Id { get; set; }

        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset DateCreated { get; set; }

        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset DateUpdated { get; set; }

        public BaseEntity()
        {
            DateCreated = DateTimeOffset.UtcNow;
            DateUpdated = DateTimeOffset.UtcNow;
        }
    }
}
