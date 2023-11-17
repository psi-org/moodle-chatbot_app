using Microsoft.Extensions.Configuration;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Lookups.V1;

namespace MoodleBot.Persistent.ExternalService
{
    public class TwilioPhoneNumberAPI : ITwilioPhoneNumberAPI
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        #endregion

        public TwilioPhoneNumberAPI(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        #region Public Method
        public async Task<WhatsAppNumberDetails> GetWhatsAppNumberDetails(string whatsAppNumber)
        {
            return await Task.Run(() =>
            {
                var phoneNumberDetail = new WhatsAppNumberDetails
                {
                    IsSuccess = false
                };

                try
                {
                    TwilioClient.Init(_configuration.GetTwilioConfig("TwilioAccountSid"), _configuration.GetTwilioConfig("TwilioAuthToken"));

                    var phoneNumber = PhoneNumberResource.Fetch(
                        type: new List<string> { "carrier", "caller-name" },
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(whatsAppNumber)
                    );

                    if (phoneNumber != null)
                    {
                        var json = JsonConvert.SerializeObject(phoneNumber);
                        phoneNumberDetail = JsonConvert.DeserializeObject<WhatsAppNumberDetails>(json);
                        phoneNumberDetail.IsSuccess = phoneNumberDetail.Carrier.ErrorCode == null;

                        var phoneUtil = PhoneNumberUtil.GetInstance();
                        var libPhoneNumber = phoneUtil.Parse(phoneNumberDetail.PhoneNumber, phoneNumberDetail.CountryRegionCode);

                        phoneNumberDetail.CountryCode = libPhoneNumber.CountryCode.ToString();
                        phoneNumberDetail.PhoneNumber = libPhoneNumber.NationalNumber.ToString();
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error($"GetWhatsAppNumberDetails: Got an error while getting WhatsApp phone number details from Twilio for phone number: {whatsAppNumber}", exception);
                }

                return phoneNumberDetail;
            });
        }
        #endregion
    }
}
