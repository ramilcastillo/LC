using System;
using System.Linq;
using System.Text;
using LifeCouple.DAL;
using LifeCouple.Server.Instrumentation;
using LifeCouple.Server.Messaging;
using LifeCouple.WebApi.Common;
using LifeCouple.WebApi.DomainLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace LifeCouple.Server
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public const string aDB2CCONFIGSECTION = "AzureAdB2C";
        private const string jWTCONFIGSECTION = "JWT";
        private const string sYSTEMSTATUSACCESSKEYSSECTION = "SystemStatusAccessKeys";
        public const string cOSMOSDBSECTION = "CosmosDbSettings";
        private const string lc_instrumentation = "LC_Instrumentation";

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
            AppInfoModel.Init(config, _config.GetSection(aDB2CCONFIGSECTION)?.GetChildren()?.Count() > 0, _config.GetSection(cOSMOSDBSECTION)?.GetChildren()?.Count() > 0);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SystemStatusSettingsModel>(_config.GetSection(sYSTEMSTATUSACCESSKEYSSECTION)); //injected into Controllers like 'IOptions<SystemStatusSettingsModel> systemStatusKeysSettings'
            services.Configure<CosmosDbSettingsModel>(_config.GetSection(cOSMOSDBSECTION)); //injected into CosmosDb repo like 'IOptions<CosmosDbSettingsModel> settings'
            services.Configure<InstrumentationSettingsModel>(_config.GetSection(lc_instrumentation)); //injected into LC Instrumentation middleware like 'IOptions<InstrumentationSettingsModel> settings'
            services.Configure<LCMessagingSettingsModel>(_config.GetSection(LCMessagingSettingsModel.SettingsSection));

            services.AddMemoryCache();

            services.AddScoped<BusinessLogicDataSeeder>(); //services.AddTransient<BusinessLogicDataSeeder>();

            if (AppInfoModel.Current.IsCosmosDbUsed)
            {
                services.AddSingleton<IRepository, CosmosDBRepo>();
            }
            else
            {
                throw new NotSupportedException("Only CosmosDB is supported...");
            }

            services.AddScoped<BusinessLogic>();
            services.AddScoped<BusinessLogicEba>();
            services.AddScoped<BusinessLogicAppCenter>();
            services.AddScoped<PhoneService>();
            services.AddScoped<AppCenterService>();
            services.AddScoped<BusinessLogicActivity>();


            //Using JWT issued by AD B2C 
            if (AppInfoModel.Current.IsAuthADB2C_elseDev_TestJWT)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Events = new JWTEvents();

                    ///sample from link 'An ASP.NET Core 2.0 web API with Azure AD B2C' in step 2  on https://grimskog.wordpress.com/2018/02/17/token-based-authentication-jwt-azure-active-directory-b2c/
                    ///appsettings.json extract:
                    ///  "AzureAdB2C": {
                    ///  "Tenant": "fabrikamb2c.onmicrosoft.com",
                    ///  "ClientId": "25eef6e4-c905-4a07-8eb4-0d08d5df8b3f",
                    ///  "Policy": "b2c_1_susi",
                    ///  
                    var auth = $"https://login.microsoftonline.com/tfp/{_config[$"{aDB2CCONFIGSECTION}:Tenant"]}/{_config[$"{aDB2CCONFIGSECTION}:Policy"]}/v2.0/";
                    jwtOptions.Authority = auth;
                    jwtOptions.Audience = _config[$"{aDB2CCONFIGSECTION}:ClientId"];
                });
            }
            //Using JWT issued by this API, for dev test only
            else
            {
                services.Configure<JWTSettingsModel>(_config.GetSection(jWTCONFIGSECTION));
                services.AddSingleton<IJWTGenerator, JWTHandler>();
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Events = new JWTEvents();
                    //TODO: change to use parts from JWTHandler
                    //app.UseJwtBearerAuthentication(new JwtBearerOptions
                    //{
                    //    AutomaticAuthenticate = true,
                    //    TokenValidationParameters = jwtHandler.Parameters
                    //});


                    //When validating token issued by this app (for development only)
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _config[$"{jWTCONFIGSECTION}:Issuer"],
                        ValidAudience = _config[$"{jWTCONFIGSECTION}:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config[$"{jWTCONFIGSECTION}:HmacSecretKey"]))
                    };
                });
            }


            services.AddMvc(opt =>
            {
                //Assumes all Web Api requires Auth. Use '[AllowAnonymous]' for the few api's that don't
                opt.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                         .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                         .RequireAuthenticatedUser()
                         .Build()));

                
                if (AppInfoModel.Current.DisableSSL == false)
                {
                    opt.Filters.Add(new RequireHttpsAttribute());
                }
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt =>
            {
                //Note this only applies to the web api, see the DocumentClient for handling DocumentDB Json Serializing
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; // supress json properties with null values - see https://stackoverflow.com/questions/14486667/suppress-properties-with-null-value-on-asp-net-web-api
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //from https://stackify.com/net-core-loggerfactory-use-correctly/
            //call ConfigureLogger in a centralized place in the code
            LoggingHelper.ConfigureLogger(loggerFactory);
            //set it as the primary LoggerFactory to use everywhere
            LoggingHelper.LoggerFactory = loggerFactory;

            app.UseInstrumentationMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var businessLogicSeeder = scope.ServiceProvider.GetService<BusinessLogicDataSeeder>();
                try
                {
                    businessLogicSeeder.Seed();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable execute BusinessLogicSeeder.Seed()", ex);
                }
            }

            if (AppInfoModel.Current.DisableSSL == false)
            {
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps(400, 666));
            }

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Index}/{action=Index}/{id?}");
            });

            AppInfoModel.Current.SetAppInitiCompleteDT(); 
        }
    }
}
