using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoodleBot.Persistent.ExternalService
{
    public class User : IUser
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        #region Consturctor
        public User(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<MoodleUserDetail> GetUserDetail(string mobilePhone, long? userId = null)
        {
            MoodleUserDetail moodleUserDetail = null;
            try
            {
                var request = new MoodleUserRequest(_configuration)
                {
                    FunctionName = _configuration.GetMoodleAPIConfig("GetUser"),
                    UserId = userId,
                    MobilePhone = mobilePhone
                };
                var result = await APICall.RunAsync<MoodleUserDetail, MoodleUserRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, request);
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
                _logger.Error("GetUserDetail: Got an error while get the user data from Moodle API.", exception);
            }
            return moodleUserDetail;
        }

        public async Task<MoodleCreateUserDetail> CreateNewUserInMoodle(MoodleCreateUserRequest moodleCreateUserRequest)
        {
            MoodleCreateUserDetail moodleCreateUserDetail = null;
            try
            {
                moodleCreateUserRequest.FunctionName = _configuration.GetMoodleAPIConfig("CreateUser");
                var result = await APICall.RunAsync<MoodleCreateUserDetail, MoodleCreateUserRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, moodleCreateUserRequest);
                if (result.Success)
                {
                    moodleCreateUserDetail = result.Data;
                    _logger.Info($"CreateNewUserInMoodle: User is created in moodle and stauts is {moodleCreateUserDetail.Status}", moodleCreateUserRequest.MobilePhone, moodleCreateUserDetail.UserId);
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("CreateNewUserInMoodle: Got an error while create a user account in Moodle.", exception);
            }

            return moodleCreateUserDetail;
        }

        public async Task<MoodleUpdateUserDetail> UpdateUserInMoodle(MoodleUpdateUserRequest moodleUpdateUserRequest)
        {
            MoodleUpdateUserDetail moodleUpdateUserDetail = null;
            try
            {
                moodleUpdateUserRequest.FunctionName = _configuration.GetMoodleAPIConfig("UpdateUser");
                var result = await APICall.RunAsync<MoodleUpdateUserDetail, MoodleUpdateUserRequest>(_configuration.GetMoodleAPIConfig("BaseUrl"), HttpMethod.Post, moodleUpdateUserRequest);
                if (result.Success)
                {
                    moodleUpdateUserDetail = result.Data;
                    _logger.Info($"UpdateUserInMoodle: User is updated in moodle and stauts is {moodleUpdateUserDetail.Status}", "", moodleUpdateUserDetail.UserId);
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("UpdateUserInMoodle: Got an error while updating a user account in Moodle.", exception);
            }

            return moodleUpdateUserDetail;
        }
        #endregion
    }
}
