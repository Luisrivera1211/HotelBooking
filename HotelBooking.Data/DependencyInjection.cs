using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HotelBooking.Data.Context;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Repositories;

namespace HotelBooking.Data
{
   public static class DependencyInjection
    {

        public static IServiceCollection AddDataDependencies(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<HotelBookingDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositorio genérico: cubre RoomType y cualquier entidad sin repositorio propio
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Repositorios concretos
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IGuestRepository, GuestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBlockRepository, BlockRepository>();

            return services;
        }

    }
}
