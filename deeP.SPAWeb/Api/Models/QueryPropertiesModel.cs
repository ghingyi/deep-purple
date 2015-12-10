using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace deeP.SPAWeb.Api.Models
{
    public class QueryPropertiesModel
    {
        [Required]
        [Display(Name = "Filter")]
        public PropertyFilter Filter { get; set; }

        [Required]
        [Display(Name = "Sorting")]
        public PropertySorting Sorting{ get; set; }
    }
}