using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Models;
using MoodleBot.Persistent.DBStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public class CountryWiseLanguage : ICountryWiseLanguage
    {
        #region Properties
        private readonly IServiceProvider _services;
        #endregion

        #region Constructor
        public CountryWiseLanguage(IServiceProvider services)
        {
            _services = services;
        }
        #endregion

        #region Public Method

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string iso, string callingCode)
        {
            using var scope = _services.CreateScope();
            var countryLanguageStore = scope.ServiceProvider.GetRequiredService<ICountryLanguageStore>();
            return await countryLanguageStore.GetCountryLanguageDetail(iso, callingCode);
        }

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string callingCode)
        {
            using var scope = _services.CreateScope();
            var countryLanguageStore = scope.ServiceProvider.GetRequiredService<ICountryLanguageStore>();
            return await countryLanguageStore.GetCountryLanguageDetail(callingCode);
        }
        #endregion
    }
}
