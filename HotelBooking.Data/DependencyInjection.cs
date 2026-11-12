using HotelBooking.Data.Context;
using HotelBooking.Data.Interfaces;
using HotelBooking.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<HotelBookingDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IGuestRepository, GuestRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}
