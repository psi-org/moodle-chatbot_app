using Microsoft.EntityFrameworkCore;
using MoodleBot.Common;
using MoodleBot.Models;
using MoodleBot.Persistent.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBStore
{
    public class CountryLanguageStore : ICountryLanguageStore
    {
        #region Properties
        private readonly IRepositoryBase<CountryLanguage> _repoCountryLanguage;
        #endregion

        #region Constructor
        public CountryLanguageStore(IRepositoryBase<CountryLanguage> repoCountryLanguage)
        {
            _repoCountryLanguage = repoCountryLanguage;
        }
        #endregion

        #region Public Method

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string iso, string callingCode)
        {
            return await _repoCountryLanguage
                    .GetManyQueryable(x => x.ISO == iso && x.CallingCode == callingCode)
                    .ToListAsync();
        }

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string callingCode)
        {
            return await _repoCountryLanguage
                    .GetManyQueryable(x => x.CallingCode == callingCode)
                    .ToListAsync();
        }
        #endregion
    }
}
