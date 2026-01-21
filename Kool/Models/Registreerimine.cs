using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kool.Models
{
    public class Registreerimine
    {

        string Id { get; set; }

        [Required]
        int nimi { get; set }

    }
}