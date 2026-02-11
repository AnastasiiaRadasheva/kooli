using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kool.Models
{
    public class Opetaja
    {
        public int Id { get; set; }
        public string Nimi { get; set; }
        public string Kvalifikatsioon { get; set; }
        public string FotoPath { get; set; }
        
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Koolitus> Koolitused { get; set; } = new List<Koolitus>();
    }

}
