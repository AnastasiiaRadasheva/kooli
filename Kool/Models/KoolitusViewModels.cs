using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class KoolitusViewModels
    {
        public int Id { get; set; }

        [Required]
        public int KeelekursusId { get; set; }
        public Keelekursus Keelekursus { get; set; }

        [Required]
        public int OpetajaId { get; set; }
        public Opetaja Opetaja { get; set; }

        public DateTime AlgusKuupaev { get; set; }
        public DateTime LoppKuupaev { get; set; }
        
        public decimal Hind { get; set; }
        public int MaxOsalejaid { get; set; }
    }
}