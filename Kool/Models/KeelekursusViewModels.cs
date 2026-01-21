using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class KeelekursusViewModels
    {
        public int Id { get; set; }

        [Required]
        public string Nimetus { get; set; }

        [Required]
        public string Keel { get; set; }

        [Required]
        public string Tase { get; set; } 
        public string Kirjeldus { get; set; }

    }
}