using System.ComponentModel.DataAnnotations;

namespace VetKlinik.Models
{
    public enum RandevuDurumu
    {
        Bekliyor,
        Onaylandi,
        Reddedildi,
        Tamamlandi
    }

    public class Randevu
    {
        public int Id { get; set; }

        [Display(Name = "Tarih ve Saat")]
        public DateTime Tarih { get; set; }

        [Display(Name = "Durum")]
        public RandevuDurumu Durum { get; set; } = RandevuDurumu.Bekliyor;

        // Randevu alindigi andaki ucret ve sure saklaniyor,
        // hizmetin fiyati sonradan degisse bile eski randevu bozulmasin
        [Display(Name = "Ücret (TL)")]
        public decimal Ucret { get; set; }

        [Display(Name = "Süre (dk)")]
        public int SureDakika { get; set; }

        [Display(Name = "Muayene Notu")]
        public string? MuayeneNotu { get; set; }

        public int EvcilHayvanId { get; set; }
        public EvcilHayvan? EvcilHayvan { get; set; }

        public int HizmetId { get; set; }
        public Hizmet? Hizmet { get; set; }

        public int VeterinerId { get; set; }
        public Veteriner? Veteriner { get; set; }
    }
}
