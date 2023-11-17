using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public interface ICountryLanguageStore
    {
        Task<List<CountryLanguage>> GetCountryLanguageDetail(string iso, string callingCode);
        Task<List<CountryLanguage>> GetCountryLanguageDetail(string callingCode);
    }
}
