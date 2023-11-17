using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters.Twilio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Adapters;
using MoodleBot.Bots;
using MoodleBot.Business;
using MoodleBot.Business.Database;
using MoodleBot.Business.Moodle;
using MoodleBot.Common;
using MoodleBot.Dialogs;
using MoodleBot.Entity;
using MoodleBot.Models;
using MoodleBot.Models.Context;
using MoodleBot.Persistent.DBContext;
using MoodleBot.Persistent.DBStore;
using MoodleBot.Persistent.ExternalService;

namespace MoodleBot.App_Start
{
    public class DependencyInjectionConfig
    {
        public static void AddScope(IServiceCollection services, IConfiguration configuration)
        {
            #region Register Bot Component
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<TwilioAdapter, TwilioAdapterWithErrorHandler>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            IStorage userDataStore = new EntityMoodleStorage(configuration);
            //IStorage userDataStore = new EntityFrameworkStorage(configuration.Database());
            services.AddSingleton<UserState>(new UserState(userDataStore));

            // Create the Conversation state. (Used by the Dialog system itself.)
            //IStorage conversationDataStore = new EntityFrameworkStorage(configuration.Database());
            IStorage conversationDataStore = new EntityMoodleStorage(configuration);
            services.AddSingleton<ConversationState>(new ConversationState(conversationDataStore));
            #endregion

            #region Register Database Component
            services.AddScoped<ILogger, SerilogWrapper>();
            services.AddSingleton<IConcurrencyExceptionHandler, ConcurrencyExceptionHandler>();
            //services.AddTransient<IDbContext, MoodleBotContext>();
            services.AddTransient<IStoreWrapper, StoreWrapper>();
            services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            #endregion

            #region Register All Dialog
            //Create the entry point for process Message reply
            services.AddSingleton<MainDialog>();

            //Register dialog for verify country code for new user
            services.AddSingleton<VerifyCountryCodeDialog>();

            //Register dialog for Create User account
            services.AddSingleton<CreateUserAccountDialog>();

            // Register the CourseDialog.
            services.AddSingleton<CourseDialog>();

            // Register the ActivityDialog.
            services.AddSingleton<ActivityDialog>();

            // Register the Lesson Activity Dialog.
            services.AddSingleton<LessonActivityDialog>();

            // Register the Quiz Activity Dialog.
            services.AddSingleton<QuizActivityDialog>();

            // Register the Feedback Activity Dialog.
            services.AddSingleton<FeedbackActivityDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();
            #endregion

            #region Business service scope
            services.AddScoped<IMoodleUser, MoodleUser>();
            services.AddScoped<IMoodleCourse, MoodleCourse>();
            services.AddScoped<IMoodleActivity, MoodleActivity>();
            services.AddScoped<IMoodleCertificate, MoodleCertificate>();
            services.AddScoped<IBusinessCommon, BusinessCommon>();
            #endregion

            #region Business Database scope
            services.AddTransient<IGenericMessages, GenericMessagesJSON>();
            services.AddTransient<IEmojis, EmojisJSON>();
            services.AddTransient<ICountryWiseLanguage, CountryWiseLanguageJSON>();

            #endregion

            #region Persistent service scope
            services.AddScoped<IUser, User>();
            services.AddScoped<ICourse, Course>();
            services.AddScoped<IActivity, Activity>();
            services.AddScoped<ICertificate, Certificate>();
            services.AddScoped<ITwilioPhoneNumberAPI, TwilioPhoneNumberAPI>();
            #endregion

            #region Persistent Database scope
            services.AddTransient<IGenericMessageStore, GenericMessageStore>();
            services.AddTransient<IEmojisStore, EmojisStore>();
            services.AddTransient<IUserCreationQuestionStore, UserCreationQuestionStoreJSON>();
            services.AddTransient<ICountryLanguageStore, CountryLanguageStore>();
            #endregion
        }
    }
}
