using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class Keelekursus
    {
        public int Id { get; set; }
        public string Nimetus { get; set; }      // nt "Inglise keel algajatele"
        public string Keel { get; set; }          // Inglise, Saksa jne
        public string Tase { get; set; }          // A1–C2
        public string Kirjeldus { get; set; }
    }
}