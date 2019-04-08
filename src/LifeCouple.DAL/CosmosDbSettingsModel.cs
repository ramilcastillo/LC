using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.DAL
{
    /// <summary>
    /// Populated using appSettings.json or environment variables
    /// Add in Startup: services.Configure<CosmosDbSettingsModel>(_config.GetSection(COSMOSDBSECTION)); 
    /// and then injected into CosmosDb repo like 'IOptions<CosmosDbSettingsModel> settings'
    /// </summary>
    public class CosmosDbSettingsModel
    {
        /// <summary>
        /// When set in EnvironmentVariables 'CosmosDbSettings:AccessKey'
        /// </summary>
        public string AccessKey { get; set; }
        public string DatabaseId { get; set; }
        public string DefaultCollectionId { get; set; }
        /// <summary>
        /// e.g. https://test.documents.azure.com:443/
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Delete all contents in db at startup when value is "CleanTestAbc"
        /// </summary>
        public string StartupMode { get; set; }
    }
}
