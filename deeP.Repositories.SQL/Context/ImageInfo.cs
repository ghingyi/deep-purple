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
    public class ImageInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(36)]
        public string Id { get; set; }

        [Required]
        [MaxLength(2048)]
        public string Uri { get; set; }

        [Required]
        [MaxLength(160)]
        public string Title { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        [ForeignKey("Property")]
        public string PropertyId { get; set; }

        [InverseProperty("ImageInfos")]
        public virtual Property Property { get; set; }
    }
}
