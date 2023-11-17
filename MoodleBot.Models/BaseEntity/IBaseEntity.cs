using System;

namespace MoodleBot.Models
{
    public interface IBaseEntity
    {
        long Id { get; set; }

        DateTime DateCreated { get; set; }

        DateTime DateUpdated { get; set; }
    }
}
