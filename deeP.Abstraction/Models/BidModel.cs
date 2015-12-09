using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class BidModel : DataModel
    {
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [Required]
        [Display(Name = "PropertyId")]
        public string PropertyId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "State")]
        public BidState State { get; set; }
    }
}
