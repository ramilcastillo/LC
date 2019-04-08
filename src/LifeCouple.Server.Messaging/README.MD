Created 25 July 2018

Initially targeted to support SMS messaging using Twilio, and since Sep 2018 also support Push Notifications using App Center 

This Project publish messages to the Azure Storage Queue. The LifeCouple.Server.Functions solution listens/subscribes to these storage queues.

Requires settings matching the LCMessagingSettingsModel.cs, in either secrets.json, appsettings.json or application variables

Links:
- https://www.twilio.com/docs/libraries/csharp/custom-http-clients-dot-net-core
- https://www.twilio.com/docs/sms/tutorials/how-to-send-sms-messages-csharp
