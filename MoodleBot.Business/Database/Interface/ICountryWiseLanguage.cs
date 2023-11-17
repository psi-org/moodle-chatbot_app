using MoodleBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public interface ICountryWiseLanguage
    {
        Task<List<CountryLanguage>> GetCountryLanguageDetail(string iso, string callingCode);
        Task<List<CountryLanguage>> GetCountryLanguageDetail(string callingCode);
    }
}
