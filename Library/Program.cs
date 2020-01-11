using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup Logging to catch any faults
            // Forgetting to enable Enrich.FromLogContext() is a frequent cause for
            // head-scratching when events are missing expected properties like RequestId.

            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.File(@$"{AppDomain.CurrentDomain.BaseDirectory}\ErrorLogs.Txt")
                                .CreateLogger();
            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // Serilog in ASP.NET Core 3 plugs into the generic host and not webBuilder.
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}