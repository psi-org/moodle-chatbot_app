using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public class Activity : IActivity
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Consturctor
        public Activity(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<List<MoodleActivityDetail>> GetActivity(long userId, long courseId, long? activityId = null, long? CourseActivityId = null)
        {
            List<MoodleActivityDetail> moodleActivityDetails = null;
            try
            {
                var request = new MoodleActivityRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetActivity"),
                    UserId = userId,
                    CourseId = courseId,
                    ActivityId = activityId,
                    CourseActivityId = CourseActivityId
                };
                var result = await APICall.RunAsync<List<MoodleActivityDetail>, MoodleActivityRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleActivityDetails = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetActivity: Got an error while getting activity details from Moodle API for CourseId: {courseId}", exception, string.Empty, userId);
            }
            return moodleActivityDetails;
        }

        public async Task<MoodleGetNextQuestionDetail> GetNextQuestion(long userId, long quizId)
        {
            MoodleGetNextQuestionDetail moodleGetNextQuestionDetail = null;
            try
            {
                var request = new MoodleGetNextQuestionRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetNextQuestion"),
                    UserId = userId,
                    Quiz = quizId
                };
                var result = await APICall.RunAsync<MoodleGetNextQuestionDetail, MoodleGetNextQuestionRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleGetNextQuestionDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetNextQuestion: Got an error while getting quiz question from Moodle API for quizId: {quizId}", exception, string.Empty, userId);
            }

            return moodleGetNextQuestionDetail;
        }

        public async Task<MoodlePostQuizAnswerDetail> PostQuizAnswer(long userId, long quizAttemptsId, long answerId, long questionAttemptId, long currentPage)
        {
            MoodlePostQuizAnswerDetail postQuizAnswerDetail = null;
            try
            {
                var request = new PostQuizAnswerDetailRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("PostQuizAnswer"),
                    UserId = userId,
                    QuizAttemptsId = quizAttemptsId,
                    AnswerId = answerId,
                    QuestionAttemptId = questionAttemptId,
                    CurrentPage = currentPage
                };

                var result = await APICall.RunAsync<MoodlePostQuizAnswerDetail, PostQuizAnswerDetailRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);

                if (result.Success)
                {
                    postQuizAnswerDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetNextQuestion: Got an error while getting post quiz answer to Moodle API for quizAttemptsId: {quizAttemptsId} and questionAttemptId: {questionAttemptId}", exception, string.Empty, userId);
            }

            return postQuizAnswerDetail;
        }

        public async Task<MoodleQuizActivitySummaryDetail> GetQuizActivitySummary(long quizAttemptsId)
        {
            MoodleQuizActivitySummaryDetail quizActivitySummaryDetail = null;
            try
            {
                var request = new QuizActivitySummaryRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetQuizSummary"),
                    QuizAttemptId = quizAttemptsId
                };

                var result = await APICall.RunAsync<MoodleQuizActivitySummaryDetail, QuizActivitySummaryRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);

                if (result.Success)
                {
                    quizActivitySummaryDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetQuizActivitySummary: Got an error while getting quiz activity Summary from Moodle API for quizAttemptsId: {quizAttemptsId}", exception);
            }

            return quizActivitySummaryDetail;
        }

        public async Task<MoodleOpenLessonActivityDetail> GetOpenLessonActivity(long userId, long LessonId)
        {
            MoodleOpenLessonActivityDetail lessonActivityDetail = null;
            try
            {
                var request = new MoodleOpenLessonActivityRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("OpenLessonActivity"),
                    UserId = userId,
                    LessonId = LessonId
                };

                var result = await APICall.RunAsync<MoodleOpenLessonActivityDetail, MoodleOpenLessonActivityRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);

                if (result.Success)
                {
                    lessonActivityDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetOpenLessonActivity: Got an error while open lesson activity from Moodle API for LessonId: {LessonId}", exception, string.Empty, userId);
            }

            return lessonActivityDetail;
        }

        public async Task<MoodleLessonActivityDetail> GetLessonActivity(long userId, long PageId, long currentPage)
        {
            MoodleLessonActivityDetail lessonActivityDetail = null;
            try
            {
                var request = new MoodleLessonActivityRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetMultipleLessonDetail"),
                    UserId = userId,
                    PageId = PageId,
                    CurrentPage = currentPage
                };

                var result = await APICall.RunAsync<MoodleLessonActivityDetail, MoodleLessonActivityRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);

                if (result.Success)
                {
                    lessonActivityDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetLessonActivity: Got an error while getting lesson activity detail from Moodle API for pageId: {PageId} & CurrentPage is: {currentPage}", exception, string.Empty, userId);
            }

            return lessonActivityDetail;
        }

        public async Task<MoodleLessonActivitySummaryDetail> GetLessonActivitySummary(long userId, long LessonId)
        {
            MoodleLessonActivitySummaryDetail lessonActivityDetail = null;
            try
            {
                var request = new MoodleLessonActivitySummaryRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetLessonSummary"),
                    UserId = userId,
                    LessonId = LessonId,
                    Completed = 1
                };

                var result = await APICall.RunAsync<List<MoodleLessonActivitySummaryDetail>, MoodleLessonActivitySummaryRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);

                if (result.Success)
                {
                    lessonActivityDetail = result.Data?.FirstOrDefault();
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetLessonActivitySummary: Got an error while getting lesson activity summary from Moodle API for LessonId: {LessonId}", exception, string.Empty, userId);
            }

            return lessonActivityDetail;
        }

        public async Task<MoodleFeedbackQuestionDetail> GetFeedbackActivityQuestion(long userId, long feedbackId)
        {
            MoodleFeedbackQuestionDetail moodleFeedbackQuestionDetail = null;
            try
            {
                var request = new MoodleFeedbackQuestionDetailRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetFeedbackActivityQuestion"),
                    UserId = userId,
                    FeedbackId = feedbackId
                };
                var result = await APICall.RunAsync<MoodleFeedbackQuestionDetail, MoodleFeedbackQuestionDetailRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleFeedbackQuestionDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetFeedbackActivityQuestion: Got an error while getting feedback activity question from Moodle API for feedbackId: {feedbackId}", exception, string.Empty, userId);
            }

            return moodleFeedbackQuestionDetail;
        }

        public async Task<MoodlePostFeedbackAnswerDetail> PostFeedbackActivityAnswer(long userId, long feedbackQuestionId, string answer)
        {
            MoodlePostFeedbackAnswerDetail moodleFeedbackQuestionDetail = null;
            try
            {
                var request = new MoodlePostFeedbackAnswerRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("PostFeedbackAnswer"),
                    UserId = userId,
                    Itemid = feedbackQuestionId,
                    Value = answer
                };

                var result = await APICall.RunAsync<MoodlePostFeedbackAnswerDetail, MoodlePostFeedbackAnswerRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleFeedbackQuestionDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"PostFeedbackActivityAnswer: Got an error while getting posting feedback answer to Moodle API for feedbackQuestionId: {feedbackQuestionId}", exception, string.Empty, userId);
            }

            return moodleFeedbackQuestionDetail;
        }
        #endregion
    }
}
