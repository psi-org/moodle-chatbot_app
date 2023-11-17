using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Entity;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Business.Entity;
using MoodleBot.Business.Moodle;
using Microsoft.Extensions.Configuration;

namespace MoodleBot.Dialogs
{
    public class QuizActivityDialog : ComponentDialog
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
        public QuizActivityDialog(UserState userState, IServiceProvider services, IConfiguration configuration)
            : base(nameof(QuizActivityDialog))
        {
            #region Create Instance
            _userState = userState;
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _services = services;
            _configuration = configuration;
            CreateServiceScoped(); 
            #endregion

            #region Register Prompt
            AddDialog(new TextPrompt("QuizChoice", QuizChoiceValidation));
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
            var questionMessage = await _moodleActivity.GetQuizQuestionAsync(userProfile.UserId, userProfile.ActivityDetail.CurrentActivityInstanceId, userProfile.PreferredLanguageCode);

            if (!questionMessage.IsQuestionAvailable)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            var genericMessage = questionMessage.GenericMessages;
            activityCompleteMessage = genericMessage[MessageName.USER_GREETINGS_ON_ACTIVITY_COMPLETE].Message;

            if (!userProfile.ShouldDisplayNextQuestion)
            {
                userProfile.ShouldDisplayNextQuestion = true;
            }

            if(questionMessage.CurrentPage == 0){
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(questionMessage.NoOfActivityAttempts));
            }
            
            await SetQuizData(stepContext.Context, userProfile, questionMessage);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(questionMessage.Message),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("QuizChoice", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            //If user press '#' then go back to previous page
            if (stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(QuizActivityDialog), null, cancellationToken);
            }

            var answerSelectedOption = Convert.ToInt32(stepContext.Result);
            var quizDetail = userProfile.QuizActivityDetail;
            
            //Reply correct answer of last question
            var selectedAnswer = quizDetail.QuizIdOptionMapping[answerSelectedOption];
            var quizAnswerDetail = await _moodleActivity.PostQuizAnswer(userProfile.UserId, quizDetail.QuizAttemptsId.Value, selectedAnswer.AnswerId.Value, 
                quizDetail.QuestionAttemptId.Value, quizDetail.CurrentPage.Value);

            var answerFeed = string.Empty;
            if (!quizAnswerDetail.IsAnswerDetailFound)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            
            await stepContext.Context.SendActivityAsync(quizAnswerDetail.AnswerFeed);

            if (!quizAnswerDetail.IsLastQuestion)
            {
                return await stepContext.ReplaceDialogAsync(nameof(QuizActivityDialog), null, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = activityCompleteMessage.IsNotNullOrEmpty() ? activityCompleteMessage : _configuration.GetMoodleMessageEmojis("UserGreetingsOnActivityComplete");

            await stepContext.Context.SendActivityAsync(message);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Set Quiz data in User profile
        private async Task<UserProfile> SetQuizData(ITurnContext turnContext, UserProfile userProfile, QuizQuestionMessage quizMessage)
        {
            if (quizMessage.AnswerIdOptionMapping != null)
            {
                var quizActivityDetail = userProfile.QuizActivityDetail ?? new QuizActivityDetail();
                
                quizActivityDetail.QuizAttemptsId = quizMessage.QuizAttemptsId;
                quizActivityDetail.QuestionAttemptId = quizMessage.QuestionAttemptId;
                quizActivityDetail.CurrentPage = quizMessage.CurrentPage;
                quizActivityDetail.QuizIdOptionMapping = quizMessage.AnswerIdOptionMapping;
                userProfile.QuizActivityDetail = quizActivityDetail;
                await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            }
            return userProfile;
        }
        #endregion

        #region Validation
        private async Task<bool> QuizChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var quizDetail = userProfile.QuizActivityDetail;
            var isNumber = int.TryParse(userInput, out int quizSelectedOption);
            var result = (isNumber && quizDetail.QuizIdOptionMapping.ContainsKey(quizSelectedOption));

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
