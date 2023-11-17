using Microsoft.Extensions.Configuration;
using MoodleBot.Models;
using MoodleBot.Models.Context;
using MoodleBot.Persistent.DBStore;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.DBContext
{
    public class StoreWrapper : IStoreWrapper
    {
        #region Properties
        //private readonly IDbContext _dbContext;
        private IGenericMessageStore _genericMessageStore;
        private IEmojisStore _emojisStore;
        private IUserCreationQuestionStore _userCreationQuestionStore;
        private ICountryLanguageStore _countryLanguageStore;
        private readonly IConfiguration _configuration;
        #endregion
        public StoreWrapper(IConfiguration configuration)
        {
            //_dbContext = dbContext;
            _configuration = configuration;
        }

        //Add database table instance
        public IGenericMessageStore BotMessagesStore
        {
            get
            {
                if (_genericMessageStore == null)
                {
                    //_genericMessageStore = new GenericMessageStore(new RepositoryBase<BotMessages>(_dbContext), _configuration);
                }
                return BotMessagesStore;
            }
        }

        public IEmojisStore BotEmojisStore
        {
            get
            {
                if (_emojisStore == null)
                {
                    //_emojisStore = new EmojisStore(new RepositoryBase<BotEmojis>(_dbContext));
                }
                return BotEmojisStore;
            }
        }
        
        public IUserCreationQuestionStore UserCreationQuestionStore
        {
            get
            {
                if (_userCreationQuestionStore == null)
                {
                    //_userCreationQuestionStore = new UserCreationQuestionStore(new RepositoryBase<UserCreationQuestion>(_dbContext), _configuration);
                    _userCreationQuestionStore = new UserCreationQuestionStoreJSON();
                }
                return UserCreationQuestionStore;
            }
        }
        
        public ICountryLanguageStore CountryLanguageStore
        {
            get
            {
                if (_countryLanguageStore == null)
                {
                    //_countryLanguageStore = new CountryLanguageStore(new RepositoryBase<CountryLanguage>(_dbContext));
                }
                return CountryLanguageStore;
            }
        }

        public void Save()
        {
            //_dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            //await _dbContext.SaveChangesAsync();
        }
    }
}
