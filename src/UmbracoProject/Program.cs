using Microsoft.OpenApi.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UmbracoProject.NotificationHandler;
using UmbracoProject.Repository;
using UmbracoProject.Service;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Umbraco Headless API",
                Version = "v1",
                Description = "Trip endpoints for the headless Umbraco project"
            });
        });

        builder.Services.AddControllers().AddJsonOptions(o =>
            o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

        builder.Services.AddScoped<IAdminTripService, AdminTripService>();
        builder.Services.AddScoped<ITripService, TripService>();
        builder.Services.AddScoped<ITripRepository, TripRepository>();
        builder.Services.AddScoped<IRocketStatusService, RocketStatusService>();
        builder.Services.AddScoped<IRocketStatusRepository, RocketStatusRepository>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IPriceCalculatorService, PriceCalculatorService>();


        builder.CreateUmbracoBuilder()
            .AddBackOffice()
            .AddWebsite()
            .AddComposers()
            .AddDeliveryApi()
            .AddNotificationAsyncHandler<ContentPublishedNotification, RocketPublishedHandler>()
            .Build();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Umbraco Headless API v1");
            c.RoutePrefix = "swagger";
        });

        await app.BootUmbracoAsync();

        app.UseHttpsRedirection();

        app.UseUmbraco()
            .WithMiddleware(u =>
            {
                u.UseBackOffice();
                u.UseWebsite();
            })
            .WithEndpoints(u =>
            {
                u.UseBackOfficeEndpoints();
                u.UseWebsiteEndpoints();
            });

        app.MapControllers();

        await app.RunAsync();
    }
}
