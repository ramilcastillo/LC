using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCouple.Server.Messaging.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Rest.Lookups.V1;

namespace LifeCouple.Server.Messaging
{
    public class PhoneService
    {
        private readonly LCMessagingSettingsModel _settings;

        private static readonly object lockObject = new object();

        private static TwilioRestClient lookUpClient;

        public PhoneService(IOptions<LCMessagingSettingsModel> settings)
        {
            _settings = settings.Value;
            if (lookUpClient == null)
            {
                lock (lockObject)
                {
                    if (lookUpClient == null)
                    {
                        if (_settings == null || JsonConvert.SerializeObject(_settings).Contains("null"))
                        {
                            throw new ArgumentNullException($"Unable to read/find one or more values for properties in the '{nameof(LCMessagingSettingsModel)}' model that needs to be set in application settings in the '{LCMessagingSettingsModel.SettingsSection}' section which is required to initiate the application. If you are running it locally check secrets.json and if in a hosted environment check Application Settings variables.");
                        }

                        lookUpClient = new TwilioRestClient(_settings.TwilioLookupSid, _settings.TwilioLookupKey);

                        //to ensure it fails fast
                        var storageAccount = CloudStorageAccount.Parse(_settings.AzureQueueConnection);
                        // Create the queue client.
                        var queueClient = storageAccount.CreateCloudQueueClient();
                        // Retrieve a reference to a container.
                        var queue = queueClient.GetQueueReference(_settings.AzureSmsQueue);
                        // Create the queue if it doesn't already exist
                        var exists = queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                    }
                }
            }
        }


        public async Task<PhoneNrInfo> GetPhoneNrInfo(string phoneNr)
        {
            // https://www.twilio.com/docs/lookup/quickstart

            PhoneNumberResource phoneNumber = null;
            var isValid = false;
            try
            {
                phoneNumber = await PhoneNumberResource.FetchAsync(
                    client: lookUpClient,
                    pathPhoneNumber: new Twilio.Types.PhoneNumber(phoneNr),
                    type: new List<string> { "carrier" }
                    );
                isValid = true;
            }
            catch (ApiException ex)
            {
                isValid = false;
            }

            var r = new PhoneNrInfo
            {
                IsValid = isValid,
                CountryCode = phoneNumber?.CountryCode,
                NationalFormat = phoneNumber?.NationalFormat,
                PhoneNr = phoneNumber?.PhoneNumber.ToString(),
                TypeOfNumber = getTypeOfNr(phoneNumber)
            };


            PhoneNrInfo.TypeOfNumberEnum getTypeOfNr(PhoneNumberResource phNrResource)
            {
                if (phNrResource?.Carrier == null)
                {
                    return PhoneNrInfo.TypeOfNumberEnum.Unknown;
                }
                else if (phNrResource.Carrier.TryGetValue("type", out var type))
                {
                    if (type == "mobile")
                    {
                        return PhoneNrInfo.TypeOfNumberEnum.Mobile;
                    }
                    else
                    {
                        return PhoneNrInfo.TypeOfNumberEnum.Other;
                    }
                }
                else
                {
                    return PhoneNrInfo.TypeOfNumberEnum.Unknown;
                }
            };

            return r;
        }

        public async Task PublishSmsDtoAsync(string toMobileNr, string smsMessage)
        {
            //NOTE: None of the Azure Storage objects are thread safe = https://github.com/Azure/azure-storage-net/issues/270
            var storageAccount = CloudStorageAccount.Parse(_settings.AzureQueueConnection);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(_settings.AzureSmsQueue);

            var messageObject = new MsgSmsDto
            {
                FromPhoneNr = _settings.SmsFromNr,
                SmsMessageBody = smsMessage,
                ToMobileNr = toMobileNr,
            };

            var message = new CloudQueueMessage(JsonConvert.SerializeObject(messageObject));
            await queue.AddMessageAsync(message);
        }
    }
}
