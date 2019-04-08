LifeCouple.Server tests

For these tests to succeed on your local dev workstation the secrets.json that is used for LifeCouple.WebApi needs to include a section like below (with actual values instead of 'replace_me'):
  "LCMessaging": {
    "SmsFromNr": "replace_me"_
    "TwilioLookupSid": "replace_me",
    "TwilioLookupKey": "replace_me",
    "AzureQueueConnection": "UseDevelopmentStorage=true",
    "AzureSmsQueue": "partner-invitation-sms"
  }
