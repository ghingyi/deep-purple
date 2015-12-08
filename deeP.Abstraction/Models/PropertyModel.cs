using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class PropertyModel : DataModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Type")]
        public PropertyType Type { get; set; }

        [Required]
        [Display(Name = "Bedrooms")]
        public byte Bedrooms { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Location details")]
        public IDictionary<string, object> LocationDetails { get; set; }

        [Display(Name = "Images")]
        public ImageInfoModel[] ImageInfos { get; set; }

        [Required]
        [Display(Name = "State")]
        public PropertyState State { get; set; }
    }
}
