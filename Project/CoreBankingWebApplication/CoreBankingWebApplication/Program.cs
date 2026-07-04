
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Middlewares;
using CoreBankingWebApplication.Migrations;
using CoreBankingWebApplication.Services;
using Serilog;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;

namespace CoreBankingWebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} started ");
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            

            builder.Services.AddDbContext<ApplicationDbContext>();
            
            /*
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.File(
                    builder.Configuration["LogPath:DefaultPath"]
                )
                .CreateLogger();
            */
            //Crucial para cambiar el motor de log utilizado en ILogger<T> por el de Serilog
            builder.Services.AddSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                .MinimumLevel.Warning() // Nivel mínimo global para capturar datos en consola
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information) //Admitimos el lifetime
                .MinimumLevel.Override("CoreBanking", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console()          // La consola registrará todo desde 'Information' hacia arriba
                .WriteTo.File(
                    path: builder.Configuration["LogPath:DefaultPath"] ?? "logs/critical.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal); //Eventos criticos para captura en fichero
            });
            
                        
            builder.Services.AddControllers();

            builder.Services.AddScoped<TransferBalanceBetweenAccountService>();
            builder.Services.AddScoped<CurrencyCrudService>();
            builder.Services.AddScoped<CurrencyExchangeReserveCrudService>();
            builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            builder.Services.AddScoped<CurrencyExchangeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
           
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
