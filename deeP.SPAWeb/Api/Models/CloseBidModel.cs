using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace deeP.SPAWeb.Api.Models
{
    public class CloseBidModel
    {
        [Required]
        [Display(Name = "Bid")]
        public BidModel Bid { get; set; }

        [Required]
        [Display(Name = "Accept")]
        public bool Accept { get; set; }
    }
}