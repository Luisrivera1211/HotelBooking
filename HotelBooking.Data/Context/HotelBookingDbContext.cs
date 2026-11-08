using Microsoft.EntityFrameworkCore;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Base;

namespace HotelBooking.Data.Context
{
    public class HotelBookingDbContext : DbContext
    {
        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options)
        { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Block> Blocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Notification>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Employee>()
                .Property(r => r.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();

            modelBuilder.Entity<RoomType>()
                .Property(r => r.RatePerNight)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Hotel>(e =>
            {
                e.Property(h => h.Name).
                IsRequired()
                .HasMaxLength(50);

                e.Property(h => h.Address)
                .IsRequired()
                .HasMaxLength(100);
            });

            modelBuilder.Entity<Block>()
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<RoomType>()
                .Property(r => r.Name).
                IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Room>()
                .Property(r => r.Number)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Guest>(e =>
            {
                e.Property(g => g.FullName)
                .IsRequired()
                .HasMaxLength(100);

                e.Property(g => g.Document)
                .IsRequired()
                .HasMaxLength(50);

                e.Property(g => g.Email)
                .IsRequired()
                .HasMaxLength(100);

                e.Property(g => g.Phone)
                .IsRequired()
                .HasMaxLength(50);

                e.HasIndex(g => g.Document)
                .IsUnique();
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

                e.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

                e.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

                e.HasIndex(x => x.Email)
                .IsUnique();

            });

            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Block>()
                .HasOne(b => b.Hotel)
                .WithMany(h => h.Blocks)
                .HasForeignKey(b => b.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Floor>()
                .HasOne(f => f.Block)
                .WithMany(b => b.Floors)
                .HasForeignKey(f => f.BlockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Floor)
                .WithMany(f => f.Rooms)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType)
                .WithMany(t => t.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(room => room.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithMany(g => g.Reservations)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Reservation)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Guest)
                .WithMany(g => g.Notifications)
                .HasForeignKey(n => n.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit()
        {
            foreach (var entry in ChangeTracker.Entries<AuditEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                }
            }
        }
    }
}