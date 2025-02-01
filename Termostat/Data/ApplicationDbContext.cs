using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Termostat.Models;

namespace Termostat.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Mieszkanie> Mieszkania { get; set; }
        public DbSet<MieszkanieWspolokator> MieszkanieWspolokatorzy { get; set; }
        public DbSet<Zaproszenie> Zaproszenia {get; set;}
        public DbSet<Harmonogram> Harmonogram { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji wiele-do-wielu
            modelBuilder.Entity<MieszkanieWspolokator>()
                .HasKey(mw => new { mw.MieszkanieId, mw.UserId });

            modelBuilder.Entity<MieszkanieWspolokator>()
                .HasOne(mw => mw.Mieszkanie)
                .WithMany(m => m.Wspolokatorzy)
                .HasForeignKey(mw => mw.MieszkanieId);

            modelBuilder.Entity<MieszkanieWspolokator>()
                .HasOne(mw => mw.User)
                .WithMany()
                .HasForeignKey(mw => mw.UserId);
            modelBuilder.Entity<Zaproszenie>()
           .HasOne(z => z.Nadawca)
           .WithMany()
           .HasForeignKey(z => z.NadawcaId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zaproszenie>()
                .HasOne(z => z.Odbiorca)
                .WithMany()
                .HasForeignKey(z => z.OdbiorcaId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Harmonogram>()
      .HasOne(h => h.Mieszkanie)
      .WithMany(m => m.Harmonogramy)
      .HasForeignKey(h => h.MieszkanieId);
        }

    }
}
