using BandApi.DataContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace BandApi
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // EnsureDatabaseRecreation(host);

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private static void EnsureDatabaseRecreation(IHost host)
        {
            using var scope = host.Services.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetService<AlbumBandDataContext>();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogError(e, "Something went wrong while performing the migrations");
            }


        }
    }

}
