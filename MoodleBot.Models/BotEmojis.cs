using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodleBot.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class BotEmojis : BaseEntity
    {
        public string EmojisName { get; set; }

        public int? StatusId { get; set; }

        public EmojisType EmojisTypeId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Emojis { get; set; }

        public class BotEmojisConfiguration : IEntityTypeConfiguration<BotEmojis>
        {
            public void Configure(EntityTypeBuilder<BotEmojis> builder)
            {
                builder.Property(p => p.StatusId).HasDefaultValue(null);
            }
        }
    }
}
