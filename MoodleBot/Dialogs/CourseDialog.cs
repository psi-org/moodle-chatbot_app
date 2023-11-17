using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Business;
using MoodleBot.Business.Entity;
using MoodleBot.Business.Moodle;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Entity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoodleBot.Dialogs
{
    public class CourseDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly IServiceProvider _services;
        private readonly UserState _userState;
        private IMoodleCourse _moodleCourse;
        private IBusinessCommon _businessCommon;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public CourseDialog(UserState userState, ActivityDialog activityDialog, IServiceProvider services, IConfiguration configuration)
            : base(nameof(CourseDialog))
        {
            #region Create Instance
            _userState = userState;
            _services = services;
            _configuration = configuration;
            CreateServiceScoped();
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            #endregion

            #region Register Prompt
            AddDialog(new TextPrompt("CourseChoice", CourseChoiceValidation));
            AddDialog(new TextPrompt("SummaryAction", SummaryActionValidation));
            #endregion

            #region Register Dialog
            AddDialog(activityDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                CourseListStepAsync,
                ReviewSelectedCourseStepAsync,
                ShowCourseSummaryStepAsync,
                ReviewNextAcionStepAsync,
                FinalStepAsync
            })); 

            InitialDialogId = nameof(WaterfallDialog);

            #endregion
        }
        #endregion

        #region WaterFall dialog steps
        private async Task<DialogTurnResult> CourseListStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var courseDetail = userProfile.CourseDetail;
            
            if (courseDetail != null && courseDetail.ShouldContinueCourse)
            {
                courseDetail.ShouldContinueCourse = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(stepContext.Options, cancellationToken);
            }

            if (courseDetail != null && courseDetail.ShouldLoadFirstPage)
            {
                courseDetail.CurretPageNumber = 0;
                courseDetail.CourseIdOptionMapping = null;
            }

            var currentPage = courseDetail?.CurretPageNumber ?? 0;
            var courseMessage = await _moodleCourse.GetCourseMessageAsync(userProfile.UserId, userProfile.ShouldDisplayWelComeMessage, ++currentPage, userProfile.PreferredLanguageCode);
            
            if (!courseMessage.IsCourseAvailable)
            {
                await stepContext.Context.SendActivityAsync(courseMessage.GenericMessages[MessageName.COURSE_NOT_FOUND].Message);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            var genericMessage = courseMessage.GenericMessages;
            userProfile.ShouldDisplayWelComeMessage = false;
            userProfile = await SetCourseData(stepContext.Context, userProfile, courseMessage);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(string.Format(courseMessage.Message, userProfile.UserDetail.FirstName, userProfile.UserDetail.LastName)),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("CourseChoice", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewSelectedCourseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;
            var userInput = stepContext.Result.ToString();
            var pageOption = await _businessCommon.GetPaginationOption(userProfile.CourseDetail.CurretPageNumber, userProfile.CourseDetail.LastPageNumber);
            var shouldStartCourseDialog = false;
            
            if (pageOption.Values.Contains(userInput.ToLower()))
            {
                if (pageOption.FirstOrDefault(x => x.Value == userInput).Key == EmojiName.PAGE_PREVIOUS)
                {
                    userProfile.CourseDetail.CurretPageNumber -= 2;
                }
                userProfile.CourseDetail.ShouldLoadFirstPage = userProfile.CourseDetail.CurretPageNumber <= 0;
                shouldStartCourseDialog = true;
            }
            
            if (shouldStartCourseDialog || userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(CourseDialog), null, cancellationToken);
            }

            var courseSelectedOption = Convert.ToInt32(userInput);
            var currentCourseDetail = userProfile.CourseDetail.CourseIdOptionMapping[courseSelectedOption];
            userProfile.CourseDetail.CurrentCourseId = currentCourseDetail.CourseId;
            userProfile.CourseDetail.CurrentCourseImageUrl = currentCourseDetail.CourseImage;
            userProfile.CourseDetail.CurrentCourseName = currentCourseDetail.CourseName;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.BeginDialogAsync(nameof(ActivityDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowCourseSummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var courseDetail = userProfile.CourseDetail;
            
            if (courseDetail.ShouldShowCourseSummary)
            {
                var courseSummaryMessage = await _moodleCourse.GetCourseSummaryAsync(userProfile.UserId, courseDetail.CurrentCourseId, userProfile.PreferredLanguageCode);
                var genericMessage = courseSummaryMessage.GenericMessages;
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(string.Format(courseSummaryMessage.Message, userProfile.UserDetail.FirstName, userProfile.UserDetail.LastName)),
                    RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
                };

                courseDetail.ShouldShowCourseSummary = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);

                // Prompt the user for a choice.
                return await stepContext.PromptAsync("SummaryAction", promptOptions, cancellationToken);
            }
            else if(courseDetail.ShouldReturnOnCoursePage)
            {
                courseDetail.ShouldReturnOnCoursePage = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(CourseDialog), null, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewNextAcionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            if (stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                userProfile.CourseDetail.ShouldShowCourseSummary = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(null, cancellationToken);
            }
            
            var selectedAction = Convert.ToInt32(stepContext.Result);
            var courseSelectedOption = 0;
            if (selectedAction == 0)
            {
                userProfile.CourseDetail.ShouldContinueCourse = true;
                courseSelectedOption = userProfile.CourseDetail.CourseIdOptionMapping.FirstOrDefault(x => x.Value.CourseId == userProfile.CourseDetail.CurrentCourseId).Key;
            }

            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.ReplaceDialogAsync(nameof(CourseDialog), courseSelectedOption, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Set data in user Profile
        private async Task<UserProfile> SetCourseData(ITurnContext turnContext, UserProfile userProfile, CourseMessage courseMessage)
        {
            if (courseMessage.CourseIdOptionMapping != null)
            {
                if (userProfile.CourseDetail?.CourseIdOptionMapping == null)
                {
                    userProfile.CourseDetail ??= new CourseDetail();
                    userProfile.CourseDetail.CourseIdOptionMapping = courseMessage.CourseIdOptionMapping;
                }
                else
                {
                    foreach (var courseIdMapping in courseMessage.CourseIdOptionMapping)
                    {
                        if (!userProfile.CourseDetail.CourseIdOptionMapping.ContainsKey(courseIdMapping.Key))
                        {
                            userProfile.CourseDetail.CourseIdOptionMapping.Add(courseIdMapping.Key, courseIdMapping.Value);
                        }
                    }    
                }
                userProfile.CourseDetail.CurretPageNumber = courseMessage.CurrentPage;
                userProfile.CourseDetail.LastPageNumber = courseMessage.LastPage;
                userProfile.CourseDetail.ShouldLoadFirstPage = true;
                await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            }
            return userProfile;
        }
        #endregion

        #region Validation
        private async Task<bool> CourseChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var courseDetail = userProfile.CourseDetail;
            var isNumber = int.TryParse(userInput, out int courseSelectedOption);
            var result = (isNumber && courseDetail.CourseIdOptionMapping.ContainsKey(courseSelectedOption));
            
            if (!result)
            {
                var pageOption = await _businessCommon.GetPaginationOption(courseDetail.CurretPageNumber, courseDetail.LastPageNumber);
                result = pageOption.Values.Contains(userInput.ToLower());
            }

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

        private async Task<bool> SummaryActionValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int summaryActionSelectedOption);
            var result = (isNumber && (summaryActionSelectedOption == 0 || summaryActionSelectedOption == 1));

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
            _moodleCourse = scope.ServiceProvider.GetRequiredService<IMoodleCourse>();
            _businessCommon = scope.ServiceProvider.GetRequiredService<IBusinessCommon>();
        }
        #endregion
    }
}
