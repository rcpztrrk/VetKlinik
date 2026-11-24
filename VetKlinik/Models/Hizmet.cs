using System.ComponentModel.DataAnnotations;

namespace VetKlinik.Models
{
    public class Hizmet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Hizmet Adı")]
        public string Ad { get; set; } = "";

        [Range(5, 480, ErrorMessage = "Süre 5 ile 480 dakika arasında olmalı")]
        [Display(Name = "Süre (dk)")]
        public int SureDakika { get; set; }

        [Range(0, 100000, ErrorMessage = "Geçerli bir ücret girin")]
        [Display(Name = "Ücret (TL)")]
        public decimal Ucret { get; set; }

        // bu hizmeti verebilen hekimler
        public List<Veteriner> Veterinerler { get; set; } = new();
    }
}
