using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kool.Models
{
    public class Registreerimine
    {
        public int Id { get; set; }

        [Required]
        public int KoolitusId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(KoolitusId))]
        public virtual Koolitus Koolitus { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public RegistreerimineStaatus Staatus { get; set; } = RegistreerimineStaatus.Pending;
    }

    public enum RegistreerimineStaatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
