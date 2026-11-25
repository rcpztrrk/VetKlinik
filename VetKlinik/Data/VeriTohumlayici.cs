using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetKlinik.Models;

namespace VetKlinik.Data
{
    // Ilk calistirmada gerekli verileri olusturur (roller, admin, klinik bilgisi)
    public static class VeriTohumlayici
    {
        public const string AdminRol = "Admin";
        public const string VeterinerRol = "Veteriner";
        public const string UyeRol = "Uye";

        public static async Task BaslatAsync(IServiceProvider servisler)
        {
            using var scope = servisler.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var rolYoneticisi = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var kullaniciYoneticisi = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // bekleyen migration varsa uygula
            await db.Database.MigrateAsync();

            // roller
            foreach (var rol in new[] { AdminRol, VeterinerRol, UyeRol })
            {
                if (!await rolYoneticisi.RoleExistsAsync(rol))
                    await rolYoneticisi.CreateAsync(new IdentityRole(rol));
            }

            // admin hesabi (odevde istenen: ogrencinumarasi@sakarya.edu.tr / sau)
            string adminMail = "b241210560@sakarya.edu.tr";
            var admin = await kullaniciYoneticisi.FindByEmailAsync(adminMail);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminMail, Email = adminMail, EmailConfirmed = true };
                await kullaniciYoneticisi.CreateAsync(admin, "sau");
                await kullaniciYoneticisi.AddToRoleAsync(admin, AdminRol);
            }

            // klinik bilgisi (tek klinik)
            if (!await db.Klinikler.AnyAsync())
            {
                db.Klinikler.Add(new Klinik
                {
                    Ad = "Pati Veteriner Kliniği",
                    Adres = "Serdivan / Sakarya",
                    Telefon = "0264 000 00 00",
                    AcilisSaati = new TimeOnly(9, 0),
                    KapanisSaati = new TimeOnly(18, 0)
                });
            }

            // ornek hizmetler, bos acilmasin diye
            if (!await db.Hizmetler.AnyAsync())
            {
                db.Hizmetler.AddRange(
                    new Hizmet { Ad = "Genel Muayene", SureDakika = 20, Ucret = 500 },
                    new Hizmet { Ad = "Aşı Uygulaması", SureDakika = 15, Ucret = 350 },
                    new Hizmet { Ad = "Kısırlaştırma", SureDakika = 90, Ucret = 3000 },
                    new Hizmet { Ad = "Diş Temizliği", SureDakika = 45, Ucret = 1200 },
                    new Hizmet { Ad = "Tıraş ve Bakım", SureDakika = 60, Ucret = 600 }
                );
            }

            await db.SaveChangesAsync();
        }
    }
}
