using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace LifeCouple.DAL
{
    public static class CosmosDbHelper
    {
        /// <summary>
        /// Extension method used to load existing documents and ensure datetime is kept in UTC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="existingEntity"></param>
        public static void LoadFromExisingEntity<T>(this Document document, T existingEntity)
        {
            using (var sr = new StringReader(Newtonsoft.Json.JsonConvert.SerializeObject(existingEntity, RepoHelpers.jsonSerializerSettings)))
            {
                document.LoadFrom(new Newtonsoft.Json.JsonTextReader(sr)
                {
                    DateParseHandling = DateParseHandling.DateTimeOffset,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
            }
        }
    }
}
