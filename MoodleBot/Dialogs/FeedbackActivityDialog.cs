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
    public class FeedbackActivityDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly IServiceProvider _services;
        private readonly UserState _userState;
        private IMoodleActivity _moodleActivity;
        private static string activityCompleteMessage = string.Empty;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public FeedbackActivityDialog(UserState userState, IServiceProvider services, IConfiguration configuration)
            : base(nameof(FeedbackActivityDialog))
        {
            #region Create Instance
            _userState = userState;
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            #endregion

            #region Register Prompt
            AddDialog(new TextPrompt("FeedbackChoice", QuizChoiceValidation));
            AddDialog(new TextPrompt("FeedbackFreeText"));
            #endregion

            #region Register Dialog
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                InitialStepAsync,
                LoopStepAsync,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            #endregion
        }
        #endregion

        #region WaterFall dialog steps
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var questionMessage = await _moodleActivity.GetFeedbackQuestionAsync(userProfile.UserId, userProfile.ActivityDetail.CurrentActivityId, userProfile.ActivityDetail.CurrentActivityInstanceId, userProfile.PreferredLanguageCode);

            if (!questionMessage.IsQuestionAvailable || questionMessage.IsActivityCompleted)
            {
                if (questionMessage.IsActivityCompleted)
                {
                    await stepContext.Context.SendActivityAsync(questionMessage.Message);
                }
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            var genericMessage = questionMessage.GenericMessages;
            activityCompleteMessage = genericMessage[MessageName.USER_GREETINGS_ON_ACTIVITY_COMPLETE].Message;

            if (!userProfile.ShouldDisplayNextQuestion)
            {
                userProfile.ShouldDisplayNextQuestion = true;
            }

            await SetFeedbackData(stepContext.Context, userProfile, questionMessage);

            var promptType = "FeedbackChoice";

            if (questionMessage.QuestionType == FeedbackQuestionType.TextField)
            {
                promptType = "FeedbackFreeText";
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(questionMessage.Message),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message),
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync(promptType, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            //If user press '#' then go back to previous page
            if (stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(FeedbackActivityDialog), null, cancellationToken);
            }

            var userAnswer = Convert.ToString(stepContext.Result);
            var feedbackDetail = userProfile.FeedbackActivityDetail;

            //Reply correct answer of last question
            if (feedbackDetail.QuestionType != FeedbackQuestionType.TextField)
            {
                var selectedOption = Convert.ToInt32(userAnswer);
                userAnswer = feedbackDetail.AnswerIdOptionMapping[selectedOption].Answer;
            }
            var feedbackAnswerDetail = await _moodleActivity.PostFeedbackAnswer(userProfile.UserId, userProfile.ActivityDetail.CurrentActivityId, feedbackDetail.FeedbackItemId, userAnswer);

            var answerFeed = string.Empty;
            
            if (feedbackAnswerDetail.IsLastQuestion)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.ReplaceDialogAsync(nameof(FeedbackActivityDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = activityCompleteMessage.IsNotNullOrEmpty() ? activityCompleteMessage : _configuration.GetMoodleMessageEmojis("UserGreetingsOnActivityComplete");
            await stepContext.Context.SendActivityAsync(message);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Set Quiz data in User profile
        private async Task<UserProfile> SetFeedbackData(ITurnContext turnContext, UserProfile userProfile, FeedbackQuestionMessage feedbackMessage)
        {
            var feedbackActivityDetail = userProfile.FeedbackActivityDetail ?? new FeedbackActivityDetail();
            feedbackActivityDetail.FeedbackItemId = feedbackMessage.FeedbackItemId ?? 0;
            feedbackActivityDetail.QuestionType = feedbackMessage.QuestionType;
            feedbackActivityDetail.AnswerIdOptionMapping = feedbackMessage.AnswerIdOptionMapping;
            userProfile.FeedbackActivityDetail = feedbackActivityDetail;
            await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            return userProfile;
        }
        #endregion

        #region Validation
        private async Task<bool> QuizChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var feedbackDetail = userProfile.FeedbackActivityDetail;
            var isNumber = int.TryParse(userInput, out int quizSelectedOption);
            var result = (isNumber && feedbackDetail.AnswerIdOptionMapping.ContainsKey(quizSelectedOption));

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
        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _moodleActivity = scope.ServiceProvider.GetRequiredService<IMoodleActivity>();
        }
        #endregion
    }
}
