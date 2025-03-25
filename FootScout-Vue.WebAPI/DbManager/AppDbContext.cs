using FootScout_Vue.WebAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.DbManager
{
    // Kontekst bazy danych do pracy z danymi z bazy danych, wykorzystując Entity Framework Core oraz IdentityDbContext do zarządzania użytkownikami.
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // Tabele bazy danych...
        public DbSet<Achievements> Achievements { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ClubHistory> ClubHistories { get; set; }
        public DbSet<ClubOffer> ClubOffers { get; set; }
        public DbSet<FavoritePlayerAdvertisement> FavoritePlayerAdvertisements { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OfferStatus> OfferStatuses { get; set; }
        public DbSet<PlayerAdvertisement> PlayerAdvertisements { get; set; }
        public DbSet<PlayerFoot> PlayerFeet { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<SalaryRange> SalaryRanges { get; set; }
        public DbSet<User> Users { get; set; }

        // Konfiguracja modelu bazy danych (definiowanie relacji między encjami, kluczy obcych, strategii usuwania)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indywidualne konfiguracje...
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.PlayerAdvertisement)
                .WithMany()
                .HasForeignKey(co => co.PlayerAdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.OfferStatus)
                .WithMany()
                .HasForeignKey(co => co.OfferStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.PlayerPosition)
                .WithMany()
                .HasForeignKey(co => co.PlayerPositionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubOffer>()
                .HasOne(co => co.ClubMember)
                .WithMany()
                .HasForeignKey(co => co.ClubMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubHistory>()
                .HasOne(ch => ch.Achievements)
                .WithOne()
                .HasForeignKey<ClubHistory>(ch => ch.AchievementsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubHistory>()
               .HasOne(ca => ca.PlayerPosition)
               .WithMany()
               .HasForeignKey(ca => ca.PlayerPositionId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClubHistory>()
                .HasOne(ch => ch.Player)
                .WithMany()
                .HasForeignKey(ch => ch.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoritePlayerAdvertisement>()
               .HasOne(paf => paf.PlayerAdvertisement)
               .WithMany()
               .HasForeignKey(paf => paf.PlayerAdvertisementId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoritePlayerAdvertisement>()
                .HasOne(paf => paf.User)
                .WithMany()
                .HasForeignKey(paf => paf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany()
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
                .HasOne(pa => pa.Player)
                .WithMany()
                .HasForeignKey(pa => pa.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
               .HasOne(pa => pa.PlayerPosition)
               .WithMany()
               .HasForeignKey(pa => pa.PlayerPositionId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
               .HasOne(pa => pa.PlayerFoot)
               .WithMany()
               .HasForeignKey(pa => pa.PlayerFootId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerAdvertisement>()
                .HasOne(pa => pa.SalaryRange)
                .WithMany()
                .HasForeignKey(pa => pa.SalaryRangeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Problem>()
                .HasOne(p => p.Requester)
                .WithMany()
                .HasForeignKey(p => p.RequesterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}