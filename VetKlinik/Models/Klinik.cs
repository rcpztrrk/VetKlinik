using System.ComponentModel.DataAnnotations;

namespace VetKlinik.Models
{
    // Tek klinik uzerinden calisiyoruz, tabloda tek satir olacak
    public class Klinik
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Klinik adı zorunludur")]
        [Display(Name = "Klinik Adı")]
        public string Ad { get; set; } = "";

        [Display(Name = "Adres")]
        public string? Adres { get; set; }

        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [Display(Name = "Açılış Saati")]
        public TimeOnly AcilisSaati { get; set; }

        [Display(Name = "Kapanış Saati")]
        public TimeOnly KapanisSaati { get; set; }
    }
}
