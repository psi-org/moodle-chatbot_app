using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodleBot.Business.Database
{
    public class CountryWiseLanguageJSON : ICountryWiseLanguage
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private string CountryLanguageFileRoute = "./DBTables/CountryLanguage.json";
        #endregion

        #region Constructor
        public CountryWiseLanguageJSON(IServiceProvider services)
        {
            _services = services;
        }
        #endregion

        #region Public Method

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string iso, string callingCode)
        {
            var countryLanguages = new List<CountryLanguage>();
            try
            {

                string text = File.ReadAllText(@"" + CountryLanguageFileRoute + "");
                var countryLanguageFile = JsonSerializer.Deserialize<List<CountryLanguage>>(text);
                countryLanguages = countryLanguageFile.Where(countryLanguage => countryLanguage.LanguageISO == iso && countryLanguage.CallingCode == callingCode).ToList();

            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessageByTypeId: Got an error while get the generic Message", exception);
            }

            return countryLanguages;
        }

        public async Task<List<CountryLanguage>> GetCountryLanguageDetail(string callingCode)
        {
            var countryLanguages = new List<CountryLanguage>();
            try
            {

                string text = File.ReadAllText(@"" + CountryLanguageFileRoute + "");
                var countryLanguageFile = JsonSerializer.Deserialize<List<CountryLanguage>>(text);
                countryLanguages = countryLanguageFile.Where(countryLanguage => countryLanguage.CallingCode == callingCode).ToList();

            }
            catch (Exception exception)
            {
                _logger.Error("GetGenericMessageByTypeId: Got an error while get the generic Message", exception);
            }

            return countryLanguages;
        }
        #endregion
    }
}
