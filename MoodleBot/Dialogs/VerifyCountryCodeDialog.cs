using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Entity;
using MoodleBot.Business;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Business.Database;
using MoodleBot.Business.Entity;
using System.Collections.Generic;

namespace MoodleBot.Dialogs
{
    public class VerifyCountryCodeDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly UserState _userState;
        private readonly IServiceProvider _services;
        private IMoodleUser _moodleUser;
        private readonly IConfiguration _configuration;
        private IGenericMessages _genericMessages;
        #endregion

        #region Constructor
        public VerifyCountryCodeDialog(UserState userState, CreateUserAccountDialog createUserAccountDialog, IServiceProvider services, IConfiguration configuration)
            : base(nameof(VerifyCountryCodeDialog))
        {
            _userState = userState;
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));

            #region Register Prompt
            AddDialog(new TextPrompt("LanguageChoice", LanguageChoiceValidation));
            AddDialog(new TextPrompt("MobileCountryCodeChoice", MobileCountryCodeValidation));
            #endregion

            AddDialog(createUserAccountDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                ReviewCountryCodeSelection,
                GetCountryCodeFromUser,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region WaterFall dialog steps
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var createUserAccountDetail = userProfile.CreateUserAccountDetail;
            var mobileNumberDetail = new WhatsAppNumberDetailMessage();
            var whatsAppId = string.Empty;
            var languageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
            if (userProfile.CreateUserAccountDetail.CurrentLanguageCode.IsNotNullOrEmpty())
            {
                languageCode = userProfile.CreateUserAccountDetail.CurrentLanguageCode;
            }

            if (createUserAccountDetail != null && createUserAccountDetail.UserCountryCode.IsNotNullOrEmpty())
            {
                mobileNumberDetail = await _moodleUser.GetLanguageSelectionMessage(createUserAccountDetail.UserCountryCode, languageCode);
            }
            else
            {
                whatsAppId = stepContext.Context.Activity.From.Id;
                mobileNumberDetail = await _moodleUser.GetWhatsAppNumberDetailsAsync(whatsAppId, languageCode);
            }
            
            if (mobileNumberDetail.IsNumberDetailFound)
            {
                var genericMessages = mobileNumberDetail.GenericMessage;
                if (userProfile.CreateUserAccountDetail == null)
                {
                    userProfile.CreateUserAccountDetail = new CreateUserAccountDetail
                    {
                        UserDetails = new Dictionary<string, string>()
                    };
                }

                if (userProfile.CreateUserAccountDetail.UserCountryCode.IsNullOrEmpty())
                {
                    userProfile.CreateUserAccountDetail.WhatsAppId = whatsAppId;
                    userProfile.CreateUserAccountDetail.CountryCode = mobileNumberDetail.CountryCode;
                    userProfile.CreateUserAccountDetail.MobileCountryCode = mobileNumberDetail.MobileCountryCode;
                    userProfile.CreateUserAccountDetail.MobilePhoneNumber = mobileNumberDetail.PhoneNumber;
                }
                else
                {
                    createUserAccountDetail.UserCountryCode = null;
                }

                userProfile.CreateUserAccountDetail.LanguageChoiceAction = mobileNumberDetail.ActionIdOptionMapping;
                userProfile.CreateUserAccountDetail.CurrentLanguageCode = mobileNumberDetail.SelectedLanguageCode;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(mobileNumberDetail.Message),
                    RetryPrompt = MessageFactory.Text(genericMessages[MessageName.INVALID_INPUT].Message)
                };

                // Prompt the user for a choice.
                return await stepContext.PromptAsync("LanguageChoice", promptOptions, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ReviewCountryCodeSelection(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var userInput = stepContext.Result.ToString();

            if (userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.BeginDialogAsync(nameof(VerifyCountryCodeDialog), null, cancellationToken);
            }

            var languageSelectedOption = Convert.ToInt32(userInput);

            if (languageSelectedOption == 2)
            {
                var languageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
                if (userProfile.CreateUserAccountDetail.CurrentLanguageCode.IsNotNullOrEmpty())
                {
                    languageCode = userProfile.CreateUserAccountDetail.CurrentLanguageCode;
                }

                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                var createUserMessages = await _moodleUser.GetQuestionDetails(languageCode);
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(createUserMessages[CreateUserMessageName.ASK_MOBILE_COUNTRY_CODE].Question),
                    RetryPrompt = MessageFactory.Text(genericMessages[MessageName.INVALID_INPUT].Message)
                };

                // Prompt the user for a choice.
                return await stepContext.PromptAsync("MobileCountryCodeChoice", promptOptions, cancellationToken);
            }
            else if (languageSelectedOption == 3)
            {
                userProfile.CreateUserAccountDetail.CurrentLanguageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }
            else if (languageSelectedOption != 1)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            
            return await stepContext.BeginDialogAsync(nameof(CreateUserAccountDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> GetCountryCodeFromUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (userProfile.IsUserCreated || userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var userInput = stepContext.Result?.ToString();

            if (userInput.IsNotNullOrEmpty())
            {
                userProfile.IsWrongInputAdded = false;
                if (userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
                {
                    stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                    await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                    return await stepContext.NextAsync(2, cancellationToken);
                }

                var mobileCountryCode = stepContext.Result.ToString();
                userProfile.CreateUserAccountDetail.UserCountryCode = mobileCountryCode;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(VerifyCountryCodeDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.CreateUserAccountDetail = null;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Validation
        private async Task<bool> LanguageChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int languageChoiceAction);
            var result = (isNumber && userProfile.CreateUserAccountDetail.LanguageChoiceAction.Contains(languageChoiceAction));

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

        private async Task<bool> MobileCountryCodeValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int languageChoiceAction);
            var result = isNumber;

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
        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _moodleUser = scope.ServiceProvider.GetRequiredService<IMoodleUser>();
            _genericMessages = scope.ServiceProvider.GetRequiredService<IGenericMessages>();
        }
        #endregion
    }
}
