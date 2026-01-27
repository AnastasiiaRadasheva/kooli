using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kool.Models
{
    public class Koolitus
    {
        public int Id { get; set; }

        public int KeelekursusId { get; set; }
        public int OpetajaId { get; set; }

        public DateTime AlgusKuupaev { get; set; }
        public DateTime LoppKuupaev { get; set; }

        public decimal Hind { get; set; }
        public int MaxOsalejaid { get; set; }

        [ForeignKey(nameof(KeelekursusId))]
        public virtual Keelekursus Keelekursus { get; set; }

        [ForeignKey(nameof(OpetajaId))]
        public virtual Opetaja Opetaja { get; set; }

        public virtual ICollection<Registreerimine> Registreerimised { get; set; } = new List<Registreerimine>();
    }
}
