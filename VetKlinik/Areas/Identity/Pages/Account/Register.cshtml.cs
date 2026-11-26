using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetKlinik.Data;

namespace VetKlinik.Areas.Identity.Pages.Account
{
    // Hazır Identity kayıt sayfasının yerine kendi sayfam:
    // kayıt olan herkes "Uye" (hayvan sahibi) rolüyle başlıyor
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _kullaniciYoneticisi;
        private readonly SignInManager<IdentityUser> _girisYoneticisi;

        public RegisterModel(UserManager<IdentityUser> kullaniciYoneticisi, SignInManager<IdentityUser> girisYoneticisi)
        {
            _kullaniciYoneticisi = kullaniciYoneticisi;
            _girisYoneticisi = girisYoneticisi;
        }

        [BindProperty]
        public KayitBilgisi Girdi { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class KayitBilgisi
        {
            [Required(ErrorMessage = "E-posta zorunludur")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")]
            [Display(Name = "E-posta")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Şifre zorunludur")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "Şifre en az 3 karakter olmalı")]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Sifre { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Şifre (tekrar)")]
            [Compare("Sifre", ErrorMessage = "Şifreler aynı değil")]
            public string SifreTekrar { get; set; } = "";
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var kullanici = new IdentityUser
            {
                UserName = Girdi.Email,
                Email = Girdi.Email,
                EmailConfirmed = true
            };

            var sonuc = await _kullaniciYoneticisi.CreateAsync(kullanici, Girdi.Sifre);
            if (sonuc.Succeeded)
            {
                await _kullaniciYoneticisi.AddToRoleAsync(kullanici, VeriTohumlayici.UyeRol);
                await _girisYoneticisi.SignInAsync(kullanici, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var hata in sonuc.Errors)
                ModelState.AddModelError(string.Empty, hata.Description);

            return Page();
        }
    }
}
