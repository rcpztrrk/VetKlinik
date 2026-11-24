using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VetKlinik.Models
{
    public class Veteriner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = "";

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur")]
        [Display(Name = "Uzmanlık Alanı")]
        public string Uzmanlik { get; set; } = "";

        // Musaitlik: hekim her gun bu saatler arasinda randevu alabilir
        [Display(Name = "Mesai Başlangıç")]
        public TimeOnly BaslangicSaati { get; set; }

        [Display(Name = "Mesai Bitiş")]
        public TimeOnly BitisSaati { get; set; }

        // Hekimin sisteme giris yaptigi kullanici (Veteriner rolu)
        public string? KullaniciId { get; set; }
        public IdentityUser? Kullanici { get; set; }

        // hekimin verebildigi hizmetler
        public List<Hizmet> Hizmetler { get; set; } = new();

        public List<Randevu> Randevular { get; set; } = new();
    }
}
