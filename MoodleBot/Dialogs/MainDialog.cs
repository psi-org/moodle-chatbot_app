using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Business;
using MoodleBot.Business.Database;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MoodleBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly UserState _userState;
        private readonly IServiceProvider _services;
        private IMoodleUser _moodleUser;
        private IGenericMessages _genericMessages;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public MainDialog(UserState userState, CourseDialog courseDialog, VerifyCountryCodeDialog verifyCountryCodeDialog, CreateUserAccountDialog createUserAccountDialog, IServiceProvider services, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            _userState = userState;
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));

            #region Register Prompt
            AddDialog(new TextPrompt("UserRegistrationChoice", UserRegistrationOptionValidation));
            #endregion

            AddDialog(courseDialog);
            AddDialog(verifyCountryCodeDialog);
            AddDialog(createUserAccountDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                ReviewUserRegistrationProcess,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region WaterFall dialog steps
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = stepContext.Context.Activity;
            var userDetail = await _moodleUser.GetUserDetail(activity.From.Id);
            var languageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
            var isUserDetailFound = false;
            if (userDetail != null && userDetail.Userid > 0)
            {
                isUserDetailFound = true;
                languageCode = userDetail.Language;
            }
            else
            {
                var userProfile = await IdentifyUserLanguage(stepContext);
                if (userProfile.CreateUserAccountDetail.CurrentLanguageCode.IsNotNullOrEmpty())
                {
                    languageCode = userProfile.CreateUserAccountDetail.CurrentLanguageCode;
                }
            }

            await SendWelcomeMessage(stepContext.Context, languageCode);

            if (isUserDetailFound)
            {
                if(userDetail.Verified){
                    var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
                    userProfile.UserDetail = userDetail;
                    userProfile.UserId = userDetail.Userid;
                    userProfile.WhatsAppNumber = userProfile.WhatsAppNumber;
                    userProfile.IsVerifiedUser = true;
                    userProfile.ShouldDisplayWelComeMessage = true;
                    userProfile.PreferredLanguageCode = userDetail.Language;
                    await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                    return await stepContext.BeginDialogAsync(nameof(CourseDialog), null, cancellationToken);
                }
                else{
                    var whatsAppId = stepContext.Context.Activity.From.Id;
                    var mobileNumberDetail = await _moodleUser.GetWhatsAppNumberDetailsAsync(whatsAppId, languageCode);
                    var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text(genericMessages[MessageName.USER_NOT_VERIFIED].Message),
                        RetryPrompt = MessageFactory.Text(genericMessages[MessageName.INVALID_INPUT].Message)
                    };

                    var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
                    userProfile.UserDetail = userDetail;
                    userProfile.UserId = userDetail.Userid;
                    userProfile.WhatsAppNumber = userProfile.WhatsAppNumber;
                    userProfile.IsVerifiedUser = false;
                    userProfile.ShouldDisplayWelComeMessage = true;
                    userProfile.PreferredLanguageCode = userDetail.Language;
                    userProfile.CreateUserAccountDetail = new CreateUserAccountDetail
                    {
                        UserDetails = new Dictionary<string, string>()
                    };

                    userProfile.CreateUserAccountDetail.WhatsAppId = whatsAppId;
                    userProfile.CreateUserAccountDetail.CountryCode = mobileNumberDetail.CountryCode;
                    userProfile.CreateUserAccountDetail.CurrentLanguageCode = userDetail.Language;
                    userProfile.CreateUserAccountDetail.MobileCountryCode = mobileNumberDetail.MobileCountryCode;
                    userProfile.CreateUserAccountDetail.MobilePhoneNumber = mobileNumberDetail.PhoneNumber;
                    userProfile.CreateUserAccountDetail.CurrentQuestionNumber = 9;
                    userProfile.CreateUserAccountDetail.LanguageChoiceAction = mobileNumberDetail.ActionIdOptionMapping;
                    #region add user details to be updated
                    //var genderTypeName = Enum.GetName(typeof(Gender), Int32.Parse(userDetail.GenderId));
                    //var healthWorkerType = Enum.GetName(typeof(HealthWorkerType), Int32.Parse(userDetail.ProfessionType));

                    userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.FIRST_NAME.ToString(), userDetail.FirstName);
                    userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.LAST_NAME.ToString(), userDetail.LastName);
                    //userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.GENDER.ToString(), genderTypeName);
                    //userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.HEALTH_WORKER_TYPE.ToString(), healthWorkerType);
                    userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.EMAIL.ToString(), userDetail.Email);
                    //userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.DATE_OF_BIRTH.ToString(), (userDetail.DateOfBirth == null ? "" : DateTime.Parse(userDetail.DateOfBirth.ToString()).ToString("yyyy-MM-dd")));
                    //userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.HEALTH_WORKER_NUMBER.ToString(), userDetail.ProfessionalNumber);
                    userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.PASSWORD.ToString(), userDetail.MobilePhone);
                    #endregion
                    userProfile.CreateUserAccountDetail.IsLastQuestion = false;
                    userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess = false;

                    await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                    // Prompt the user for a choice.
                    return await stepContext.PromptAsync("UserRegistrationChoice", promptOptions, cancellationToken);
                }
            }
            else
            {
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(genericMessages[MessageName.USER_NOT_REGISTERD].Message),
                    RetryPrompt = MessageFactory.Text(genericMessages[MessageName.INVALID_INPUT].Message)
                };

                // Prompt the user for a choice.
                return await stepContext.PromptAsync("UserRegistrationChoice", promptOptions, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ReviewUserRegistrationProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var userProfileIsNull = false;
            if(!(userProfile.UserDetail == null)){
                userProfileIsNull = !userProfile.UserDetail.Verified;
            }

            if (userProfile.IsVerifiedUser)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
            else if(userProfileIsNull)
            {
                return await stepContext.BeginDialogAsync(nameof(CreateUserAccountDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(VerifyCountryCodeDialog), null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (userProfile != null && userProfile.IsUserCreated)
            {
                userProfile.IsUserCreated = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
            }

            var genericMessages = await _genericMessages.GetGenericMessageByTypeId(_configuration.GetMoodleConfig("DefaultLanguageCode"), MessageType.User);
            var greetingsMessage = genericMessages[MessageName.USER_GREETINGS].Message;
            if (userProfile?.UserDetail != null)
            {
                greetingsMessage = string.Format(greetingsMessage, userProfile.UserDetail.FirstName, userProfile.UserDetail.LastName);
            }
            else
            {
                greetingsMessage = string.Format(greetingsMessage, string.Empty, string.Empty);
            }
            await stepContext.Context.SendActivityAsync(greetingsMessage);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Validation
        private async Task<bool> UserRegistrationOptionValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int userAccountCreationOption);
            var result = (isNumber && userAccountCreationOption == 1);

            if (!result && userProfile.IsWrongInputAdded && userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                result = true;
            }

            if (!result && !userProfile.IsWrongInputAdded)
            {
                userProfile.IsWrongInputAdded = true;
            }

            userProfile.InValidInputAttempt = result ? 0 : ++userProfile.InValidInputAttempt;

            if (userProfile.InValidInputAttempt == 3)
            {
                throw new Exception("Invalid input attempt exceed.");
            }

            await _userProfileStateAccessor.SetAsync(promptcontext.Context, userProfile);

            return result;
        }
        #endregion

        #region Private Method
        private async Task SendWelcomeMessage(ITurnContext context, string languageCode)
        {
            var genericMessages = await _genericMessages.GetGenericMessageByTypeId(languageCode, MessageType.WelcomeMessage);
            var welcomeImage = genericMessages[MessageName.WELCOME_IMAGE].Message;

            if (welcomeImage.IsNotNullOrEmpty())
            {
                await context.SendActivityAsync(MessageFactory.Attachment(new Attachment("image/jpg", welcomeImage)));
                await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig("ImageWaitingTime")));
            }

            await context.SendActivityAsync(MessageFactory.Text(genericMessages[MessageName.WELCOME_MESSAGE].Message));
            await Task.Delay(1000);
        }

        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _genericMessages = scope.ServiceProvider.GetRequiredService<IGenericMessages>();
            _moodleUser = scope.ServiceProvider.GetRequiredService<IMoodleUser>();
        }

        private async Task<UserProfile> IdentifyUserLanguage(WaterfallStepContext stepContext)
        {
            var whatsAppId = stepContext.Context.Activity.From.Id;
            var mobileNumberDetail = await _moodleUser.GetWhatsAppNumberDetailsAsync(whatsAppId, _configuration.GetMoodleConfig("DefaultLanguageCode"));
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            
            if (userProfile.CreateUserAccountDetail == null)
            {
                userProfile.CreateUserAccountDetail = new CreateUserAccountDetail
                {
                    UserDetails = new Dictionary<string, string>()
                };
            }

            if (mobileNumberDetail.IsNumberDetailFound)
            {
                var genericMessages = mobileNumberDetail.GenericMessage;
                userProfile.CreateUserAccountDetail.WhatsAppId = whatsAppId;
                userProfile.CreateUserAccountDetail.CountryCode = mobileNumberDetail.CountryCode;
                userProfile.CreateUserAccountDetail.MobileCountryCode = mobileNumberDetail.MobileCountryCode;
                userProfile.CreateUserAccountDetail.MobilePhoneNumber = mobileNumberDetail.PhoneNumber;
                userProfile.CreateUserAccountDetail.CurrentLanguageCode = mobileNumberDetail.SelectedLanguageCode;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }

            return userProfile;
        }
        #endregion
    }
}
