using MechanciShop.Infrustructure;
using MechanciShop.Infrustructure.Data;
using MechanciShop.Infrustructure.RealTime;
using MechanicShop.Application;
using MechanicShop.Client;
using Scalar.AspNetCore;
using Serilog;

namespace MechanicShop.Api
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services
                .AddPresentation(builder.Configuration)
                .AddApplication()
                .AddInfrustructure(builder.Configuration);

            builder.Host.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference();

                await app.InitialiseDatabaseAsync();

                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCoreMiddlewares(builder.Configuration);

            app.MapControllers();

            app.UseAntiforgery();

            app.MapStaticAssets();

            app.MapRazorComponents<App>().AllowAnonymous()
                .AddInteractiveWebAssemblyRenderMode();
               

            app.MapHub<WorkOrderHub>("/hubs/workorders");

            app.Run();
        }
    }
}
