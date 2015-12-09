using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class PropertyFilter : PageFilter
    {
        [Display(Name = "PropertyId")]
        public string PropertyId { get; set; }

        [Display(Name = "SellerName")]
        public string SellerName { get; set; }

        [Display(Name = "Type")]
        public PropertyType? Type { get; set; }

        [Display(Name = "MinBedrooms")]
        public byte MinBedrooms { get; set; }

        [Display(Name = "MinPrice")]
        public double? MinPrice { get; set; }

        [Display(Name = "MaxPrice")]
        public double? MaxPrice { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "IncludeTaken")]
        public bool IncludeTaken { get; set; }
    }
}
