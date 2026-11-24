using System.ComponentModel.DataAnnotations;

namespace VetKlinik.Models
{
    public class AsiKaydi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Aşı adı zorunludur")]
        [Display(Name = "Aşı Adı")]
        public string AsiAdi { get; set; } = "";

        [DataType(DataType.Date)]
        [Display(Name = "Yapıldığı Tarih")]
        public DateTime YapildigiTarih { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Sonraki Doz")]
        public DateTime? SonrakiDozTarihi { get; set; }   // tek dozluk asilarda bos kalabilir

        public int EvcilHayvanId { get; set; }
        public EvcilHayvan? EvcilHayvan { get; set; }
    }
}
