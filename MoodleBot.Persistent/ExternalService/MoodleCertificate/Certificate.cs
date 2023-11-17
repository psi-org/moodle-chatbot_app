using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public class Certificate : ICertificate
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Consturctor
        public Certificate(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<List<MoodleCertificateDetail>> GetCourseCertificateDetail(long userId, long courseId)
        {
            List<MoodleCertificateDetail> moodleUserDetail = null;
            try
            {
                var request = new MoodleCertificateRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetCertificates"),
                    UserId = userId,
                    CourseId = courseId
                };
                var result = await APICall.RunAsync<List<MoodleCertificateDetail>, MoodleCertificateRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
                if (result.Success)
                {
                    moodleUserDetail = result.Data;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetCourseCertificateDetail: Got an error while get the course certificate data from Moodle API. CourseId: {courseId}", exception, null, userId);
            }
            return moodleUserDetail;
        }
        #endregion
    }
}
