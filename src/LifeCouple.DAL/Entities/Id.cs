using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LifeCouple.DAL.Entities
{
    [Table("Id")]
    public class Id
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(30)]
        public string EntityType { get; set; }
        [Required]
        public long LastId { get; set; }
    }
}
