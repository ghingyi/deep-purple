using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL.Context
{
    public class Bid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(36)]
        public string Id { get; set; }
        
        [Index("IX_Owner", 1)]
        [Required]
        [MaxLength(256)]
        public string Owner { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public BidState State { get; set; }

        [Required]
        [ForeignKey("Property")]
        public string PropertyId { get; set; }

        [InverseProperty("Bids")]
        public virtual Property Property { get; set; }
    }
}
