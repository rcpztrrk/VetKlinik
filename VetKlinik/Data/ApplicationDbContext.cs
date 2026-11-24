using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetKlinik.Models;

namespace VetKlinik.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Klinik> Klinikler { get; set; }
    public DbSet<Hizmet> Hizmetler { get; set; }
    public DbSet<Veteriner> Veterinerler { get; set; }
    public DbSet<EvcilHayvan> EvcilHayvanlar { get; set; }
    public DbSet<AsiKaydi> AsiKayitlari { get; set; }
    public DbSet<Randevu> Randevular { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);   // Identity tablolari icin gerekli

        // Hekim - Hizmet coka cok iliski (ara tablo: VeterinerHizmet)
        builder.Entity<Veteriner>()
            .HasMany(v => v.Hizmetler)
            .WithMany(h => h.Veterinerler)
            .UsingEntity(j => j.ToTable("VeterinerHizmet"));

        // Randevusu olan hizmet / hekim / hayvan silinemesin, gecmis kayitlar kaybolmasin
        builder.Entity<Randevu>()
            .HasOne(r => r.Hizmet).WithMany()
            .HasForeignKey(r => r.HizmetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Randevu>()
            .HasOne(r => r.Veteriner).WithMany(v => v.Randevular)
            .HasForeignKey(r => r.VeterinerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Randevu>()
            .HasOne(r => r.EvcilHayvan).WithMany(e => e.Randevular)
            .HasForeignKey(r => r.EvcilHayvanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Hayvan silinince asi kayitlari da silinsin
        builder.Entity<AsiKaydi>()
            .HasOne(a => a.EvcilHayvan).WithMany(e => e.AsiKayitlari)
            .HasForeignKey(a => a.EvcilHayvanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
