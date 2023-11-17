using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodleBot.Business;
using MoodleBot.Business.Database;
using MoodleBot.Business.Entity;
using MoodleBot.Business.Moodle;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Entity;
using MoodleBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoodleBot.Dialogs
{
    public class ActivityDialog : ComponentDialog
    {
        #region Properties
        private readonly IStatePropertyAccessor<UserProfile> _userProfileStateAccessor;
        private readonly IServiceProvider _services;
        private readonly UserState _userState;
        private IMoodleActivity _moodleActivity;
        private IBusinessCommon _businessCommon;
        private IMoodleCertificate _moodleCertificate;
        private IEmojis _emojis;
        private IGenericMessages _genericMessages;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public ActivityDialog(
            LessonActivityDialog lessonActivityDialog, QuizActivityDialog quizActivityDialog,
            FeedbackActivityDialog feedbackActivityDialog,UserState userState, IServiceProvider services, IConfiguration configuration)
            : base(nameof(ActivityDialog))
        {
            #region Create Instance
            _userState = userState;
            _userProfileStateAccessor = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _services = services;
            _configuration = configuration;
           
            CreateServiceScoped();
            #endregion
            
            #region Register Prompt
            AddDialog(new TextPrompt("ActivityChoice", ActivityChoiceValidation));
            AddDialog(new TextPrompt("ActivityChoiceConfirmation", ActivityChoiceConfirmationValidation));
            AddDialog(new TextPrompt("NextActionConfirmation", NextActionConfirmationValidation));
            #endregion

            #region Register Dialog
            AddDialog(lessonActivityDialog);
            AddDialog(quizActivityDialog);
            AddDialog(feedbackActivityDialog);

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
                ActivityListStepAsync,
                ReviewSelectedActivityStepAsync,
                ConfirmSelectedActivityStepAsync,
                ShowActivitySummaryAndNextAction,
                ReviewNextAcion,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            #endregion
        }
        #endregion

        #region WaterFall dialog steps
        private async Task<DialogTurnResult> ActivityListStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var activityDetail = userProfile.ActivityDetail;

            if (userProfile.ActivityDetail != null && userProfile.ActivityDetail.ShouldStartActivity == true)
            {
                userProfile.CourseDetail.ShouldReturnOnCoursePage = false;
                userProfile.ActivityDetail.ShouldStartActivity = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(stepContext.Options, cancellationToken);
            }

            if (activityDetail != null && activityDetail.ShouldLoadFirstPage)
            {
                activityDetail.CurretPageNumber = 0;
                activityDetail.ActivityIdOptionMapping = null;
            }

            var courseSelectedOption = userProfile.CourseDetail.CourseIdOptionMapping.FirstOrDefault(x => x.Value.CourseId == userProfile.CourseDetail.CurrentCourseId).Key;
            var currentPage = activityDetail?.CurretPageNumber ?? 0;
            var activityMessage = await _moodleActivity.GetActivityMessageAsync(userProfile.UserId, userProfile.CourseDetail.CurrentCourseId, courseSelectedOption, ++currentPage, userProfile.PreferredLanguageCode);
            var genericMessage = activityMessage.GenericMessages;

            if (!activityMessage.IsActivityAvailable)
            {
                await stepContext.Context.SendActivityAsync(genericMessage[MessageName.ACTIVITY_NOT_FOUND].Message);
                userProfile.CourseDetail.ShouldReturnOnCoursePage = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            if (activityMessage.CurrentPage == 1)
            {
                await SendSelectedCourseImage(stepContext, userProfile);
            }

            userProfile = await SetActivityData(stepContext.Context, userProfile, activityMessage);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(string.Format(activityMessage.Message, courseSelectedOption)),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("ActivityChoice", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewSelectedActivityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;
            var userInput = stepContext.Result.ToString();
            var pageOption = await _businessCommon.GetPaginationOption(userProfile.ActivityDetail.CurretPageNumber, userProfile.ActivityDetail.LastPageNumber);
            var shouldStartActivityDialog = false;

            //Move on Next/Previous page
            if (pageOption.Values.Contains(userInput.ToLower()))
            {
                if (pageOption.FirstOrDefault(x => x.Value == userInput).Key == EmojiName.PAGE_PREVIOUS)
                {
                    userProfile.ActivityDetail.CurretPageNumber -= 2;
                }
                userProfile.ActivityDetail.ShouldLoadFirstPage = userProfile.ActivityDetail.CurretPageNumber <= 0;
                shouldStartActivityDialog = true;
            }

            //Display certificate to user of current course
            if (userProfile.ActivityDetail.ShouldShowCertificate)
            {
                var certificateEmojis = (await _emojis.GetEmoji(EmojiName.DOWNLOAD_CERTIFICATE)).FirstOrDefault();
                if (certificateEmojis.Value.Emojis == userInput.ToLower())
                {
                    var moodleCertificateDetail = await _moodleCertificate.GetCertificateDetail(userProfile.UserId, userProfile.CourseDetail.CurrentCourseId);
                    var genericMessages = await _genericMessages.GetGenericMessageByTypeId(userProfile.PreferredLanguageCode, MessageType.Course);
                    var isCertificateAvailable = false;
                    if (moodleCertificateDetail?.Count > 0)
                    {
                        await stepContext.Context.SendActivityAsync(genericMessages[MessageName.CERTIFICATE_WAITING_MESSAGE].Message);
                        var certificateDetail = await _moodleCertificate.PrepareCourseCertificate(moodleCertificateDetail, userProfile.UserId, userProfile.CourseDetail.CurrentCourseId);
                        if (certificateDetail.IsCertificateAvailable)
                        {
                            foreach (var certificateUrl in certificateDetail.CertificateUrl)
                            {
                                await SendCertificate(stepContext, certificateUrl);
                            }
                            isCertificateAvailable = true;
                        }
                    }

                    if(!isCertificateAvailable)
                    {
                        await stepContext.Context.SendActivityAsync(genericMessages[MessageName.CERTIFICATE_NOT_FOUND].Message);
                    }

                    userProfile.ActivityDetail.ShouldLoadFirstPage = true;
                    shouldStartActivityDialog = true;
                }
            }

            //If user press '#' then go back to previous page
            if (shouldStartActivityDialog || stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(ActivityDialog), null, cancellationToken);
            }

            var activityDetail = userProfile.ActivityDetail;
            var activitySelectedOption = Convert.ToInt32(stepContext.Result);
            
            //Go back on Course menu
            if (activitySelectedOption == 0)
            {
                userProfile.CourseDetail.ShouldReturnOnCoursePage = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            userProfile.ActivityDetail.CurrentActivityId = activityDetail.ActivityIdOptionMapping[activitySelectedOption];
            var activityConfirmationMessage = await _moodleActivity.GetActivityConfirmationMessageAsync(userProfile.UserId, userProfile.CourseDetail.CurrentCourseId, userProfile.ActivityDetail.CurrentActivityId, activitySelectedOption, userProfile.PreferredLanguageCode);
            var genericMessage = activityConfirmationMessage.GenericMessages;

            if (!activityConfirmationMessage.IsActivityAvailable)
            {
                await stepContext.Context.SendActivityAsync(genericMessage[MessageName.ACTIVITY_NOT_FOUND].Message);
                userProfile.CourseDetail.ShouldReturnOnCoursePage = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            if ((activityConfirmationMessage.CurrentActivityTypeId == ActivityType.Feedback &&
                        activityConfirmationMessage.IsFeedbackActivityCompleted) || activityConfirmationMessage.IsAccessRestriction)
            {
                await stepContext.Context.SendActivityAsync(activityConfirmationMessage.Message);
                return await stepContext.ReplaceDialogAsync(nameof(ActivityDialog), null, cancellationToken);
            }

            userProfile = await SetSelectedActivityDetails(stepContext.Context, userProfile, activityConfirmationMessage, activitySelectedOption);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(string.Format(activityConfirmationMessage.Message, activitySelectedOption)),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
            };

            // Show activity img
            await SendSelectedActivityImage(stepContext, userProfile);
            await Task.Delay(1000);

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("ActivityChoiceConfirmation", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmSelectedActivityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            if (stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(userProfile.ActivityDetail.CurrentActivitySelectedOption, cancellationToken);
            }

            var activitySelectedOption = Convert.ToInt32(stepContext.Result);

            if (activitySelectedOption == 0)
            {
                return await stepContext.ReplaceDialogAsync(nameof(ActivityDialog), null, cancellationToken);
            }

            userProfile.LessonActivityDetail = null;
            userProfile.QuizActivityDetail = null;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);

            return userProfile.ActivityDetail.CurrentActivityTypeId switch
            {
                ActivityType.Lesson => await stepContext.BeginDialogAsync(nameof(LessonActivityDialog), null, cancellationToken),
                ActivityType.Quiz => await stepContext.BeginDialogAsync(nameof(QuizActivityDialog), null, cancellationToken),
                ActivityType.Feedback => await stepContext.BeginDialogAsync(nameof(FeedbackActivityDialog), null, cancellationToken),
                _ => throw new Exception("Invalid Activity type Id")
            };
        }

        private async Task<DialogTurnResult> ShowActivitySummaryAndNextAction(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (userProfile.ActivityDetail.ShouldReturnOnActivityPage)
            {
                userProfile.ActivityDetail.ShouldReturnOnActivityPage = false;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.ReplaceDialogAsync(nameof(ActivityDialog), null, cancellationToken);
            }

            var quizLessonId = userProfile.ActivityDetail.CurrentActivityTypeId switch
            {
                ActivityType.Lesson => userProfile.ActivityDetail.CurrentActivityInstanceId,
                ActivityType.Quiz => userProfile.QuizActivityDetail?.QuizAttemptsId ?? 0,
                _ => 0
            };

            var shouldShowCertificate = (userProfile.ActivityDetail.ShouldShowCertificate && !userProfile.ActivityDetail.ShouldReturnOnActivitySummary);

            if (shouldShowCertificate)
            {
                var genericMessages = await _genericMessages.GetGenericMessageByTypeId(userProfile.PreferredLanguageCode, MessageType.Course);
                await stepContext.Context.SendActivityAsync(genericMessages[MessageName.CERTIFICATE_WAITING_MESSAGE].Message);
            }

            var summaryMessageDetail = await _moodleActivity.GetActivitySummaryAsync(userProfile.UserId, userProfile.ActivityDetail.CurrentActivityTypeId, 
                userProfile.ActivityDetail.IsLastActivity, quizLessonId, userProfile.PreferredLanguageCode, userProfile.CourseDetail.CurrentCourseId, shouldShowCertificate);

            var genericMessage = summaryMessageDetail.GenericMessages;
            var summaryMessage = await SendCertificateOnSummary(stepContext, summaryMessageDetail, genericMessage, shouldShowCertificate);

            userProfile.ActivityDetail.ShouldReturnOnActivitySummary = false;
            userProfile.ActivityDetail.NextActionIdOption = summaryMessageDetail.NextActionIdOption;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(summaryMessage),
                RetryPrompt = MessageFactory.Text(genericMessage[MessageName.INVALID_INPUT].Message)
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync("NextActionConfirmation", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ReviewNextAcion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.IsWrongInputAdded = false;

            if (stepContext.Result.ToString() == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                userProfile.ActivityDetail.ShouldReturnOnActivitySummary = true;
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var selectedOption = Convert.ToInt32(stepContext.Result);
            var activityActionIdOption = userProfile.ActivityDetail.NextActionIdOption;
            //Set data to show Course Summary
            if (activityActionIdOption[selectedOption] == ActivitySummaryAction.CourseSummary)
            {
                userProfile.CourseDetail.ShouldShowCourseSummary = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }

            //Go on course page or show course summary
            if (activityActionIdOption[selectedOption] == ActivitySummaryAction.CourseMenu || activityActionIdOption[selectedOption] == ActivitySummaryAction.CourseSummary)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var nextActivityOption = userProfile.ActivityDetail.CurrentActivitySelectedOption;

            //Set data to go on next activity
            if (activityActionIdOption[selectedOption] == ActivitySummaryAction.NextActivity)
            {
                ++nextActivityOption;
                var activityDetail = userProfile.ActivityDetail;
                if (!activityDetail.ActivityIdOptionMapping.ContainsKey(nextActivityOption))
                {
                    var courseSelectedOption = userProfile.CourseDetail.CourseIdOptionMapping.FirstOrDefault(x => x.Value.CourseId == userProfile.CourseDetail.CurrentCourseId).Key;
                    var currentPage = activityDetail?.CurretPageNumber ?? 0;
                    var activityMessage = await _moodleActivity.GetActivityMessageAsync(userProfile.UserId, userProfile.CourseDetail.CurrentCourseId, courseSelectedOption, ++currentPage, userProfile.PreferredLanguageCode);
                    userProfile = await SetActivityData(stepContext.Context, userProfile, activityMessage);
                }
            }

            //Continue with current or next activity
            userProfile.ActivityDetail.ShouldStartActivity = true;
            userProfile.ShouldDisplayNextQuestion = false;
            await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            return await stepContext.ReplaceDialogAsync(nameof(ActivityDialog), nextActivityOption, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileStateAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (!userProfile.CourseDetail.ShouldShowCourseSummary)
            {
                userProfile.CourseDetail.ShouldReturnOnCoursePage = true;
                await _userProfileStateAccessor.SetAsync(stepContext.Context, userProfile);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        #endregion

        #region Set data in user Profile
        private async Task<UserProfile> SetActivityData(ITurnContext turnContext, UserProfile userProfile, ActivityMessage activityMessage)
        {
            if (activityMessage.ActivityIdOptionMapping != null)
            {
                if (userProfile.ActivityDetail?.ActivityIdOptionMapping == null)
                {
                    userProfile.ActivityDetail ??= new ActivityDetail();
                    userProfile.ActivityDetail.ActivityIdOptionMapping = activityMessage.ActivityIdOptionMapping;
                }
                else
                {
                    foreach (var activityIdMapping in activityMessage.ActivityIdOptionMapping)
                    {
                        if (!userProfile.ActivityDetail.ActivityIdOptionMapping.ContainsKey(activityIdMapping.Key))
                        {
                            userProfile.ActivityDetail.ActivityIdOptionMapping.Add(activityIdMapping.Key, activityIdMapping.Value);
                        }
                    }
                }

                userProfile.ActivityDetail.CurretPageNumber = activityMessage.CurrentPage;
                userProfile.ActivityDetail.LastPageNumber = activityMessage.LastPage;
                userProfile.ActivityDetail.ShouldShowCertificate = activityMessage.ShouldShowCertificate;
                userProfile.ActivityDetail.ShouldLoadFirstPage = true;
                await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            }
            return userProfile;
        }
        private async Task<UserProfile> SetSelectedActivityDetails(ITurnContext turnContext, UserProfile userProfile, ActivityMessage activityMessage, int activitySelectedOption)
        {
            userProfile.ActivityDetail.CurrentActivitySelectedOption = activitySelectedOption;
            userProfile.ActivityDetail.CurrentActivityName = activityMessage.CurrentActivityName;
            userProfile.ActivityDetail.CurrentActivityImgUrl = activityMessage.CurrentActivityImgUrl;
            userProfile.ActivityDetail.CurrentActivityTypeId = activityMessage.CurrentActivityTypeId;
            userProfile.ActivityDetail.IsLastActivity = activityMessage.IsLastActivity;
            userProfile.ActivityDetail.CurrentActivityInstanceId = activityMessage.CurrentActivityInstanceId;
            userProfile.ActivityDetail.ShouldShowCertificate = activityMessage.ShouldShowCertificate;

            await _userProfileStateAccessor.SetAsync(turnContext, userProfile);
            return userProfile;
        }
        #endregion

        #region Validation
        private async Task<bool> ActivityChoiceValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var activityDetail = userProfile.ActivityDetail;
            var isNumber = int.TryParse(userInput, out int activitySelectedOption);
            var result = (isNumber && (activitySelectedOption == 0 || activityDetail.ActivityIdOptionMapping.ContainsKey(activitySelectedOption)));

            if (!result)
            {
                var pageOption = await _businessCommon.GetPaginationOption(activityDetail.CurretPageNumber, activityDetail.LastPageNumber);
                result = pageOption.Values.Contains(userInput.ToLower());
            }

            if (!result && userProfile.IsWrongInputAdded && userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
            {
                result = true;
            }

            if (!result && userProfile.ActivityDetail.ShouldShowCertificate)
            {
                var certificateEmojis = (await _emojis.GetEmoji(EmojiName.DOWNLOAD_CERTIFICATE)).FirstOrDefault();
                result = certificateEmojis.Value.Emojis == userInput.ToLower();
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

        private async Task<bool> ActivityChoiceConfirmationValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            await Task.Delay(1);
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(userInput, out int activitySelectedOption);
            var result = (isNumber && (activitySelectedOption == 0 || activitySelectedOption == 1));

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

        private async Task<bool> NextActionConfirmationValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            var userInput = promptcontext.Context.Activity.Text;
            var userProfile = await _userProfileStateAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            var isNumber = int.TryParse(promptcontext.Context.Activity.Text, out int activitySelectedOption);
            var nextActionIdOption = userProfile.ActivityDetail.NextActionIdOption;
            var result = (isNumber && nextActionIdOption != null && nextActionIdOption.ContainsKey(activitySelectedOption));

            if (!result && userProfile.IsWrongInputAdded && 
                userInput == _configuration.GetMoodleConfig("GoBackActionOnWrongInput"))
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
        private async Task SendSelectedCourseImage(WaterfallStepContext stepContext, UserProfile userProfile)
        {
            if (userProfile.CourseDetail.CurrentCourseImageUrl.IsNotNullOrEmpty())
            {
                var courseDetail = userProfile.CourseDetail;
                var activity = MessageFactory.Text(courseDetail.CurrentCourseName);
                activity.Attachments.Add(new Attachment("image/png", courseDetail.CurrentCourseImageUrl));
                await stepContext.Context.SendActivityAsync(activity);
                await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig("ImageWaitingTime")));
            }
        }
        private async Task SendSelectedActivityImage(WaterfallStepContext stepContext, UserProfile userProfile)
        {
            if (userProfile.ActivityDetail.CurrentActivityImgUrl.IsNotNullOrEmpty())
            {
                var activityDetail = userProfile.ActivityDetail;
                var activityImage = MessageFactory.Text($"*{activityDetail.CurrentActivitySelectedOption} - {activityDetail.CurrentActivityName}*");
                activityImage.Attachments.Add(new Attachment("image/png", activityDetail.CurrentActivityImgUrl));
                await stepContext.Context.SendActivityAsync(activityImage);
                await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig("ImageWaitingTime")));
            }
        }
        private void CreateServiceScoped()
        {
            var scope = _services.CreateScope();
            _moodleActivity = scope.ServiceProvider.GetRequiredService<IMoodleActivity>();
            _businessCommon = scope.ServiceProvider.GetRequiredService<IBusinessCommon>(); 
            _moodleCertificate = scope.ServiceProvider.GetRequiredService<IMoodleCertificate>();
            _emojis = scope.ServiceProvider.GetRequiredService<IEmojis>();
            _genericMessages = scope.ServiceProvider.GetRequiredService<IGenericMessages>();
        }
        private async Task<string> SendCertificateOnSummary(WaterfallStepContext stepContext, ActivitySummaryMessage summaryMessageDetail, Dictionary<MessageName, BotMessages> genericMessages, bool shouldShowCertificate)
        {
            var summaryMessage = string.Empty;
            var isCertificateAvailable = false;
            foreach (var message in summaryMessageDetail.Message)
            {
                if (message.IsMediaType)
                {
                    await SendCertificate(stepContext, message.Message);
                    isCertificateAvailable = true;
                }
                else
                {
                    summaryMessage = message.Message;
                }
            }

            if (shouldShowCertificate && !isCertificateAvailable)
            {
                await stepContext.Context.SendActivityAsync(genericMessages[MessageName.CERTIFICATE_NOT_FOUND].Message);
            }

            return summaryMessage;
        }
        private async Task SendCertificate(WaterfallStepContext stepContext, string fileUrl)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(new Attachment("pdf", fileUrl)));
            await Task.Delay(Convert.ToInt32(_configuration.GetMoodleConfig("ImageWaitingTime")));
        }
        #endregion
    }
}
