using Microsoft.Extensions.Configuration;
using MoodleBot.Business.Database;
using MoodleBot.Business.Entity;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.Entity;
using MoodleBot.Persistent.ExternalService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public class MoodleActivity : IMoodleActivity
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IActivity _activity;
        private readonly IGenericMessages _genericMessages;
        private readonly IEmojis _emojis;
        private readonly IConfiguration _configuration;
        private readonly IBusinessCommon _businessCommon;
        private readonly IMoodleCertificate _moodleCertificate;
        #endregion

        #region Constructor
        public MoodleActivity(IActivity activity, IGenericMessages genericMessages, ILogger logger, IEmojis emojis, IConfiguration configuration, IBusinessCommon businessCommon, IMoodleCertificate moodleCertificate)
        {
            _logger = logger;
            _activity = activity;
            _genericMessages = genericMessages;
            _emojis = emojis;
            _configuration = configuration;
            _businessCommon = businessCommon;
            _moodleCertificate = moodleCertificate;
        }
        #endregion

        #region Public Method

        #region Activity Method
        public async Task<ActivityMessage> GetActivityMessageAsync(long userId, long courseId, int selectedCourseOption, int pageNumber, string languageCode)
        {
            var activityMessage = new ActivityMessage
            {
                IsActivityAvailable = false,
            };

            try
            {
                var moodleActivityDetails = await _activity.GetActivity(userId, courseId);
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                activityMessage.GenericMessages = genericMessages;

                if (moodleActivityDetails?.Count > 0)
                {
                    var pageSize = Convert.ToInt32(_configuration.GetMoodleConfig("ActivityPageSize"));
                    var activityCompressed = Convert.ToBoolean(_configuration.GetMoodleConfig("ActivityCompressed"));
                    var (lastPageNumber, activities) = await _businessCommon.GetPageWiseRecord(moodleActivityDetails, pageNumber, pageSize);
                    activityMessage.Message = genericMessages[MessageName.ACTIVITY_LIST_INFO].Message;
                    var courseName = string.Empty;
                    var optionMapping = new Dictionary<int, int>();
                    var counter = ((pageNumber - 1) * pageSize);
                    var botEmojis = await _emojis.GetEmojis();
                    foreach (var activity in activities)
                    {
                        counter++;
                        var activityStatusEmoji = GetStatusWiseEmojis(botEmojis, activity.ActivityCompletionsStatusId);
                        var activityStatus = activity.ActivityCompletionStatus.IsNullOrEmpty() ? _configuration.GetMoodleConfig("UnknownStatusName") : activity.ActivityCompletionStatus;
                        var activityRestrictionMessage = "";//activity.LastAttemptState;
                        if (activity.IsAccessRestriction)
                        {
                            activityRestrictionMessage += $"```{botEmojis[EmojiName.RESTRICTED_ITEM].Emojis} {activity.AccessRestrictionMessage}```";
                        }
                        

                        if(activityCompressed){
                            activityMessage.Message += string.Format("*{0}* - {1}{2}\r\n\r\n", counter, activity.ActivityName?.Trim(), " " + activityRestrictionMessage);
                        }else{
                            activityMessage.Message += string.Format(
                                genericMessages[MessageName.ACTIVITY_LIST].Message, counter, activity.ActivityName?.Trim(), 
                                $"{activityStatusEmoji} {activityStatus}",
                                GetPercentageGrade(botEmojis, activity.ActivityPercentageGrade, activity.ActivityCompletionsStatusId),
                                GetAttempt(botEmojis, activity.NumOfAttempts), "\r\n      "+activityRestrictionMessage);
                        }
                        
                        optionMapping.Add(counter, activity.ActivityId);
                        courseName = activity.CourseShortName;
                    }

                    activityMessage.ShouldShowCertificate = moodleActivityDetails.Any(x => x.ShouldShowCertificate);
                    activityMessage.Message = string.Format(activityMessage.Message, selectedCourseOption, courseName);
                    activityMessage.Message = activityMessage.Message.Replace("$#ACTIVITY_COUNT#$", $"{moodleActivityDetails.Count()}");
                    activityMessage.ActivityIdOptionMapping = optionMapping;
                    activityMessage.Message += genericMessages[MessageName.ACTIVITY_GO_BACK_TO_COURSE].Message;

                    if (activityMessage.ShouldShowCertificate)
                    {
                        activityMessage.Message += string.Format(genericMessages[MessageName.DOWNLOAD_CERTIFICATE].Message, botEmojis[EmojiName.DOWNLOAD_CERTIFICATE].Emojis);
                    }

                    activityMessage.Message += await _businessCommon.GetPaginationMessage(pageNumber, lastPageNumber, languageCode);
                    
                    activityMessage.IsActivityAvailable = true;
                    activityMessage.CurrentPage = pageNumber;
                    activityMessage.LastPage = lastPageNumber;

                    _logger.Info($"GetActivityMessageAsync: Successfully got an activity details for CourseId: {courseId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetActivityMessageAsync: Activity details not found for CourseId: {courseId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetActivityMessageAsync: Got an error while prepare activity message for CourseId:{courseId}", exception, string.Empty, userId);
            }

            return activityMessage;
        }

        public async Task<ActivityMessage> GetActivityConfirmationMessageAsync(long userId, long courseId, long activityId, int selectedActivityOption, string languageCode)
        {
            var activityMessage = new ActivityMessage {
                IsActivityAvailable = false
            };

            try
            {
                var activities = await _activity.GetActivity(userId, courseId, activityId);
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                activityMessage.GenericMessages = genericMessages;
                if (activities?.Count > 0)
                {
                    var activity = activities.FirstOrDefault();
                    if (activity.ActivitytypeId == Convert.ToInt32(ActivityType.Feedback) &&
                        activity.ActivityCompletionsStatusId == Convert.ToInt32(ActivityStatus.Completed))
                    {
                        var botEmojis = await _emojis.GetEmojis();
                        activityMessage.Message = $"{botEmojis[EmojiName.ACTIVITY_COMPLETED].Emojis} {genericMessages[MessageName.FEEDBACK_ACTIVITY_ALREADY_COMPLETED].Message}";
                        activityMessage.IsFeedbackActivityCompleted = true;
                    }
                    else if (activity.IsAccessRestriction)
                    {
                        var botEmojis = await _emojis.GetEmojis();
                        activityMessage.Message = $"```{botEmojis[EmojiName.RESTRICTED_ITEM].Emojis} {activity.AccessRestrictionMessage}```";
                        activityMessage.IsAccessRestriction = true;
                    }
                    else
                    {
                        var activityName = activity.ActivityName.Trim();
                        activityMessage.CurrentActivityInstanceId = activity.InstanceId;
                        activityMessage.IsLastActivity = activity.IsLastActivity;
                        activityMessage.CurrentActivityName = activityName;
                        activityMessage.CurrentActivityImgUrl = activity.ActivityImg;
                        activityMessage.Message = string.Format("{0}\r\n\r\n", activity.ActivityDescription);
                        activityMessage.Message += string.Format(genericMessages[MessageName.ACTIVITY_SELECTION_CONFIRM].Message, activityName);
                        activityMessage.Message += genericMessages[MessageName.GO_BACK_TO_ACTIVITY].Message;
                    }

                    activityMessage.IsActivityAvailable = true;
                    activityMessage.CurrentActivityTypeId = (ActivityType)activity.ActivitytypeId;
                    activityMessage.ShouldShowCertificate = activity.ShouldShowCertificate;
                    _logger.Info($"GetActivityConfirmationMessageAsync: Successfully got an activity details for CourseId: {courseId} and activityId:{activityId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetActivityConfirmationMessageAsync: Activity details not found for CourseId:{courseId} and activityId:{activityId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetActivityConfirmationMessageAsync: Got an error while prepare activity confirmation message for CourseId:{courseId} and activityId:{activityId}", exception, string.Empty, userId);
            }

            return activityMessage;
        }

        public async Task<List<MoodleActivityDetail>> GetActivity(long userId, long courseId)
        {
            return await _activity.GetActivity(userId, courseId);
        }
        #endregion

        #region Quiz Activity Method
        public async Task<QuizQuestionMessage> GetQuizQuestionAsync(long userId, long quizId, string languageCode, long? activityId = null)
        {
            var questionMessage = new QuizQuestionMessage
            {
                IsQuestionAvailable = false,
            };

            try
            {
                var questionDetail = await _activity.GetNextQuestion(userId, quizId);
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                questionMessage.GenericMessages = genericMessages;

                if (questionDetail != default(MoodleGetNextQuestionDetail))
                {
                    questionMessage.Message = string.Format(genericMessages[MessageName.QUESTIION_TEMPLATE].Message, questionDetail.QuestionName, questionDetail.QuestionText);
                    var counter = 1;
                    var answerOptionMapping = new Dictionary<int, Entity.AnswerDetail>();

                    foreach (var answer in questionDetail.Answers)
                    {
                        questionMessage.Message += string.Format(genericMessages[MessageName.QUESTIION_OPTION_TEMPLATE].Message, counter, answer.Answer);
                        answerOptionMapping.Add(counter, new Entity.AnswerDetail
                        {
                            AnswerId = answer.AnswerId,
                            Answer = answer.Answer,
                            IsCorrectAnswer = answer.IsCorrectAnswer,
                            Feedback = answer.Feedback
                        });
                        counter++;
                    }

                    questionMessage.AnswerIdOptionMapping = answerOptionMapping;
                    questionMessage.ActivityImageUrl = questionDetail.ActivityImage;
                    questionMessage.QuizAttemptsId = questionDetail.QuizAttemptsId;
                    questionMessage.QuestionAttemptId = questionDetail.QuestionAttemptId;
                    questionMessage.CurrentPage = questionDetail.CurrentPage;
                    questionMessage.NoOfActivityAttempts = string.Format(genericMessages[MessageName.QUESTIION_ATTEMPT_COUNT].Message, questionDetail.ActivityAttempt);
                    questionMessage.IsQuestionAvailable = true;

                    _logger.Info($"GetQuizQuestionAsync: Successfully found quiz question details for activityId:{activityId} and quizId: {quizId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetQuizQuestionAsync: quiz question details not found for activityId:{activityId} and quizId: {quizId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetQuizQuestionAsync: Got an error while get quiz question for activityId:{activityId} and quizId: {quizId}", exception, string.Empty, userId);
            }

            return questionMessage;
        }

        public async Task<QuizAnswerMessage> PostQuizAnswer(long userId, long quizAttemptsId, long answerId, long questionAttemptId, long currentPage)
        {
            var quizAnswerMessage = new QuizAnswerMessage {
                IsAnswerDetailFound = false
            };
            try
            {
                var postQuizAnswerDetail = await _activity.PostQuizAnswer(userId, quizAttemptsId, answerId, questionAttemptId, currentPage);
                if (postQuizAnswerDetail != default(MoodlePostQuizAnswerDetail))
                {
                    var botEmojis = await _emojis.GetEmojis();

                    quizAnswerMessage.AnswerStatus = postQuizAnswerDetail.AnswerStatus;
                    quizAnswerMessage.AnswerFeed = postQuizAnswerDetail.AnswerFeed;
                    quizAnswerMessage.IsLastQuestion = postQuizAnswerDetail.IsLastQuestion;
                    quizAnswerMessage.IsAnswerDetailFound = true;

                    var answerStatusEmoji = botEmojis[EmojiName.COURSE_ENROLLED].Emojis;
                    var answerStatus = _configuration.GetMoodleMessageEmojis("QuizAnswerSubmitted");

                    if(postQuizAnswerDetail.ShowAnswerStatus){
                        answerStatusEmoji = botEmojis[EmojiName.ANSWER_INCORRECT].Emojis;
                        answerStatus = _configuration.GetMoodleMessageEmojis("QuizAnswerInCorrect");
                        if (quizAnswerMessage.AnswerStatus.ToLower() == "correct"){
                            answerStatusEmoji = botEmojis[EmojiName.ANSWER_CORRECT].Emojis;
                            answerStatus = _configuration.GetMoodleMessageEmojis("QuizAnswerCorrect");
                        }
                        
                        if(postQuizAnswerDetail.ShowAnswerFeedback){
                            quizAnswerMessage.AnswerFeed = $"{answerStatusEmoji} {answerStatus}\r\n{quizAnswerMessage.AnswerFeed}";
                        }
                        else{
                            quizAnswerMessage.AnswerFeed = $"{answerStatusEmoji} {answerStatus}";
                        }
                    }
                    else{
                        if(postQuizAnswerDetail.ShowAnswerFeedback){
                            quizAnswerMessage.AnswerFeed = $"{answerStatusEmoji} {answerStatus}\r\n{quizAnswerMessage.AnswerFeed}";
                        }
                        else{
                            quizAnswerMessage.AnswerFeed = $"{answerStatusEmoji} {answerStatus}";
                        }
                    }
                    _logger.Info($"PostQuizAnswer: Successfully found quiz answer details for quizAttemptsId:{quizAttemptsId} and questionAttemptId: {questionAttemptId}");
                }
                else
                {
                    _logger.Info($"PostQuizAnswer: Quiz answer detail is not found for quizAttemptsId:{quizAttemptsId} and questionAttemptId: {questionAttemptId}");
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"PostQuizAnswer: Got an error while post quiz answer for quizAttemptsId:{quizAttemptsId} and questionAttemptId: {questionAttemptId}", exception, string.Empty, userId);
            }

            return quizAnswerMessage;
        }
        #endregion

        #region Lesson Activity Method
        public async Task<OpenLessonActivityDetail> GetOpenLessonActivityAsync(long userId, long LessonId)
        {
            var openLessonActivityDetail = new OpenLessonActivityDetail();
            try
            {
                var response = await _activity.GetOpenLessonActivity(userId, LessonId);
                if (response != null)
                {
                    openLessonActivityDetail.StartPageId = response.StartPageId;
                    openLessonActivityDetail.LastSeenPageId = response.LastSeenPageId;

                    _logger.Info($"GetOpenLessonActivityAsync: Successfully got open lesson detail for LessonId: {LessonId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetOpenLessonActivityAsync: Open lesson detail is not found for LessonId: {LessonId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetOpenLessonActivityAsync: Got an error while getting open lesson detail for lesson id: {LessonId}", exception, string.Empty, userId);
            }

            return openLessonActivityDetail;
        }

        public async Task<LessonActivityMessage> GetLessonActivityDetailAsync(long userId, long lessonId, long? pageId, long? currentPage, bool shouldReturnGenericMessage, string languageCode)
        {
            var lessonActivityMessage = new LessonActivityMessage {
                Message = new List<LessonMessageDetail>(),
                IsLessonDetailAvailable = false
            };

            try
            {
                if (pageId == null || currentPage == null)
                {
                    var openLessonDetail = await GetOpenLessonActivityAsync(userId, lessonId);
                    pageId = openLessonDetail.LastSeenPageId == 0 ? openLessonDetail.StartPageId : openLessonDetail.LastSeenPageId;
                    currentPage = openLessonDetail.StartPageId;
                }

                var lessonDetail = await _activity.GetLessonActivity(userId, pageId.Value, currentPage.Value);
                
                if (lessonDetail != null && lessonDetail.Contents?.Count > 0)
                {
                    if (shouldReturnGenericMessage)
                    {
                        var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                        lessonActivityMessage.ActionMessage = $"{genericMessages[MessageName.LESSON_ACTION_CONTINUE].Message}{genericMessages[MessageName.LESSON_ACTION_GO_BACK_TO_ACTIVITY].Message}";
                        lessonActivityMessage.GenericMessage = genericMessages;
                    }
                    
                    lessonActivityMessage.CurrentPage = lessonDetail.CurrentPage;
                    lessonActivityMessage.PageId = lessonDetail.NextPageId;

                    lessonActivityMessage.Message.Add(new LessonMessageDetail { 
                        Message = $"*{lessonDetail.Title}*",
                        ContentType = LessonContentType.Text
                    });

                    foreach (var content in lessonDetail.Contents)
                    {
                        if (content.PageContent.IsNotNullOrEmpty())
                        {
                            var contentType = IsValidateContentType(content.TypeId, content.PageContent);
                            lessonActivityMessage.Message.Add(new LessonMessageDetail
                            {
                                Message = contentType != LessonContentType.Text ? content.PageContent : content.PageContent.Replace("\\r", "\r").Replace("\\n", "\n"),
                                ContentType = contentType
                            }); 
                        }
                    }
                    
                    lessonActivityMessage.IsLessonDetailAvailable = true;

                    _logger.Info($"GetLessonActivityDetailAsync: Successfully found Lesson detail for lessonId:{lessonId}, CurrentPageId: {currentPage} & NextPageId: {pageId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetLessonActivityDetailAsync: Lesson detail is not found for lessonId:{lessonId}, CurrentPageId: {currentPage} & NextPageId: {pageId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetLessonActivityDetailAsync: Got an error while get lesson detail for lessonId:{lessonId}, CurrentPageId: {currentPage} & NextPageId: {pageId}", exception, string.Empty, userId);
            }

            return lessonActivityMessage;
        }
        #endregion

        #region Feedback Activity
        public async Task<FeedbackQuestionMessage> GetFeedbackQuestionAsync(long userId, long activityId, long feedbackItemId, string languageCode)
        {
            var questionMessage = new FeedbackQuestionMessage
            {
                IsQuestionAvailable = false,
            };

            try
            {
                var questionDetail = await _activity.GetFeedbackActivityQuestion(userId, feedbackItemId);
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                questionMessage.GenericMessages = genericMessages;
                
                if (questionDetail.Completed)
                {
                    var botEmojis = await _emojis.GetEmojis();
                    questionMessage.Message = $"{botEmojis[EmojiName.ACTIVITY_COMPLETED].Emojis} {genericMessages[MessageName.FEEDBACK_ACTIVITY_ALREADY_COMPLETED].Message}";
                    questionMessage.IsActivityCompleted = questionDetail.Completed;
                }
                else if (questionDetail != default(MoodleFeedbackQuestionDetail))
                {
                    questionMessage.Message = string.Format(genericMessages[MessageName.QUESTIION_TEMPLATE].Message, $"Q{questionDetail.ItemPosition}", questionDetail.FeedbackQuestion);
                    questionMessage.QuestionType = questionDetail.ItemType.GetEnumFromDescription<FeedbackQuestionType>();

                    if (questionMessage.QuestionType != FeedbackQuestionType.TextField)
                    {
                        var counter = 1;
                        var answerOptionMapping = new Dictionary<int, FeedbackChoiceDetail>();
                        foreach (var choice in questionDetail.Choices)
                        {
                            questionMessage.Message += string.Format(genericMessages[MessageName.QUESTIION_OPTION_TEMPLATE].Message, counter, choice.Choice);
                            answerOptionMapping.Add(counter, new FeedbackChoiceDetail
                            {
                                AnswerId = counter,
                                Answer = choice.Choice,
                            });
                            counter++;
                        }

                        questionMessage.AnswerIdOptionMapping = answerOptionMapping;
                    }

                    questionMessage.ActivityImageUrl = questionDetail.ActivityImage;
                    questionMessage.FeedbackItemId = questionDetail.QuestionId;
                    questionMessage.IsQuestionAvailable = true;
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetFeedbackQuestionAsync: Got an error while get feedback question for activityId:{activityId} and feedbackItemId: {feedbackItemId}", exception, string.Empty, userId);
            }

            return questionMessage;
        }

        public async Task<FeedbackAnswerMessage> PostFeedbackAnswer(long userId, long activityId, long feedbackItemId, string answer)
        {
            var feedbackAnswerMessage = new FeedbackAnswerMessage
            {
                IsAnswerDetailFound = false
            };

            try
            {
                var postFeedbackAnswerDetail = await _activity.PostFeedbackActivityAnswer(userId, feedbackItemId, answer);
                if (postFeedbackAnswerDetail != default(MoodlePostFeedbackAnswerDetail))
                {
                    feedbackAnswerMessage.ErrorMessage = postFeedbackAnswerDetail.EndMessage;
                    feedbackAnswerMessage.IsLastQuestion = postFeedbackAnswerDetail.IsLastQuestion;
                    feedbackAnswerMessage.IsAnswerDetailFound = postFeedbackAnswerDetail.EndMessage.IsNullOrEmpty();

                    _logger.Info($"PostFeedbackAnswer: Successfully found feedback answer details for feedbackItemId:{feedbackItemId} and activityId: {activityId}", null, userId);
                }
                else
                {
                    _logger.Info($"PostFeedbackAnswer: Feedback answer detail is not found for feedbackItemId:{feedbackItemId} and activityId: {activityId}", null, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"PostFeedbackAnswer: Got an error while post feedback answer for feedbackItemId:{feedbackItemId} and activityId: {activityId}", exception, string.Empty, userId);
            }

            return feedbackAnswerMessage;
        }
        #endregion

        public async Task<ActivitySummaryMessage> GetActivitySummaryAsync(long userId, ActivityType activityTypeId, bool IsLastActivity, long quizLessonId, string languageCode, long courseId, bool shouldShowCertificate)
        {
            var activitySummaryCompressed = Convert.ToBoolean(_configuration.GetMoodleConfig("ActivitySummaryCompressed"));

            var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
            var botEmojis = await _emojis.GetEmojis();
            var summaryMessage = activityTypeId switch
            {
                ActivityType.Lesson => await GetLessonActivitySummaryMessage(userId, quizLessonId, genericMessages),
                ActivityType.Quiz => await GetQuizActivitySummaryMessage(userId, quizLessonId, genericMessages, botEmojis),
                _ => string.Empty
            };

            if (summaryMessage.IsNotNullOrEmpty()){
                summaryMessage = $"{botEmojis[EmojiName.SUMMARY].Emojis} {summaryMessage}";
            }

            var nextActionIdOption = new Dictionary<int, ActivitySummaryAction> ();
            var counter = 0;

            if(!activitySummaryCompressed){
                nextActionIdOption.Add(1, ActivitySummaryAction.CourseMenu);
                nextActionIdOption.Add(2, ActivitySummaryAction.CourseSummary);
                summaryMessage += string.Format(
                        genericMessages[MessageName.SUMMARY_ACTION].Message,
                        string.Format(genericMessages[MessageName.ACTIVITY_ACTION_COURSE_MENU].Message, 1),
                        string.Format(genericMessages[MessageName.ACTIVITY_ACTION_SHOW_COURSE_SUMMARY].Message, 2));

                counter = 2;

                if (!IsLastActivity){
                    ++counter;
                    nextActionIdOption.Add(counter, ActivitySummaryAction.NextActivity);
                    summaryMessage += string.Format(genericMessages[MessageName.ACTIVITY_ACTION_NEXT_ACTIVITY].Message, counter);
                    summaryMessage += "\r\n";
                }

                if (activityTypeId != ActivityType.Feedback){
                    ++counter;
                    nextActionIdOption.Add(counter, ActivitySummaryAction.RepetActivity);
                    summaryMessage += $"{string.Format(genericMessages[MessageName.ACTIVITY_ACTION_REPEAT_ACTIVITTY].Message, counter)}";
                }
            }else{
                if (IsLastActivity){
                    nextActionIdOption.Add(1, ActivitySummaryAction.CourseMenu);
                    summaryMessage += $"Type *1* to go to the course list \r\n";

                    nextActionIdOption.Add(2, ActivitySummaryAction.CourseSummary);
                    summaryMessage += $"Type *2* to show course summary \r\n";

                    counter = 2;
                }else{
                    ++counter;
                    nextActionIdOption.Add(counter, ActivitySummaryAction.NextActivity);
                    summaryMessage += $"Type *{counter}* to continue \r\n";
                }

                if (activityTypeId != ActivityType.Feedback){
                    ++counter;
                    nextActionIdOption.Add(counter, ActivitySummaryAction.RepetActivity);
                    summaryMessage += $"Type *{counter}* to repeat this module";
                }
            }
            var activitySummaryMessage = new ActivitySummaryMessage
            {
                Message = new List<SummaryMessage>(),
                GenericMessages = genericMessages,
                NextActionIdOption = nextActionIdOption
            };

            if (shouldShowCertificate)
            {
                var certificateDetail = await _moodleCertificate.PrepareCourseCertificate(userId, courseId);
                if (certificateDetail?.IsCertificateAvailable == true)
                {
                    foreach (var url in certificateDetail.CertificateUrl)
                    {
                        activitySummaryMessage.Message.Add(new SummaryMessage { Message = url, IsMediaType = true });
                    }
                }
            }

            activitySummaryMessage.Message.Add(new SummaryMessage { Message = summaryMessage, IsMediaType = false });

            return activitySummaryMessage;
        }
        #endregion

        #region Private Method
        private async Task<string> GetQuizActivitySummaryMessage(long userId, long quizAttemptId, Dictionary<MessageName, BotMessages> genericMessages, Dictionary<EmojiName, BotEmojis> emojis)
        {
            var messages = string.Empty;
            var activitySummaryCompressed = Convert.ToBoolean(_configuration.GetMoodleConfig("ActivitySummaryCompressed"));

            try
            {
                var quizActivitySymmary = await _activity.GetQuizActivitySummary(quizAttemptId);
                if (quizActivitySymmary != default(MoodleQuizActivitySummaryDetail))
                {
                    if(activitySummaryCompressed){
                        messages += string.Format("*Module Summary:*\r\n\r\n*Marks:* {0} out of {1}\r\n*Grade:* {2}\r\n\r\n",
                            quizActivitySymmary.AttemptFinalGrade,
                            quizActivitySymmary.ActivityGradeMax,
                            quizActivitySymmary.AttemptPercentageGrade);
                    }else{
                        messages += string.Format(genericMessages[MessageName.ACTIVITY_SUMMARY].Message,
                                            quizActivitySymmary.TimeStarted.ToString("dd-MM-yyyy h:mm tt"),
                                            quizActivitySymmary.TimeFinished?.ToString("dd-MM-yyyy h:mm tt"),
                                            $"{quizActivitySymmary.TimeTakenInMinutes ?? 0} Minutes");

                        messages += string.Format(genericMessages[MessageName.QUIZ_SUMMARY].Message,
                            quizActivitySymmary.AttemptFinalGrade,
                            quizActivitySymmary.ActivityGradeMax,
                            quizActivitySymmary.AttemptPercentageGrade,
                            $"{emojis[EmojiName.ATTEMPT].Emojis} {quizActivitySymmary.Attempt}",
                            quizActivitySymmary.AttemptState);
                    }

                    _logger.Info($"GetQuizActivitySummaryMessage: Successfully found Quiz summary detail for QuizAttemptId: {quizAttemptId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetQuizActivitySummaryMessage: Quiz summary detail is not found for QuizAttemptId: {quizAttemptId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                messages = string.Empty;
                _logger.Error($"GetQuizActivitySummaryMessage: Got an error while getting QuizSummary for QuizAttemptId: {quizAttemptId}", exception, string.Empty, userId);
            }
            return messages;
        }

        private async Task<string> GetLessonActivitySummaryMessage(long userId, long lessonId, Dictionary<MessageName, BotMessages> genericMessages)
        {
            var messages = string.Empty;
            try
            {
                var activitySummaryCompressed = Convert.ToBoolean(_configuration.GetMoodleConfig("ActivitySummaryCompressed"));
                var lessonActivitySymmary = await _activity.GetLessonActivitySummary(userId, lessonId);
                var attempts = lessonActivitySymmary?.Attempts?.FirstOrDefault();
                if (lessonActivitySymmary != default(MoodleLessonActivitySummaryDetail) && attempts != null)
                {
                    if(!activitySummaryCompressed){
                        messages = string.Format(genericMessages[MessageName.ACTIVITY_SUMMARY].Message, attempts.StartTime, attempts.EndTime, $"{attempts.Duration}");
                    }
                    _logger.Info($"GetLessonActivitySummaryMessage: Successfully found Lesson summary detail for lessonId: {lessonId}", string.Empty, userId);
                }
                else
                {
                    _logger.Info($"GetLessonActivitySummaryMessage: Lesson summary detail is not found for lessonId: {lessonId}", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                messages = string.Empty;
                _logger.Error($"GetLessonActivitySummaryMessage: Got an error while getting LessonSummary for lessonId: {lessonId}", exception, string.Empty, userId);
            }
            return messages;
        }

        private LessonContentType IsValidateContentType(LessonContentType contentType, string value)
        {
            var result = LessonContentType.Text;

            if (contentType == LessonContentType.Image || contentType == LessonContentType.Video)
            {
                var extension = Path.GetExtension(value)?.ToLower();
                result = extension == ".png" || extension == ".jpg" || extension == ".jpeg" ? LessonContentType.Image :
                         extension == ".mp4" ? LessonContentType.Video : LessonContentType.Text;
            }

            return result;
        }

        private string GetStatusWiseEmojis(Dictionary<EmojiName, BotEmojis> emojis, int activityStatusId)
        {
            var emoji = emojis[EmojiName.STATUS_UNKNOWN].Emojis;

            var activityStatusEmoji = emojis.Values.Where(x => x.EmojisTypeId == EmojisType.ActivityStatus);
            if (activityStatusEmoji.Any(x => x.StatusId == activityStatusId))
            {
                emoji = activityStatusEmoji.Where(x => x.StatusId == activityStatusId).FirstOrDefault().Emojis;
            }

            return emoji;
        }

        private string GetPercentageGrade(Dictionary<EmojiName, BotEmojis> emojis, double percentageGrade, int activityStatusId)
        {
            var grade = " ";
            if (activityStatusId == Convert.ToInt16(ActivityStatus.Completed) ||
                activityStatusId == Convert.ToInt16(ActivityStatus.CompletePass) ||
                activityStatusId == Convert.ToInt16(ActivityStatus.CompleteFail))
            {
                if(activityStatusId == Convert.ToInt16(ActivityStatus.CompletePass)){
                    grade = emojis[EmojiName.GRADE_GREEN].Emojis;
                    grade = $"{grade} {percentageGrade}%";
                }
                else if(activityStatusId == Convert.ToInt16(ActivityStatus.CompleteFail)){
                    grade = emojis[EmojiName.GRADE_RED].Emojis;
                    grade = $"{grade} {percentageGrade}%";
                }
                else{
                    grade = (percentageGrade > 0 ? $"{percentageGrade}%" : " ");
                }
            }

            return grade;
        }

        private string GetAttempt(Dictionary<EmojiName, BotEmojis> emojis, int? numberOfAttempt)
        {
            var attempt = " ";

            if (numberOfAttempt.HasValue && numberOfAttempt.Value > 0)
            {
                attempt = $"{emojis[EmojiName.ATTEMPT].Emojis} {numberOfAttempt},";
            }

            return attempt;
        }
        #endregion
    }
}
