# Markdown File

This solution was created Feb 22, 2018 using Visual Studio 2017 Community edition 15.5.7

Projects:
- LifeCouple.WebApi - Startup project that is being deployed to Azure. The Empty asp.net core project template was used 
- LifeCouple.DAL - .net core library for Data Access

Folders:
- webApiSampleRequests - see README_webApiSampleRequests.MD in that folder

Dependencies
- LifeCouple.DTO - DTOs that is part of the LifeCouple.DTOs solution which in its on git repo (https://lifecouple.visualstudio.com/LC90/_git/LifeCouple.DTOs) and published to https://lifecouple.visualstudio.com/LC90/_Packaging?feed=Mobile&package=LifeCouple.DTO&version=1.0.0.14-ci&protocolType=NuGet&_a=package
  - see nuget feed "https://lifecouple.pkgs.visualstudio.com/_packaging/Mobile/nuget/v3/index.json"
- LifeCouple.Server.Instrumentation - instrumentation for the web api that is in its own solution and repo, built using VSTS and published to our private feed

Azure Deployment:
- lcapicitest = used for CI build deployments, but Test are not execute there for some reason

API Versioning
		http://localhost:8888/api/things #latest version, in progress
		http://localhost:8888/api/v1/things #version 1 released (currently only sample, but to illustrate API 

Development:
- Visual Studio Community 2017 version 15.8.5 (or higher is required)
- dotnet --version => 2.1.402 #https://aka.ms/dotnet-download / https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.302-windows-x64-installer
- Azure Cosmos Db Emulator 2.0.0 and Azure Storage Emulator 5.6.0.0 and Azure Storage Explorer 1.4.1 is required for Unit Test and Development

Use/Add 'secrets.json' and add the following (replace values with actual valid ones):
    "CosmosDbSettings": {
      "AccessKey": "C2y6y Djf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", //Cosmos db Emulator key
      "DatabaseId": "LC_Local_Emulator",
      "DefaultCollectionId": "LCDefCol-secretsJson",
      "Endpoint": "https://localhost:8081",
      "StartupMode": "CleanTestAbc"
    },
"LCMessaging": { //Used for LifeCouple.Server.Messaging....
    "SmsFromNr": "REPLACE_ME",
    "TwilioLookupSid": "REPLACE_ME",
    "TwilioLookupKey": "REPLACE_ME", 
    "AzureQueueConnection": "UseDevelopmentStorage=true",
    "AzureSmsQueue": "partner-invitation-sms"
  }

Testing
- to test using ADB2C 
-- add the following to secrets.json (replace values with actual valid ones):
  "AzureAdB2C": {
    "Tenant": "REPLACE_ME",
    "ClientId": "REPLACE_ME",
    "Policy": "REPLACE_ME"
  },
-- Obtain and ADB2C token by going to https://webapilcprod.azurewebsites.net/index
-- Use that token and call some web apis using the Rest Client in VS CODE

TODO / TO CONSIDER:
- Asp.Net Core 2.1:
    - Change to derive from ControllerBase instead of Controller
    - Change to use ActionResult<T> instead of IActionResult
- To SystemStatusController add Environment.GetVersion and System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(x => x.Name.StartsWith("Microsoft.AspNetCore.Mvc.Core").First().Version.ToString()
- Fix controller when validation fails, like email address... to return proper error
- Need ot validate email, gender and notificationOption when PUTs are invoked
- port LookupsController from BeverageMgrServer... including state, zip codes, city lookup - see also Processing Large Datasets Using C# and SQL Server Table Data Types (Zip Code Loader Problem) - http://www.codemag.com/article/1701101/Processing-Large-Datasets-Using-C - http://simplemaps.com/resources/us-cities-data
- figure out how both internal JWT and ADB2C token can work side by side
- Application Insights - see [Setting up Application Insights took 10 minutes. It created two days of work for me. - Scott Hanselman](https://www.hanselman.com/blog/SettingUpApplicationInsightsTook10MinutesItCreatedTwoDaysOfWorkForMe.aspx)
- Determine where and how we should invoke validators like [Required], for sure in Context, but not sure we need it in the Api DTO's but proably 'only' in BusinessLogic DTO's
- review encryption options since some data needs to be encrypted:
-- https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction


DPF 
-             //TODO: COuld not get this to work so the keys for CSRF ends up in C:\Users\per\AppData\Local\ASP.NET\DataProtection-Keys which is not ook when hosted on Azure..
            ////https://codeopinion.com/asp-net-core-data-protection/
            //var creds = new StorageCredentials("cxYWJDNWhPx b3pbwg3RCtZE1+AjBVWUv+wrnTrvEP1NBEbP35nivomqqLKb8NgMNXS0utYdZY99UdgO6KW8PLw==");
            //var uri = new Uri("https://lcdevteststorage.blob.core.windows.net/test-dpf-persistkeys-quickstartcore180408/dpfkeys/keys.xml");
            //var cloudstorage = new CloudBlockBlob(uri, creds);
            //services.AddDataProtection().PersistKeysToAzureBlobStorage(cloudstorage);
