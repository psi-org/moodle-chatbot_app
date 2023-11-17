using Microsoft.Extensions.Configuration;
using MoodleBot.Business.Database;
using MoodleBot.Business.Entity;
using MoodleBot.Common;
using MoodleBot.Common.Enums;
using MoodleBot.Models;
using MoodleBot.Persistent.DBStore;
using MoodleBot.Persistent.Entity;
using MoodleBot.Persistent.ExternalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoodleBot.Business
{
    public class MoodleUser : IMoodleUser
    {
        #region Properties

        private readonly ILogger _logger;
        private readonly IUser _user;
        private readonly ITwilioPhoneNumberAPI _twilioPhoneNumberAPI;
        private readonly IGenericMessages _genericMessages;
        private readonly IUserCreationQuestionStore _userCreationQuestionStore;
        private readonly ICountryWiseLanguage _countryWiseLanguage;
        private readonly IConfiguration _configuration;
        private readonly IEmojis _emojis;

        #endregion

        #region Constructor

        public MoodleUser(IUser user, ILogger logger, ITwilioPhoneNumberAPI twilioPhoneNumberAPI,
            IGenericMessages genericMessages, IUserCreationQuestionStore userCreationQuestionStore,
            ICountryWiseLanguage countryWiseLanguage, IConfiguration configuration, IEmojis emojis)
        {
            _logger = logger;
            _user = user;
            _twilioPhoneNumberAPI = twilioPhoneNumberAPI;
            _genericMessages = genericMessages;
            _userCreationQuestionStore = userCreationQuestionStore;
            _countryWiseLanguage = countryWiseLanguage;
            _configuration = configuration;
            _emojis = emojis;
        }

        #endregion

        #region Public Method

        public async Task<MoodleUserDetail> GetUserDetail(string whatsAppId, long? userId = null)
        {
            MoodleUserDetail userDetail = null;

            try
            {
                if (whatsAppId.IsNotNullOrEmpty())
                {
                    var whatsAppNumber = whatsAppId.GetWhatsNumber();
                    userDetail = await _user.GetUserDetail(whatsAppNumber, userId);
                    if (userDetail != default(MoodleUserDetail))
                    {
                        _logger.Info("GetUserDetail: Successfully get the user detail", userDetail.Email,
                            userDetail.Userid);
                    }
                }
                else
                {
                    _logger.Info("GetUserDetail: WhatsAppId is not found");
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetUserDetail: Got an error while get the user data.", exception);
            }

            return userDetail;
        }

        public async Task<WhatsAppNumberDetailMessage> GetWhatsAppNumberDetailsAsync(string whatsAppId,
            string languageCode)
        {
            var whatsAppNumberDetailMessage = new WhatsAppNumberDetailMessage
            {
                IsNumberDetailFound = false
            };

            try
            {
                var whatsAppNumber = whatsAppId.GetWhatsNumber();
                if (whatsAppNumber.IsNotNullOrEmpty())
                {
                    var whatsAppNumberDetail = await _twilioPhoneNumberAPI.GetWhatsAppNumberDetails(whatsAppNumber);
                    if (whatsAppNumberDetail.IsSuccess)
                    {
                        var languageDetail =
                            await _countryWiseLanguage.GetCountryLanguageDetail(whatsAppNumberDetail.CountryRegionCode,
                                whatsAppNumberDetail.CountryCode);
                        var IsLanguageDetailFound = false;
                        var selectedLanguageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
                        var selectedLanguage = _configuration.GetMoodleConfig("DefaultLanguage");
                        var userAccountQuestion = await GetQuestionDetails(languageCode);
                        if (languageDetail?.Count > 0)
                        {
                            var language = languageDetail.FirstOrDefault();
                            selectedLanguage = language.LanguageName;
                            selectedLanguageCode = language.LanguageISO;
                            IsLanguageDetailFound = true;
                        }

                        whatsAppNumberDetailMessage = await MapWhatsAppNumberDetailMessage(whatsAppNumberDetail,
                            _configuration.GetMoodleConfig("DefaultLanguageCode"));
                        whatsAppNumberDetailMessage.Message = string.Format(
                            userAccountQuestion[CreateUserMessageName.COUNTRY_CODE_CONFIRMATION].Question,
                            whatsAppNumberDetail.CountryCode, whatsAppNumberDetail.CountryRegionCode, selectedLanguage);
                        whatsAppNumberDetailMessage.Message += string.Format(
                            userAccountQuestion[CreateUserMessageName.SELECT_PREFER_LANGUAGE].Question,
                            selectedLanguage);
                        whatsAppNumberDetailMessage.Message +=
                            userAccountQuestion[CreateUserMessageName.CHANGE_MOBILE_CODE].Question;

                        whatsAppNumberDetailMessage.ActionIdOptionMapping = new List<int> { 1, 2 };
                        whatsAppNumberDetailMessage.SelectedLanguageCode = selectedLanguageCode;
                        whatsAppNumberDetailMessage.IsNumberDetailFound = true;
                        if (IsLanguageDetailFound)
                        {
                            whatsAppNumberDetailMessage.Message +=
                                userAccountQuestion[CreateUserMessageName.SELECT_DEFAULT_LANGUAGE].Question;
                            whatsAppNumberDetailMessage.ActionIdOptionMapping.Add(3);
                        }

                        _logger.Info(
                            $"GetWhatsAppNumberDetailsAsync: Successfully get the whats app number detail for mobile number {whatsAppNumberDetail.PhoneNumber} and Country code {whatsAppNumberDetail.CountryRegionCode}");
                    }
                }
                else
                {
                    _logger.Info("GetUserDetail: WhatsAppId is not found");
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetUserDetail: Got an error while get the user data.", exception);
            }

            return whatsAppNumberDetailMessage;
        }

        public async Task<WhatsAppNumberDetailMessage> GetLanguageSelectionMessage(string callingCode,
            string languageCode)
        {
            var whatsAppNumberDetailMessage = new WhatsAppNumberDetailMessage
            {
                IsNumberDetailFound = false
            };

            try
            {
                var languageDetail = await _countryWiseLanguage.GetCountryLanguageDetail(callingCode);
                var IsLanguageDetailFound = false;
                var selectedLanguageCode = _configuration.GetMoodleConfig("DefaultLanguageCode");
                var selectedLanguage = _configuration.GetMoodleConfig("DefaultLanguage");
                if (languageDetail?.Count > 0)
                {
                    var language = languageDetail.FirstOrDefault();
                    selectedLanguage = language.LanguageName;
                    selectedLanguageCode = language.LanguageISO;
                    IsLanguageDetailFound = true;
                }

                var userAccountQuestion = await GetQuestionDetails(languageCode);
                whatsAppNumberDetailMessage.Message = string.Format(
                    userAccountQuestion[CreateUserMessageName.SELECT_LANGUAGE_USING_CODE].Question, selectedLanguage,
                    callingCode);
                whatsAppNumberDetailMessage.Message += string.Format(
                    userAccountQuestion[CreateUserMessageName.SELECT_PREFER_LANGUAGE].Question, selectedLanguage);
                whatsAppNumberDetailMessage.Message +=
                    userAccountQuestion[CreateUserMessageName.CHANGE_MOBILE_CODE].Question;

                whatsAppNumberDetailMessage.ActionIdOptionMapping = new List<int> { 1, 2 };
                whatsAppNumberDetailMessage.SelectedLanguageCode = selectedLanguageCode;
                whatsAppNumberDetailMessage.GenericMessage =
                    await _genericMessages.GetGenericMessage(_configuration.GetMoodleConfig("DefaultLanguageCode"));
                whatsAppNumberDetailMessage.IsNumberDetailFound = true;

                if (IsLanguageDetailFound)
                {
                    whatsAppNumberDetailMessage.Message +=
                        userAccountQuestion[CreateUserMessageName.SELECT_DEFAULT_LANGUAGE].Question;
                    whatsAppNumberDetailMessage.ActionIdOptionMapping.Add(3);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(
                    "GetLanguageSelectionMessage: Got an error while getting language details based on mobile country code",
                    exception);
            }

            return whatsAppNumberDetailMessage;
        }

        public async Task<CreateUserQuestionMessage> GetNextQuestion(int currentQuestionPosition,
            Dictionary<string, string> userDetails, string languageCode)
        {
            var questionNumber = currentQuestionPosition - 1;
            var useTermsConditions = bool.Parse(_configuration.GetRootConfig("useTermsConditions"));
            if (!useTermsConditions && currentQuestionPosition == 1)
            {
                currentQuestionPosition += 1;
            }

            if (questionNumber == 0)
            {
                questionNumber = 1;
            }

            var createUserQuestionMessage = new CreateUserQuestionMessage
            {
                IsQuestionFound = false
            };

            try
            {
                var questionList = await GetQuestionDetails(languageCode);
                var totalQuestionCount =
                    questionList.Count(x => x.Value.MessageType == CreateUserMessageType.UserDetailQuestion);
                var userDetailQuestion =
                    questionList.Where(x => x.Value.MessageType != CreateUserMessageType.NextPreviousQuestion);
                var nextQuestion = userDetailQuestion.Where(x => x.Value.MessagePosition == currentQuestionPosition)
                    .FirstOrDefault();
                if (nextQuestion.Value != null)
                {
                    var genericMessage = await _genericMessages.GetGenericMessage(languageCode);
                    var message = string.Empty;
                    var question = nextQuestion.Value;
                    var nextActionMessage = string.Empty;
                    var validationMessage = genericMessage[MessageName.INVALID_INPUT].Message;
                    if (question.MessageType == CreateUserMessageType.UserDetailQuestion)
                    {
                        var nextPreviousEmojis = await _emojis.GetEmoji(EmojisType.Pagination);
                        var genericEmojis = await _emojis.GetEmoji(EmojisType.Generic);
                        if (question.MessageName != CreateUserMessageName.FIRST_NAME.ToString())
                        {
                            nextActionMessage +=
                                $"\r\n{string.Format(questionList[CreateUserMessageName.PREVIOUS_QUESTION].Question, nextPreviousEmojis[EmojiName.PAGE_PREVIOUS].Emojis)}";
                        }

                        var questionTitle = (question.IsRequired
                            ? $"{genericEmojis[EmojiName.STATUS_ALERT].Emojis} {questionList[CreateUserMessageName.QUESTION_TITLE].Question}"
                            : questionList[CreateUserMessageName.QUESTION_TITLE].Question);
                        if (!question.IsRequired)
                        {
                            questionTitle = questionTitle.Replace("\r\n\r\n",
                                $" ({genericMessage[MessageName.OPTIONAL].Message})\r\n\r\n");
                            nextActionMessage +=
                                $"\r\n{string.Format(questionList[CreateUserMessageName.NEXT_QUESTION].Question, nextPreviousEmojis[EmojiName.PAGE_NEXT].Emojis)}";
                        }

                        message = string.Format(questionTitle, questionNumber, totalQuestionCount);
                        message += question.Question;
                    }
                    else if (question.MessageName == CreateUserMessageName.USER_DETAIL_SUMMARY.ToString())
                    {
                        var userAccountQuestion = await GetQuestionDetails(languageCode);
                        message = await GetRegistrationSummary(question.Question, userDetails);
                        nextActionMessage = userAccountQuestion[CreateUserMessageName.VALIDATE_SUMMARY].Question;
                        nextActionMessage += userAccountQuestion[CreateUserMessageName.UPDATE_SUMMARY_DETAILS].Question;
                    }
                    else if (question.MessageName == CreateUserMessageName.TERMS_CONDITIONS.ToString())
                    {
                        message = question.Question;
                        nextActionMessage = userDetailQuestion
                            .Where(x => x.Key == CreateUserMessageName.TERMS_CONDITIONS_CONFIRMATION)
                            .Select(x => x.Value.Question).FirstOrDefault();
                    }

                    if (question.ValidationMessage.IsNotNullOrEmpty())
                    {
                        validationMessage = question.ValidationMessage;
                    }

                    createUserQuestionMessage.Message = message;
                    createUserQuestionMessage.NextActionMessage = nextActionMessage;
                    createUserQuestionMessage.CurrentQuestionNumber = currentQuestionPosition;
                    createUserQuestionMessage.QuestionValidationName = question.ValidationName;
                    createUserQuestionMessage.QuestionValidationMessage = validationMessage;
                    createUserQuestionMessage.CurrentQuestionName = question.MessageName;
                    createUserQuestionMessage.PropmtValidationMessage = validationMessage;
                    createUserQuestionMessage.IsAnswerRequired = question.IsRequired;
                    createUserQuestionMessage.IsLastQuestion =
                        nextQuestion.Key == CreateUserMessageName.USER_DETAIL_SUMMARY;
                    createUserQuestionMessage.IsQuestionFound = true;
                }
            }
            catch (Exception exception)
            {
                _logger.Error("GetUserCreationQuestion: Error while prepared User account creataion question",
                    exception);
            }

            return createUserQuestionMessage;
        }

        public async Task<Dictionary<CreateUserMessageName, UserCreationQuestion>> GetQuestionDetails(
            string languageCode)
        {
            var question = new Dictionary<CreateUserMessageName, UserCreationQuestion>();
            try
            {
                var questionDetail = await _userCreationQuestionStore.GetUserCreationQuestion(languageCode);
                question = questionDetail.ToDictionary(
                    x => (CreateUserMessageName)Enum.Parse(typeof(CreateUserMessageName), x.MessageName), x => x);
            }
            catch (Exception exception)
            {
                _logger.Error("GetQuestionDetails: Got an error while get user account Question Details", exception);
            }

            return question;
        }

        public async Task<bool> CreateUserInMoodle(CreateUserAccountRequest userAccountRequest)
        {
            var result = false;
            if (userAccountRequest?.UserDetails?.Count > 0)
            {
                try
                {
                    var userDetails = userAccountRequest.UserDetails;
                    //var genderTypeName = userDetails[CreateUserMessageName.GENDER.ToString()];
                    //var healthWorkerType = userDetails[CreateUserMessageName.HEALTH_WORKER_TYPE.ToString()];
                    var createUserRequest = new MoodleCreateUserRequest(_configuration)
                    {
                        //WhatsAppId = Regex.Replace(userAccountRequest.WhatsAppId, @"[^0-9a-zA-Z]+", ""),
                        MobileCountryCode = userAccountRequest.MobileCountryCode,
                        MobilePhone = userAccountRequest.MobilePhoneNumber.Replace(" ", ""),
                        Country = userAccountRequest.CountryCode,
                        Language = userAccountRequest.LanguageCode,

                        FirstName = userDetails[CreateUserMessageName.FIRST_NAME.ToString()],
                        LastName = userDetails[CreateUserMessageName.LAST_NAME.ToString()],
                        Password = userDetails[CreateUserMessageName.PASSWORD.ToString()],
                        //Gender = genderTypeName.IsEnumContainsValue<Gender>() ? Convert.ToInt32(genderTypeName.GetEnumFromValue<Gender>()) : 0,
                        //DateOfBirth = userDetails[CreateUserMessageName.DATE_OF_BIRTH.ToString()],
                        Email = userDetails[CreateUserMessageName.EMAIL.ToString()],
                        //HealthWorkerNumber = userDetails[CreateUserMessageName.HEALTH_WORKER_NUMBER.ToString()],
                        //HealthWorkerType = healthWorkerType.IsEnumContainsValue<HealthWorkerType>() ? Convert.ToInt32(healthWorkerType.GetEnumFromValue<HealthWorkerType>()) : 0
                    };

                    var userResponse = await _user.CreateNewUserInMoodle(createUserRequest);

                    result = (userResponse != null && userResponse.UserId > 0);
                }
                catch (Exception exception)
                {
                    _logger.Error("CreateUserInMoodle: Got an error while creating user in Moodle.", exception);
                }
            }

            return result;
        }

        public async Task<bool> UpdateUserInMoodle(CreateUserAccountRequest userAccountRequest)
        {
            var result = false;
            if (userAccountRequest?.UserDetails?.Count > 0)
            {
                try
                {
                    var userDetails = userAccountRequest.UserDetails;
                    //var genderTypeName = userDetails[CreateUserMessageName.GENDER.ToString()];
                    //var healthWorkerType = userDetails[CreateUserMessageName.HEALTH_WORKER_TYPE.ToString()];
                    var updateUserRequest = new MoodleUpdateUserRequest(_configuration)
                    {
                        Userid = Int32.Parse(userDetails[CreateUserMessageName.USER_ID.ToString()]),
                        FirstName = userDetails[CreateUserMessageName.FIRST_NAME.ToString()],
                        LastName = userDetails[CreateUserMessageName.LAST_NAME.ToString()],
                        Password = userDetails[CreateUserMessageName.PASSWORD.ToString()],
                        //Gender = genderTypeName.IsEnumContainsValue<Gender>() ? Convert.ToInt32(genderTypeName.GetEnumFromValue<Gender>()) : 0,
                        //DateOfBirth = userDetails[CreateUserMessageName.DATE_OF_BIRTH.ToString()],
                        Email = userDetails[CreateUserMessageName.EMAIL.ToString()],
                        //HealthWorkerNumber = userDetails[CreateUserMessageName.HEALTH_WORKER_NUMBER.ToString()],
                        //HealthWorkerType = healthWorkerType.IsEnumContainsValue<HealthWorkerType>() ? Convert.ToInt32(healthWorkerType.GetEnumFromValue<HealthWorkerType>()) : 0,
                        Language = userAccountRequest.LanguageCode
                    };

                    var userResponse = await _user.UpdateUserInMoodle(updateUserRequest);

                    result = (userResponse != null && userResponse.UserId > 0);
                }
                catch (Exception exception)
                {
                    _logger.Error("UpdateUserInMoodle: Got an error while updating user in Moodle.", exception);
                }
            }

            return result;
        }

        #endregion

        #region Private Method

        private async Task<WhatsAppNumberDetailMessage> MapWhatsAppNumberDetailMessage(
            WhatsAppNumberDetails whatsAppNumberDetails, string languageCode)
        {
            var genericMessage = await _genericMessages.GetGenericMessage(languageCode);
            return new WhatsAppNumberDetailMessage
            {
                IsNumberDetailFound = true,
                PhoneNumber = whatsAppNumberDetails.PhoneNumber,
                NationalFormat = whatsAppNumberDetails.NationalFormat,
                CountryCode = whatsAppNumberDetails.CountryRegionCode,
                GenericMessage = genericMessage,
                MobileCountryCode = whatsAppNumberDetails.CountryCode,
                Carrier = new Entity.Carrier
                {
                    MobileCountryCode = whatsAppNumberDetails.Carrier.MobileCountryCode,
                    MobileNetworkCode = whatsAppNumberDetails.Carrier.MobileNetworkCode,
                    CompanyName = whatsAppNumberDetails.Carrier.CompanyName,
                    Type = whatsAppNumberDetails.Carrier.Type,
                    ErrorCode = whatsAppNumberDetails.Carrier.ErrorCode
                }
            };
        }

        private async Task<string> GetRegistrationSummary(string summaryFormat, Dictionary<string, string> userDetails)
        {
            return await Task.Run(() =>
            {
                var message = summaryFormat;
                foreach (var keyValue in userDetails)
                {
                    message = message.Replace($"$#{keyValue.Key}#$", keyValue.Value);
                }

                return (message);
            });
        }

        #endregion
    }
}