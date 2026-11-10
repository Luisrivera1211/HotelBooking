using HotelBooking.Business.Interfaces;
using HotelBooking.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessDependencies(this IServiceCollection services)
        {
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IBlockService, BlockService>();
            services.AddScoped<IFloorService, FloorService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IGuestService, GuestService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IReservationService, ReservationService>();

            // Envío real de correo detrás de una interfaz (DIP)
            services.AddScoped<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
