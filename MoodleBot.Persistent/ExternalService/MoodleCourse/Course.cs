using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public class Course : ICourse
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Consturctor
        public Course(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<List<MoodleCourseDetail>> GetCourses(long userId, long? courseId = null)
        {
            List<MoodleCourseDetail> moodleCourseDetail = null;
            try
            {
                var request = new MoodleCourseRequest(_configuration){
                    FunctionName = _configuration.GetMoodleAPIConfig("GetCourse"),
                    UserId = userId,
                    CourseId = courseId
                };


                var result = await APICall.RunAsync<List<MoodleCourseDetail>, MoodleCourseRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleCourseDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetCourses: Got an error while getting course details from Moodle API.", exception, string.Empty, userId);
            }
            return moodleCourseDetail;
        }
        #endregion
    }
}
