using System.Text.Json.Serialization;
using HotelBooking.API.Services;
using HotelBooking.Business;
using HotelBooking.Data;
using HotelBooking.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en appsettings.");

builder.Services.AddDataDependencies(connectionString);
builder.Services.AddBusinessDependencies();

builder.Services.AddScoped<ICurrentUser, ApiCurrentUser>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
