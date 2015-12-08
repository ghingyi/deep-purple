using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class ImageInfoModel : DataModel
    {
        [Required]
        [Display(Name = "Uri")]
        public string Uri { get; set; }

        [Display(Name = "Title")]
        [MaxLength(160)]
        public string Title { get; set; }
    }
}
