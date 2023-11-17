using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Business;
using MoodleBot.Business.Database;
using MoodleBot.Business.Entity;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Entity;
using MoodleBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MoodleBot.Dialogs
{
    public class CreateUserAccountDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly UserState _userState;
        private readonly IServiceProvider _services;
        private IMoodleUser _moodleUser;
        private IEmojis _emojis;
        private readonly IConfiguration _configuration;
        private static Dictionary<EmojiName, BotEmojis> nextPreviousOption;
        #endregion

        #region Constructor
        public CreateUserAccountDialog(UserState userState, CourseDialog courseDialog, IServiceProvider services, IConfiguration configuration)
            : base(nameof(CreateUserAccountDialog))
        {
            _userState = userState;
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));

            #region Register Prompt
            AddDialog(new TextPrompt("QuestionDetail", QuestionDetailValidation));
            #endregion

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                SaveUserAnswer,
                CreateUserInMoodle,
                UpdateUserInMoodle,
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
            var currentQuestionNo = 1;
            if (createUserAccountDetail != null)
            {
                currentQuestionNo = createUserAccountDetail.CurrentQuestionNumber == 0 ? 1 : createUserAccountDetail.CurrentQuestionNumber;
            }

            var nextQuestionDetail = await _moodleUser.GetNextQuestion(currentQuestionNo, userProfile.CreateUserAccountDetail.UserDetails, createUserAccountDetail.CurrentLanguageCode);

            if (nextQuestionDetail.IsQuestionFound)
            {
                userProfile = await SetCreateUserQuestionData(stepContext.Context, userProfile, nextQuestionDetail);
                return await SendUserQuestion(stepContext, nextQuestionDetail, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> SaveUserAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var userInput = stepContext.Result.ToString();
            var userAccountDetail = userProfile.CreateUserAccountDetail;

            var shouldProcessedQuestion = await ShouldProcessQuestion(userProfile, userInput, stepContext.Context);
            await SetQuestionDetails(userProfile, userInput, stepContext.Context, shouldProcessedQuestion);

            if (!userAccountDetail.IsLastQuestion || shouldProcessedQuestion)
            {
                return await stepContext.ReplaceDialogAsync(nameof(CreateUserAccountDialog), null, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> CreateUserInMoodle(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (!userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess && userProfile.UserDetail == null)
            {
                var isUserCreated = await _moodleUser.CreateUserInMoodle(MapUserAccountRequestDTO(userProfile.CreateUserAccountDetail));

                var messages = await _moodleUser.GetQuestionDetails(userProfile.CreateUserAccountDetail.CurrentLanguageCode);
                userProfile.IsUserCreated = isUserCreated;
                if (isUserCreated)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(messages[CreateUserMessageName.SUCCESS_ACCOUNT_CREATE].Question));
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(messages[CreateUserMessageName.UNSUCCESS_ACCOUNT_CREATE].Question));
                    userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess = true;
                }

                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }
            
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> UpdateUserInMoodle(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (!userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess && userProfile.UserDetail != null)
            {
                userProfile.CreateUserAccountDetail.UserDetails.Add(CreateUserMessageName.USER_ID.ToString(), userProfile.UserId.ToString());

                var isUserCreated = await _moodleUser.UpdateUserInMoodle(MapUserAccountRequestDTO(userProfile.CreateUserAccountDetail));

                var messages = await _moodleUser.GetQuestionDetails(userProfile.CreateUserAccountDetail.CurrentLanguageCode);
                userProfile.IsUserCreated = isUserCreated;
                if (isUserCreated)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(messages[CreateUserMessageName.SUCCESS_ACCOUNT_UPDATE].Question));
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(messages[CreateUserMessageName.UNSUCCESS_ACCOUNT_CREATE].Question));
                    userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess = true;
                }

                userProfile.IsVerifiedUser = true;
                userProfile.CreateUserAccountDetail.IsLastQuestion = true;
                userProfile.CreateUserAccountDetail.ShouldStopAccountCreationProcess = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }
            
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Private Method
        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _moodleUser = scope.ServiceProvider.GetRequiredService<IMoodleUser>();
            _emojis = scope.ServiceProvider.GetRequiredService<IEmojis>();
        }

        private async Task<UserProfile> SetCreateUserQuestionData(ITurnContext turnContext, UserProfile userProfile, CreateUserQuestionMessage questionMessage)
        {
            if (questionMessage != null)
            {
                if (userProfile.CreateUserAccountDetail == null)
                {
                    userProfile.CreateUserAccountDetail ??= new CreateUserAccountDetail();
                }

                userProfile.CreateUserAccountDetail.CurrentQuestionNumber = questionMessage.CurrentQuestionNumber;
                userProfile.CreateUserAccountDetail.QuestionValidationName = questionMessage.QuestionValidationName;
                userProfile.CreateUserAccountDetail.CurrentQuestionName = questionMessage.CurrentQuestionName;
                await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            }

            return userProfile;
        }

        private async Task SetQuestionDetails(UserProfile userProfile, string userInput, ITurnContext turnContext, bool shouldProcessQuestion)
        {
            var userAccountDetail = userProfile.CreateUserAccountDetail;
            if (shouldProcessQuestion)
            {
                userInput = string.Empty;
            }
            /*else
            {
                userInput = await GetAnswerOfChoiceQuestion(userAccountDetail.CurrentQuestionName, userInput);
            }*/

            if (userAccountDetail.UserDetails.ContainsKey(userAccountDetail.CurrentQuestionName))
            {
                userAccountDetail.UserDetails[userAccountDetail.CurrentQuestionName] = userInput;
            }
            else
            {
                userAccountDetail.UserDetails.Add(userAccountDetail.CurrentQuestionName, userInput);
            }

            if (userAccountDetail.CurrentQuestionName == CreateUserMessageName.FIRST_NAME.ToString() && 
            !userAccountDetail.UserDetails.ContainsKey(CreateUserMessageName.PASSWORD.ToString()))
            {
                userAccountDetail.UserDetails.Add(CreateUserMessageName.PASSWORD.ToString(), userAccountDetail.MobilePhoneNumber);
            }

            await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
        }
        /*
        private async Task<string> GetAnswerOfChoiceQuestion(string questionName, string userInput)
        {
            var enumQuestion = questionName.GetEnumFromValue<CreateUserMessageName>();
            return await Task.Run(() => {
                return enumQuestion switch
                {
                    CreateUserMessageName.GENDER => userInput.GetEnumFromValue<Gender>().ToString(),
                    CreateUserMessageName.HEALTH_WORKER_TYPE => userInput.GetEnumFromValue<HealthWorkerType>().ToString(),
                    _ => userInput
                };
            });
        }
        */
        private async Task<bool> ShouldProcessQuestion(UserProfile userProfile, string userInput, ITurnContext turnContext)
        {
            var userAccountDetail = userProfile.CreateUserAccountDetail;
            var shouldProcessQuestion = false;

            if (userAccountDetail.CurrentQuestionName == CreateUserMessageName.USER_DETAIL_SUMMARY.ToString())
            {
                if (userInput == "1")
                {
                    userAccountDetail.IsLastQuestion = true;
                }
                else if (userInput == "2")
                {
                    userAccountDetail.UserDetails = new Dictionary<string, string>();
                    userAccountDetail.CurrentQuestionNumber = 1;
                    shouldProcessQuestion = true;
                }
            }
            else if (userAccountDetail.CurrentQuestionName == CreateUserMessageName.TERMS_CONDITIONS.ToString())
            {
                if (userInput == "2")
                {
                    userAccountDetail.ShouldStopAccountCreationProcess = true;
                    userAccountDetail.IsLastQuestion = true;
                }
                else
                {
                    shouldProcessQuestion = true;
                    userAccountDetail.CurrentQuestionNumber++;
                }
            }
            else
            {
                await GetNextPreviousOption();
                if (nextPreviousOption[EmojiName.PAGE_PREVIOUS].Emojis == userInput)
                {
                    userAccountDetail.CurrentQuestionNumber -= 2;
                    shouldProcessQuestion = true;
                }
                else if (nextPreviousOption[EmojiName.PAGE_NEXT].Emojis == userInput)
                {
                    shouldProcessQuestion = true;
                }
                userAccountDetail.CurrentQuestionNumber++;
            }

            shouldProcessQuestion = (shouldProcessQuestion || userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"));
            await _userProfileStateAccessor.SetAsync(turnContext, userProfile);

            return shouldProcessQuestion;
        }

        private CreateUserAccountRequest MapUserAccountRequestDTO(CreateUserAccountDetail createUserAccountDetail)
        {
            return new CreateUserAccountRequest
            {
                UserDetails = createUserAccountDetail.UserDetails,
                WhatsAppId = createUserAccountDetail.WhatsAppId,
                MobileCountryCode = createUserAccountDetail.MobileCountryCode,
                MobilePhoneNumber = createUserAccountDetail.MobilePhoneNumber,
                CountryCode = createUserAccountDetail.CountryCode,
                LanguageCode = createUserAccountDetail.CurrentLanguageCode
            };
        }

        private async Task<Dictionary<EmojiName, BotEmojis>> GetNextPreviousOption()
        {
            if (nextPreviousOption == null || nextPreviousOption.Count <= 0)
            {
                nextPreviousOption = await _emojis.GetEmoji(EmojisType.Pagination);
            }

            return nextPreviousOption;
        }

        private async Task<DialogTurnResult> SendUserQuestion(WaterfallStepContext stepContext, CreateUserQuestionMessage nextQuestionDetail, CancellationToken cancellationToken)
        {
            var shouldSendMessageWithDelay = false;
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(nextQuestionDetail.Message),
                RetryPrompt = MessageFactory.Text(nextQuestionDetail.PropmtValidationMessage)
            };

            if (nextQuestionDetail.CurrentQuestionName == CreateUserMessageName.TERMS_CONDITIONS.ToString() && IsTermConditionWithPDFFormat(nextQuestionDetail.Message))
            {
                var activity = MessageFactory.Text("Term & Condition");
                activity.Attachments.Add(new Attachment("pdf", nextQuestionDetail.Message));
                promptOptions.Prompt = activity;
                shouldSendMessageWithDelay = true;
            }
            
            var result = await stepContext.PromptAsync("QuestionDetail", promptOptions, cancellationToken);

            if (shouldSendMessageWithDelay)
            {
                await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig("ImageWaitingTime")));
            }

            if (nextQuestionDetail.NextActionMessage.IsNotNullOrEmpty())
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(nextQuestionDetail.NextActionMessage));
            }

            return result;
        }

        private bool IsTermConditionWithPDFFormat(string fileUrl)
        {
            var result = false;
            try
            {
                if (fileUrl.IsNotNullOrEmpty())
                {
                   var extension = Path.GetExtension(fileUrl);
                    result = extension?.ToLower() == ".pdf";
                }
            }
            catch { }

            return result;
        }
        #endregion

        #region Validation
        private async Task<bool> QuestionDetailValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var questionDetail = userProfile.CreateUserAccountDetail;
            var result = true;
            
            if (questionDetail.QuestionValidationName.IsNotNullOrEmpty())
            {
                var methodInstance = this.GetType().GetMethod(questionDetail.QuestionValidationName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                result = Convert.ToBoolean(await (dynamic)methodInstance.Invoke(this, new object[] { promptcontext }));
            }
            return result;
        }

        private async Task<bool> ValidateWrongInputCount(PromptValidatorContext<string> promptcontext, bool result, string userInput)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
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

        private async Task<bool> ValidateFirstName(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var isNumber = int.TryParse(userInput, out _);
            var result = !isNumber;
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidateLastName(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var isNumber = int.TryParse(userInput, out _);
            var result = !isNumber || nextPreviousOption.Any(x => x.Key == EmojiName.PAGE_PREVIOUS && x.Value.Emojis == userInput);
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidatePassword(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var result = ((userInput.Length >= 6 && userInput.Length < 12) || nextPreviousOption.Any(x => x.Key == EmojiName.PAGE_PREVIOUS && x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }
        
        private async Task<bool> ValidateConfirmPassword(PromptValidatorContext<string> promptcontext)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var password = userProfile.CreateUserAccountDetail.UserDetails[CreateUserMessageName.PASSWORD.ToString()];
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var result = (password == userInput || nextPreviousOption.Any(x => x.Key == EmojiName.PAGE_PREVIOUS && x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidateEmail(PromptValidatorContext<string> promptcontext)
        {
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var result = (emailRegex.IsMatch(userInput) || nextPreviousOption.Any(x => x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }
        /*
        private async Task<bool> ValidateGenderOption(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var isNumber = int.TryParse(userInput, out int generChoiceOption);
            var result = ((isNumber && generChoiceOption.IsEnumContainsValue<Gender>()) || nextPreviousOption.Any(x => x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }
        
        private async Task<bool> ValidateBirthDate(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var result = false;
            await GetNextPreviousOption();
            if (userInput.Contains("-"))
            {
                try
                {
                    var datePart = userInput.Split('-').Select(x => Convert.ToInt32(x)).ToList();
                    var birthDate = new DateTime(datePart[0], datePart[1], datePart[2]);
                    var userMinimumAge = Convert.ToInt32(_configuration.GetMoodleConfig("UserMinimumAge"));
                    result = (DateTime.Today.AddYears(-userMinimumAge) > birthDate.Date);
                }
                catch { }
            }
            result = (result || nextPreviousOption.Any(x => x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidateHealthWorkerType(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var isNumber = int.TryParse(userInput, out int workerTypeChoice);
            var result = ((isNumber && workerTypeChoice.IsEnumContainsValue<HealthWorkerType>()) || nextPreviousOption.Any(x => x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidateHealthWorkerNumber(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            await GetNextPreviousOption();
            var result = ((userInput.Length >= 2 && userInput.Length < 20) || nextPreviousOption.Any(x => x.Value.Emojis == userInput));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }
        */
        private async Task<bool> ValidateRegistrationSummaryAction(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var isNumber = int.TryParse(userInput, out int summaryAction);
            var result = (isNumber && (summaryAction == 1 || summaryAction == 2));
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }

        private async Task<bool> ValidateTermPolicyConfirmation(PromptValidatorContext<string> promptcontext)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var isNumber = int.TryParse(userInput, out int summaryAction);
            var result = (isNumber && summaryAction == 1 || summaryAction == 2);
            return await ValidateWrongInputCount(promptcontext, result, userInput);
        }
        #endregion
    }
}
