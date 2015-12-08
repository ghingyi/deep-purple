using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction.Models
{
    public abstract class DataModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }
    }
}
