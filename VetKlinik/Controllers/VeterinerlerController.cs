using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetKlinik.Data;
using VetKlinik.Models;

namespace VetKlinik.Controllers
{
    [Authorize(Roles = VeriTohumlayici.AdminRol)]
    public class VeterinerlerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _kullaniciYoneticisi;

        public VeterinerlerController(ApplicationDbContext db, UserManager<IdentityUser> kullaniciYoneticisi)
        {
            _db = db;
            _kullaniciYoneticisi = kullaniciYoneticisi;
        }

        // Hekim listesini herkes görebilir
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var hekimler = await _db.Veterinerler
                .Include(v => v.Hizmetler)
                .OrderBy(v => v.AdSoyad)
                .ToListAsync();
            return View(hekimler);
        }

        public async Task<IActionResult> Create()
        {
            await HizmetleriHazirla(new List<int>());
            return View(new Veteriner { BaslangicSaati = new TimeOnly(9, 0), BitisSaati = new TimeOnly(17, 0) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veteriner veteriner, string email, string sifre, int[] secilenHizmetler)
        {
            SaatleriKontrolEt(veteriner);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre))
                ModelState.AddModelError(string.Empty, "Hekimin giriş yapacağı e-posta ve şifre zorunludur.");

            if (!ModelState.IsValid)
            {
                await HizmetleriHazirla(secilenHizmetler.ToList());
                return View(veteriner);
            }

            // hekimin sisteme giriş yapacağı hesap (Veteriner rolü)
            var kullanici = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var sonuc = await _kullaniciYoneticisi.CreateAsync(kullanici, sifre);
            if (!sonuc.Succeeded)
            {
                foreach (var hata in sonuc.Errors)
                    ModelState.AddModelError(string.Empty, hata.Description);
                await HizmetleriHazirla(secilenHizmetler.ToList());
                return View(veteriner);
            }
            await _kullaniciYoneticisi.AddToRoleAsync(kullanici, VeriTohumlayici.VeterinerRol);

            veteriner.KullaniciId = kullanici.Id;
            veteriner.Hizmetler = await _db.Hizmetler.Where(h => secilenHizmetler.Contains(h.Id)).ToListAsync();

            _db.Veterinerler.Add(veteriner);
            await _db.SaveChangesAsync();
            TempData["Mesaj"] = "Hekim eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var veteriner = await _db.Veterinerler.Include(v => v.Hizmetler).FirstOrDefaultAsync(v => v.Id == id);
            if (veteriner == null)
                return NotFound();

            await HizmetleriHazirla(veteriner.Hizmetler.Select(h => h.Id).ToList());
            return View(veteriner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veteriner veteriner, int[] secilenHizmetler)
        {
            if (id != veteriner.Id)
                return BadRequest();

            SaatleriKontrolEt(veteriner);
            if (!ModelState.IsValid)
            {
                await HizmetleriHazirla(secilenHizmetler.ToList());
                return View(veteriner);
            }

            var kayitli = await _db.Veterinerler.Include(v => v.Hizmetler).FirstOrDefaultAsync(v => v.Id == id);
            if (kayitli == null)
                return NotFound();

            kayitli.AdSoyad = veteriner.AdSoyad;
            kayitli.Uzmanlik = veteriner.Uzmanlik;
            kayitli.BaslangicSaati = veteriner.BaslangicSaati;
            kayitli.BitisSaati = veteriner.BitisSaati;

            // hizmet seçimlerini yenile
            kayitli.Hizmetler.Clear();
            var hizmetler = await _db.Hizmetler.Where(h => secilenHizmetler.Contains(h.Id)).ToListAsync();
            kayitli.Hizmetler.AddRange(hizmetler);

            await _db.SaveChangesAsync();
            TempData["Mesaj"] = "Hekim bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var veteriner = await _db.Veterinerler.Include(v => v.Hizmetler).FirstOrDefaultAsync(v => v.Id == id);
            if (veteriner == null)
                return NotFound();

            return View(veteriner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veteriner = await _db.Veterinerler.Include(v => v.Hizmetler).FirstOrDefaultAsync(v => v.Id == id);
            if (veteriner == null)
                return NotFound();

            if (await _db.Randevular.AnyAsync(r => r.VeterinerId == id))
            {
                TempData["Hata"] = "Bu hekimin randevuları olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            string? kullaniciId = veteriner.KullaniciId;
            _db.Veterinerler.Remove(veteriner);
            await _db.SaveChangesAsync();

            // hekimin giriş hesabını da sil
            if (kullaniciId != null)
            {
                var kullanici = await _kullaniciYoneticisi.FindByIdAsync(kullaniciId);
                if (kullanici != null)
                    await _kullaniciYoneticisi.DeleteAsync(kullanici);
            }

            TempData["Mesaj"] = "Hekim silindi.";
            return RedirectToAction(nameof(Index));
        }

        // Formdaki hizmet onay kutuları için tüm hizmetler ve seçili olanlar
        private async Task HizmetleriHazirla(List<int> secilenler)
        {
            ViewBag.Hizmetler = await _db.Hizmetler.OrderBy(h => h.Ad).ToListAsync();
            ViewBag.Secilenler = secilenler;
        }

        private void SaatleriKontrolEt(Veteriner veteriner)
        {
            if (veteriner.BaslangicSaati >= veteriner.BitisSaati)
                ModelState.AddModelError(nameof(Veteriner.BitisSaati), "Mesai bitişi başlangıçtan sonra olmalı.");
        }
    }
}
