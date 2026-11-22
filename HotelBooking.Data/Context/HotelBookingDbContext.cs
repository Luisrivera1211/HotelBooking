using HotelBooking.Data.Base;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Context
{
    public class HotelBookingDbContext : DbContext
    {
        private readonly ICurrentUser? _currentUser;

        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options, ICurrentUser? currentUser = null)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>().Property(r => r.Status).HasConversion<string>();
            modelBuilder.Entity<Reservation>().Property(r => r.Status).HasConversion<string>();
            modelBuilder.Entity<Notification>().Property(n => n.Status).HasConversion<string>();
            modelBuilder.Entity<Notification>().Property(n => n.Type).HasConversion<string>();
            modelBuilder.Entity<Employee>().Property(e => e.Role).HasConversion<string>();

            modelBuilder.Entity<RoomType>().Property(r => r.RatePerNight).HasPrecision(10, 2);
            modelBuilder.Entity<Reservation>().Property(r => r.Total).HasPrecision(10, 2);

            modelBuilder.Entity<Hotel>(e =>
            {
                e.Property(h => h.Name).IsRequired().HasMaxLength(50);
                e.Property(h => h.Address).IsRequired().HasMaxLength(100);
            });
            modelBuilder.Entity<Block>().Property(b => b.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<RoomType>().Property(r => r.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Room>(e =>
            {
                e.Property(r => r.Number).IsRequired().HasMaxLength(50);
                e.Property(r => r.Description).IsRequired().HasMaxLength(300);
                e.Property(r => r.Features).IsRequired().HasMaxLength(300);
            });
            modelBuilder.Entity<Guest>(e =>
            {
                e.Property(g => g.FullName).IsRequired().HasMaxLength(100);
                e.Property(g => g.Document).IsRequired().HasMaxLength(50);
                e.Property(g => g.Email).IsRequired().HasMaxLength(100);
                e.Property(g => g.Phone).IsRequired().HasMaxLength(50);
                e.HasIndex(g => g.Document).IsUnique();
            });
            modelBuilder.Entity<Employee>(e =>
            {
                e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(100);
                e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
                e.HasIndex(x => x.Email).IsUnique();
            });
            modelBuilder.Entity<Notification>().Property(n => n.Message).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Notification>().Property(n => n.Channel).IsRequired().HasMaxLength(30);

            modelBuilder.Entity<AuditLog>(e =>
            {
                e.Property(a => a.EntityName).IsRequired().HasMaxLength(50);
                e.Property(a => a.Action).IsRequired().HasMaxLength(30);
                e.Property(a => a.UserName).HasMaxLength(100);
                e.Property(a => a.Details).HasMaxLength(1000);
            });

            modelBuilder.Entity<Block>()
                .HasOne(b => b.Hotel).WithMany(h => h.Blocks)
                .HasForeignKey(b => b.HotelId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Floor>()
                .HasOne(f => f.Block).WithMany(b => b.Floors)
                .HasForeignKey(f => f.BlockId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Floor).WithMany(f => f.Rooms)
                .HasForeignKey(r => r.FloorId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType).WithMany(t => t.Rooms)
                .HasForeignKey(r => r.RoomTypeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room).WithMany(room => room.Reservations)
                .HasForeignKey(r => r.RoomId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest).WithMany(g => g.Reservations)
                .HasForeignKey(r => r.GuestId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Reservation).WithMany(r => r.Notifications)
                .HasForeignKey(n => n.ReservationId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Guest).WithMany(g => g.Notifications)
                .HasForeignKey(n => n.GuestId).OnDelete(DeleteBehavior.Restrict);
        }

        private static readonly HashSet<string> IgnoredProperties = new()
        {
            nameof(AuditEntity.Id), nameof(AuditEntity.CreatedAt), nameof(AuditEntity.UpdatedAt),
            nameof(AuditEntity.CreatedBy), nameof(AuditEntity.UpdatedBy)
        };

        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuditLogIsImmutable();
            ApplyAudit();

            var pending = CapturePendingAudit();

            var affected = await base.SaveChangesAsync(cancellationToken);

            if (pending.Count > 0)
            {
                var now = DateTime.UtcNow;
                var user = _currentUser?.Name;

                AuditLogs.AddRange(pending.Select(p => new AuditLog
                {
                    EntityName = p.EntityName,
                    EntityId = p.Entity.Id,
                    Action = p.Action,
                    UserName = user,
                    OccurredAt = now,
                    Details = p.Details
                }));
                await base.SaveChangesAsync(cancellationToken);
            }

            return affected;
        }

        private void EnsureAuditLogIsImmutable()
        {
            var tampered = ChangeTracker.Entries<AuditLog>()
                .Any(e => e.State is EntityState.Modified or EntityState.Deleted);

            if (tampered)
                throw new InvalidOperationException("El historial de auditoría no se puede modificar ni eliminar.");
        }

        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;
            var user = _currentUser?.Name;

            foreach (var entry in ChangeTracker.Entries<AuditEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = user;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = user;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                }
            }
        }

        private sealed record PendingAudit(AuditEntity Entity, string EntityName, string Action, string Details);

        private List<PendingAudit> CapturePendingAudit()
        {
            var list = new List<PendingAudit>();

            var entries = ChangeTracker.Entries<AuditEntity>()
                .Where(e => e.Entity is IAuditable
                            && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                string action;
                string details;

                if (entry.State == EntityState.Added)
                {
                    action = "Create";
                    details = Describe(entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    action = "Delete";
                    details = Describe(entry);
                }
                else
                {
                    action = "Update";
                    var changes = entry.Properties
                        .Where(p => !IgnoredProperties.Contains(p.Metadata.Name)
                                    && !Equals(p.OriginalValue, p.CurrentValue))
                        .Select(p => $"{p.Metadata.Name}: {p.OriginalValue} -> {p.CurrentValue}");
                    details = string.Join("; ", changes);
                    if (details.Length == 0)
                        details = Describe(entry);
                }

                if (details.Length > 1000)
                    details = details[..1000];

                list.Add(new PendingAudit(entry.Entity, entry.Entity.GetType().Name, action, details));
            }

            return list;
        }

        private static string Describe(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) =>
            string.Join("; ", entry.Properties
                .Where(p => !IgnoredProperties.Contains(p.Metadata.Name))
                .Select(p => $"{p.Metadata.Name}={p.CurrentValue}"));
    }
}
