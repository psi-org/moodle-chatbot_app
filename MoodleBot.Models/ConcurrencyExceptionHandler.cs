using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoodleBot.Models
{
    public interface IConcurrencyExceptionHandler
    {
        string GetAdditionalInfo(DbUpdateConcurrencyException ce);
    }
    public class ConcurrencyExceptionHandler : IConcurrencyExceptionHandler
    {
        public string GetAdditionalInfo(DbUpdateConcurrencyException ce)
        {
            string additionalInfo = string.Empty;
            if (ce == null || ce.Entries == null)
            {
                return additionalInfo;
            }

            foreach (var entry in ce.Entries)
            {
                additionalInfo += $"{entry.Metadata.Name} causing problems. " + Environment.NewLine;
                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();
                foreach (var property in proposedValues.Properties)
                {
                    var proposedValue = proposedValues[property];
                    var databaseValue = databaseValues != null ? databaseValues[property] : "doesn't exist in DB";
                    additionalInfo += $"EF value:{proposedValue}, DB value:{databaseValue}. " + Environment.NewLine;
                }
            }

            return additionalInfo;
        }
    }
}
