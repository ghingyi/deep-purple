using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class BidFilter : PageFilter
    {
        [Display(Name = "PropertyId")]
        public string PropertyId { get; set; }

        [Display(Name = "SellerName")]
        public string SellerName { get; set; }

        [Display(Name = "BuyerName")]
        public string BuyerName { get; set; }

        [Display(Name = "IncludeRejected")]
        public bool IncludeRejected { get; set; }
    }
}
