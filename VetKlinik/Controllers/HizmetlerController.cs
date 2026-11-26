using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetKlinik.Data;
using VetKlinik.Models;

namespace VetKlinik.Controllers
{
    // Hizmet ekleme/düzenleme/silme sadece Admin'e açık
    [Authorize(Roles = VeriTohumlayici.AdminRol)]
    public class HizmetlerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HizmetlerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Hizmet listesini herkes görebilir
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _db.Hizmetler.OrderBy(h => h.Ad).ToListAsync();
            return View(hizmetler);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet hizmet)
        {
            if (!ModelState.IsValid)
                return View(hizmet);

            _db.Hizmetler.Add(hizmet);
            await _db.SaveChangesAsync();
            TempData["Mesaj"] = "Hizmet eklendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _db.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hizmet hizmet)
        {
            if (id != hizmet.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(hizmet);

            _db.Hizmetler.Update(hizmet);
            await _db.SaveChangesAsync();
            TempData["Mesaj"] = "Hizmet güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _db.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _db.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            // randevusu olan hizmet silinmesin, geçmiş kayıtlar bozulur
            bool randevusuVar = await _db.Randevular.AnyAsync(r => r.HizmetId == id);
            if (randevusuVar)
            {
                TempData["Hata"] = "Bu hizmete ait randevular olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _db.Hizmetler.Remove(hizmet);
            await _db.SaveChangesAsync();
            TempData["Mesaj"] = "Hizmet silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
