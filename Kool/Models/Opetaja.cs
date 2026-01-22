using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class Opetaja
    {
        public int Id { get; set; }
        public string Nimi { get; set; }
        public string Kvalifikatsioon { get; set; }
        public string FotoPath { get; set; }

        // связь с логином
        public string ApplicationUserId { get; set; }
    }
}