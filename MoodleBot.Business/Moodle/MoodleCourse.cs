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
using System.Linq;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public class MoodleCourse : IMoodleCourse
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly ICourse _course;
        private readonly IEmojis _emojis;
        private readonly IGenericMessages _genericMessages;
        private readonly IConfiguration _configuration;
        private readonly IBusinessCommon _businessCommon;
        #endregion

        #region Constructor
        public MoodleCourse(ICourse course, IGenericMessages genericMessages, ILogger logger, IEmojis emojis, IConfiguration configuration, IBusinessCommon businessCommon)
        {
            _course = course;
            _emojis = emojis;
            _genericMessages = genericMessages;
            _logger = logger;
            _configuration = configuration;
            _businessCommon = businessCommon;
        }
        #endregion

        #region Public Method
        public async Task<CourseMessage> GetCourseMessageAsync(long userId, bool shouldAddWelComeMessage, int pageNumber, string languageCode, long? courseId = null)
        {
            var courseMessage = new CourseMessage
            { 
                IsCourseAvailable = false
            };

            try
            {
                var moodleCourseDetails = await _course.GetCourses(userId, courseId);
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                courseMessage.GenericMessages = genericMessages;
                if (moodleCourseDetails?.Count > 0)
                {
                    var botEmojis = await _emojis.GetEmojis();
                    var pageSize = Convert.ToInt32(_configuration.GetMoodleConfig("CoursePageSize"));
                    var (lastPageNumber, courses) = await _businessCommon.GetPageWiseRecord(moodleCourseDetails, pageNumber, pageSize);
                    courseMessage.Message = genericMessages[MessageName.COURSE_LIST_INFO].Message;
                    if (shouldAddWelComeMessage)
                    {
                        courseMessage.Message = $"{genericMessages[MessageName.USER_WELCOME].Message}{courseMessage.Message}";
                    }

                    var optionMapping = new Dictionary<int, CourseDetailDto>();
                    var counter = ((pageNumber - 1) * pageSize);
                    foreach (var course in courses)
                    {
                        counter++;
                        var courceProgress = course.CourseProgress ?? 0;
                        var courseStatusEmoji = GetStatusWiseEmojis(botEmojis, course.CourseStatusId);
                        var courseStatus = course.CourseStatus.IsNullOrEmpty() ? _configuration.GetMoodleConfig("UnknownStatusName") : course.CourseStatus;
                        courseMessage.Message += string.Format(
                            genericMessages[MessageName.COURSE_LIST].Message, counter,
                            Convert.ToString(course.ShortName),
                            $"{courseStatusEmoji} {courseStatus}",
                            Math.Round(courceProgress, 2),
                            GetPercentageGrade(botEmojis, course.PercentageGrade, course.CourseStatusId));
                        optionMapping.Add(counter, new CourseDetailDto
                        {
                            CourseId = course.CourseId,
                            CourseImage = course.CourseImage,
                            CourseName = course.ShortName
                        });
                    }

                    courseMessage.Message += await _businessCommon.GetPaginationMessage(pageNumber, lastPageNumber, languageCode);
                    courseMessage.Message = courseMessage.Message.Replace("$#COURSE_COUNT#$", $"{moodleCourseDetails.Count()}");
                    courseMessage.CourseIdOptionMapping = optionMapping;
                    courseMessage.IsCourseAvailable = true;
                    courseMessage.CurrentPage = pageNumber;
                    courseMessage.LastPage = lastPageNumber;
                    _logger.Info("GetCourseMessageAsync: Successfully got Course Detail", string.Empty, userId);
                }
                else
                {
                    courseMessage.Message = genericMessages[MessageName.COURSE_NOT_FOUND].Message;
                    _logger.Info("GetCourseMessageAsync: Course Detail is not found", string.Empty, userId);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetCourseMessageAsync: Got an error while getting course details", exception, string.Empty, userId);
            }

            return courseMessage;
        }

        public async Task<CourseSummaryMessage> GetCourseSummaryAsync(long userId, long courseId, string languageCode)
        {
            var courseSummaryMessage = new CourseSummaryMessage();
            try
            {
                var genericMessages = await _genericMessages.GetGenericMessage(languageCode);
                var courses = await _course.GetCourses(userId);
                var course = courses?.Where(x => x.CourseId == courseId).FirstOrDefault();
                
                if (course != default(MoodleCourseDetail))
                {
                    var timeCompleted = string.Empty;
                    if (course.CourseStatusId == Convert.ToInt16(CourseStatus.Completed) ||
                        course.CourseStatusId == Convert.ToInt16(CourseStatus.CompletedPass) ||
                        course.CourseStatusId == Convert.ToInt16(CourseStatus.CompletedFail)){
                            //because moodle has a lag sync the completion date we check if the date is a human date
                            if(course.TimeCompleted > new DateTime(2019, 1, 1)){
                                timeCompleted = course.TimeCompleted?.ToString("dd-MM-yyyy h:mm tt");
                            }
                    }

                    courseSummaryMessage.Message = string.Format(genericMessages[MessageName.COURSE_SUMMARY].Message, 
                        course.CourseStatus, 
                        Math.Round(course.CourseProgress.Value, 2), 
                        course.PercentageGrade + "%", 
                        course.TimeStarted?.ToString("dd-MM-yyyy h:mm tt"), 
                        timeCompleted);

                    var botEmojis = await _emojis.GetEmojis();
                    courseSummaryMessage.Message = $"{botEmojis[EmojiName.SUMMARY].Emojis} {courseSummaryMessage.Message}";

                    _logger.Info("GetCourseSummaryAsync: Successfully got Course summary Detail", string.Empty, userId);
                }
                else
                {
                    _logger.Info("GetCourseSummaryAsync: Course summary Detail is not found", string.Empty, userId);
                }

                courseSummaryMessage.GenericMessages = genericMessages;
                courseSummaryMessage.Message += string.Format(
                            genericMessages[MessageName.SUMMARY_ACTION].Message,
                            genericMessages[MessageName.COURSE_ACTION_CURRENT_COURSE].Message,
                            genericMessages[MessageName.COURSE_ACTION_SHOW_MENU].Message,
                            string.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error($"GetCourseSummaryAsync: Got an error while getting course summary details for course Id: {courseId}", exception, string.Empty, userId);
            }

            return courseSummaryMessage;
        }

        public async Task<List<MoodleCourseDetail>> GetCourse(long userId, long? courseId = null)
        {
            return await _course.GetCourses(userId, courseId);
        }
        #endregion

        #region Private Method
        private string GetStatusWiseEmojis(Dictionary<EmojiName, BotEmojis> emojis, int courseStatusId)
        {
            var emoji = emojis[EmojiName.STATUS_UNKNOWN].Emojis;

            var courseStatusEmoji = emojis.Values.Where(x => x.EmojisTypeId == EmojisType.CourseStaus);
            if (courseStatusEmoji.Any(x => x.StatusId == courseStatusId))
            {
                emoji = courseStatusEmoji.Where(x => x.StatusId == courseStatusId).FirstOrDefault().Emojis;
            }

            return emoji;
        }

        private string GetPercentageGrade(Dictionary<EmojiName, BotEmojis> emojis, decimal? percentageGrade, int courseStatusId)
        {
            var grade = " ";
            if (courseStatusId == Convert.ToInt16(CourseStatus.Completed) ||
                courseStatusId == Convert.ToInt16(CourseStatus.CompletedPass) ||
                courseStatusId == Convert.ToInt16(CourseStatus.CompletedFail))
            {
                if(courseStatusId == Convert.ToInt16(CourseStatus.CompletedPass)){
                    grade = emojis[EmojiName.GRADE_GREEN].Emojis;
                    grade = $"{grade} {percentageGrade}%";
                }
                else if(courseStatusId == Convert.ToInt16(CourseStatus.CompletedFail)){
                    grade = emojis[EmojiName.GRADE_RED].Emojis;
                    grade = $"{grade} {percentageGrade}%";
                }
                else{
                    grade = (percentageGrade > 0 ? $"{percentageGrade}%" : " ");
                }
            }

            return grade;
        }

        #endregion
    }
}
