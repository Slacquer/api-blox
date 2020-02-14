using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Examples
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            StartupLogger = LoggerFactory.Create(a => a.AddConsole());

            CreateHostBuilder(args).Build().Run();
        }

        public static ILoggerFactory StartupLogger { get; private set; }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
