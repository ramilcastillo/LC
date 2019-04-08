using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LifeCouple.DAL.Entities
{
    public abstract class CosmosDbEntity
    {
        /// <summary>
        /// Set by CosmosDb repository to identify type of Document
        /// </summary>
        [JsonProperty(PropertyName = "type", Order = -2)]
        [NotMapped]
        public string Type { get; set; }
    }
}
