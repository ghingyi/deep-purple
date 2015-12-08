using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public class PageFilter
    {
        [Display(Name = "Skip")]
        public uint? Skip { get; set; }

        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Take")]
        public uint? Take { get; set; }
    }
}
