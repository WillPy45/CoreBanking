
using CoreBankingWebApplication.Database;
using CoreBankingWebApplication.Middlewares;
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
                .WriteTo
                .File(builder.Configuration["LogPath:DefaultPath"])
                .WriteTo
                .Console();
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

           
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.MapControllers();

            Console.WriteLine("Before running");
            app.Run();
            Console.WriteLine("After running");
        }
    }
}
