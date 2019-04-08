using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCouple.DAL.Entities
{
    public class Relationship : CosmosDbEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "relationshipStatus")]
        public RelationshipStatusEnum? RelationshipStatus { get; set; }

        [Required]
        [JsonProperty(PropertyName = "marriageStatus")]
        public MarriageStatusEnum? MarriageStatus { get; set; }

        [JsonProperty(PropertyName = "currentWeddingDate")]
        public DateTime? CurrentWeddingDate { get; set; }

        [JsonProperty(PropertyName = "nrOfChildren")]
        public int? NrOfChildren { get; set; }

        [JsonProperty(PropertyName = "nrOfStepChildren")]
        public int? NrOfStepChildren { get; set; }

        [Required]
        [JsonProperty(PropertyName = "beenToCounseler")]
        public CounselerStatusEnum? BeenToCounseler { get; set; }

        [Required]
        [JsonProperty(PropertyName = "registeredPartner_Id")]
        public string RegisteredPartner_Id { get; set; }
    }


    public enum RelationshipStatusEnum
    {
        Unknown = 0,
        Married = 1,
        NotMarried = 2,
    }

    public enum MarriageStatusEnum
    {
        Unknown = 0,
        FirstMarriage = 1,
        MoreThanOneMarriage = 99,
    }

    public enum CounselerStatusEnum
    {
        Unknown = 0,
        NoNever = 1,
        YesInThePast = 2,
        YesCurrently = 3,
    }

}
