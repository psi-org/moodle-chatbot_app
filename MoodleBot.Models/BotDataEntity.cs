using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoodleBot.Models
{
    public class BotDataEntity
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(1024)]
        [Required]
        public string RealId { get; set; }

        [Required]
        public string Document { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
     
        public DateTimeOffset TimeStamp { get; set; }

        public class BotDataEntityConfiguration : IEntityTypeConfiguration<BotDataEntity>
        {
            public void Configure(EntityTypeBuilder<BotDataEntity> builder)
            {
                builder.HasIndex(p => p.RealId).IsUnique();
                builder.Property(x => x.CreatedTime).IsRequired().HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
                builder.Property(x => x.TimeStamp).IsRequired().HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
            }
        }
    }
}
