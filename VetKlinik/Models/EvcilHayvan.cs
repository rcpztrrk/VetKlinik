using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VetKlinik.Models
{
    public class EvcilHayvan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hayvanın adı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Adı")]
        public string Ad { get; set; } = "";

        [Required(ErrorMessage = "Tür zorunludur")]
        [Display(Name = "Tür")]
        public string Tur { get; set; } = "";   // kedi, kopek, kus...

        [Display(Name = "Irk")]
        public string? Irk { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime DogumTarihi { get; set; }

        [Range(0.01, 200, ErrorMessage = "Kilo 0 ile 200 arasında olmalı")]
        [Display(Name = "Kilo (kg)")]
        public double Kilo { get; set; }

        [Display(Name = "Fotoğraf")]
        public string? FotografYolu { get; set; }

        // hayvanin sahibi (Uye rolundeki kullanici)
        public string SahipId { get; set; } = "";
        public IdentityUser? Sahip { get; set; }

        public List<AsiKaydi> AsiKayitlari { get; set; } = new();
        public List<Randevu> Randevular { get; set; } = new();
    }
}
