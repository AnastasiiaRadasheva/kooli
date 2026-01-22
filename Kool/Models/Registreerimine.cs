using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class Registreerimine
    {
        public int Id { get; set; }

        public int KoolitusId { get; set; }

        public string ApplicationUserId { get; set; }

        public string Staatus { get; set; }
    }
}