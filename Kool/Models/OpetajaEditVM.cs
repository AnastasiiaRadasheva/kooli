using System.ComponentModel.DataAnnotations;

namespace Kool.Models
{
    public class OpetajaEditVM
    {
        public int Id { get; set; }

        [Required]
        public string Nimi { get; set; }

        public string Kvalifikatsioon { get; set; }
        public string FotoPath { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        public string ApplicationUserId { get; set; }
    }
}
