using deeP.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Repositories.SQL.Context
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(36)]
        public string Id { get; set; }
        
        [Index("IX_Owner", 1)]
        [Required]
        [MaxLength(256)]
        public string Owner { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public PropertyType Type { get; set; }

        [Required]
        public byte Bedrooms { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [MaxLength(300)]
        public string Address { get; set; }

        [Required]
        [MaxLength(4096)]
        public string LocationDetails { get; set; }

        [Required]
        public PropertyState State { get; set; }

        private ICollection<Bid> _bids;
        [InverseProperty("Property")]
        public virtual ICollection<Bid> Bids { get { return _bids ?? (_bids = new Collection<Bid>()); } protected set { _bids = value; } }
        private ICollection<ImageInfo> _imageInfos;
        [InverseProperty("Property")]
        public virtual ICollection<ImageInfo> ImageInfos { get { return _imageInfos ?? (_imageInfos = new Collection<ImageInfo>()); } protected set { _imageInfos = value; } }
    }
}
