using Baz.AOP.Logger.ExceptionLog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Baz.IysServiceApi
{
    /// <summary>
    /// API'ýn çalýþmasý için gereken Main() methodunu barýndýran class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// API'ý ayaða kaldýran method.
        /// </summary>
        /// <param name="args"></param>
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Host oluþturan method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).BazConfigureLogging(); // Graylog a log yazýlabilmesi için Baz.AOP.Logger.ExceptionLog paketi eklenip BazConfigureLogging() fonksiyonu çaðrýlýr.

        // BazConfigureLogging() fonksiyonu graylog için gerekli netwok ayarlarýný yapar. network ayarlarýný appsetting.json dan alýr.
    }
}