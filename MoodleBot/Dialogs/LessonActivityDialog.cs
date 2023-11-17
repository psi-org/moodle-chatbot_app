using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Business.Entity;
using MoodleBot.Business.Moodle;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoodleBot.Dialogs
{
    public class LessonActivityDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly IServiceProvider _services;
        private readonly UserState _userState;
        private IMoodleActivity _moodleActivity;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public LessonActivityDialog(UserState userState, IServiceProvider services, IConfiguration configuration)
            : base(nameof(LessonActivityDialog))
        {
            #region Create Instance
            _userState = userState;
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            #endregion

            #region Register Prompt
            AddDialog(new TextPrompt("NextActionChoice", NextActionChoiceValidation));
            #endregion

            #region Register Dialog
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                NextActionStepAsync,
                TakeActionStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            #endregion
        }
        #endregion

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var lessonActivityDetail = userProfile.LessonActivityDetail;
            var isGenericMessageAvailable = (lessonActivityDetail != null && lessonActivityDetail.IsGenericMessageAvailable == true);
            var lessonDetail = await _moodleActivity.GetLessonActivityDetailAsync(userProfile.UserDetail.Userid, userProfile.ActivityDetail.CurrentActivityInstanceId,
                lessonActivityDetail?.PageId ?? null, lessonActivityDetail?.CurrentPage ?? null, !isGenericMessageAvailable, userProfile.PreferredLanguageCode);

            if (!lessonDetail.IsLessonDetailAvailable)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            if (!isGenericMessageAvailable)
            {
                var genericMessage = lessonDetail.GenericMessage;
                userProfile.LessonActivityDetail = new LessonActivityDetail
                {
                    IsGenericMessageAvailable = true,
                    LessonActionMessage = lessonDetail.ActionMessage,
                    InvalidInputMessage = genericMessage[MessageName.INVALID_INPUT].Message,
                    UserGreetingsOnActivityComplete = genericMessage[MessageName.USER_GREETINGS_ON_ACTIVITY_COMPLETE].Message
                };
            }

            userProfile.LessonActivityDetail.CurrentPage = lessonDetail.CurrentPage;
            userProfile.LessonActivityDetail.PageId = lessonDetail.PageId;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);

            foreach (var message in lessonDetail.Message)
            {
                await SendActivityMessage(stepContext, message);
                await Task.Delay(1000);
            }
            
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> NextActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (userProfile.LessonActivityDetail.PageId == 0)
            {
                userProfile.LessonActivityDetail.ShouldShowSummary = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(userProfile.LessonActivityDetail.LessonActionMessage),
                RetryPrompt = MessageFactory.Text(userProfile.LessonActivityDetail.InvalidInputMessage)
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("NextActionChoice", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> TakeActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            //If user press '#' then go back to previous page
            if (stepContext.Result?.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                return await stepContext.ReplaceDialogAsync(nameof(LessonActivityDialog), null, cancellationToken);
            }
            
            if (userProfile.LessonActivityDetail.ShouldShowSummary)
            {
                userProfile.LessonActivityDetail.ShouldShowSummary = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var selectedActionOption = Convert.ToInt32(stepContext.Result);
            if (selectedActionOption == 1)
            {
                return await stepContext.ReplaceDialogAsync(nameof(LessonActivityDialog), null, cancellationToken);
            }
            
            userProfile.ActivityDetail.ShouldReturnOnActivityPage = true;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activitySummaryCompressed = Convert.ToBoolean(_configuration.GetMoodleConfig("ActivitySummaryCompressed"));
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            
            var message = userProfile.LessonActivityDetail.UserGreetingsOnActivityComplete.IsNotNullOrEmpty() ? 
                            userProfile.LessonActivityDetail.UserGreetingsOnActivityComplete :
                            "👏 Congratulations! You have finished this activity.";

            if(!activitySummaryCompressed){
                await stepContext.Context.SendActivityAsync(message);
            }
            
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        #region Set Lesson activity data in User profile
        private async Task<UserProfile> SetLessonData(ITurnContext turnContext, UserProfile userProfile, LessonActivityMessage lessonMessage)
        {
            var lessonActivityDetail = userProfile.LessonActivityDetail;
            if (lessonActivityDetail == null)
            {
                lessonActivityDetail = new LessonActivityDetail();
            }
            lessonActivityDetail.LessonActionMessage = lessonMessage.ActionMessage;
            await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            return userProfile;
        }
        #endregion

        #region Validation
        private async Task<bool> NextActionChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int actionSelectedOption);
            var result = (isNumber && (actionSelectedOption == 0 || actionSelectedOption == 1));

            if (!result && userProfile.IsWrongInputAdded && userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                result = true;
            }

            if (!result && !userProfile.IsWrongInputAdded)
            {
                userProfile.IsWrongInputAdded = true;
                await _userProfileStateAccessor.SetAsync(promptcontext.Context, userProfile);
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
        private async Task SendActivityMessage(WaterfallStepContext stepContext, LessonMessageDetail lessonMessageDetail)
        {
            if (lessonMessageDetail.ContentType == LessonContentType.Image || lessonMessageDetail.ContentType == LessonContentType.Video)
            {
                var contentType = "video/mp4";
                var waitingTimeKey = "VideoWaitingTime";

                if (lessonMessageDetail.ContentType == LessonContentType.Image)
                {
                    contentType = "image/jpg";
                    waitingTimeKey = "ImageWaitingTime";
                }

                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment(contentType, lessonMessageDetail.Message)));
                await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig(waitingTimeKey)));
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{lessonMessageDetail.Message}"));
            }
        }

        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _moodleActivity = scope.ServiceProvider.GetRequiredService<IMoodleActivity>();
        }
        #endregion
    }
}
