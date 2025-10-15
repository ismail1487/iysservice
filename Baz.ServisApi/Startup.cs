using AutoMapper;
using Baz.AletKutusu;
using Baz.AOP.Logger.ExceptionLog;
using Baz.AOP.Logger.Http;
using Baz.IysServiceApi.Handlers;
using Baz.IysServiceApi.Helper;
using Baz.Model.Entity.Constants;
using Baz.Model.Pattern;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Baz.Service;

namespace Baz.IysServiceApi
{
    /// <summary>
    /// Uygulamayý ayaða kaldýran class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Uygulamayý ayaða kaldýran servisin yapýcý methodudur.
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables().Build();
        }

        /// <summary>
        /// Uygulamayý yapýlandýran özellik
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            SetCoreURL(Configuration.GetValue<string>("CoreUrl"));

            services.AddHttpContextAccessor();

            services.AddControllers(c => { c.Filters.Add(typeof(ModelValidationFilter), int.MinValue); });
            // services.AddDataProtection()
            //    .SetApplicationName(typeof(Startup).Assembly.FullName)
            //    .AddKeyManagementOptions(opt =>
            //    {
            //        opt.AutoGenerateKeys = true;
            //    }).UseCryptographicAlgorithms(
            //    new AuthenticatedEncryptorConfiguration()
            //    {
            //        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            //        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            //    })
            //.PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"Keys\ASP.NET\DataProtection-Keys"));
            // var instance = ActivatorUtilities.CreateInstance<ConnectionProtect>(services.BuildServiceProvider());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Baz.IysServiceApi", Version = "v1" });
                c.OperationFilter<DefaultHeaderParameter>();
            });
            services.AddDbContext<Repository.Pattern.IDataContext, Repository.Pattern.Entity.DataContext>(conf => conf.UseSqlServer((Configuration.GetConnectionString("Connection"))));
           
            
            
            services.AddSingleton<Baz.Mapper.Pattern.IDataMapper>(new Baz.Mapper.Pattern.Entity.DataMapper(GenerateConfiguratedMapper()));
            //////////////////////////////////////////SESSION SERVER AYARLARI/////////////////////////////////////////////////
            //Distributed session iþlemleri için session serverýn network baðlantýlarýný yapýlandýrýr.
            services.AddDistributedSqlServerCache(p =>
            {
                p.ConnectionString = (Configuration.GetConnectionString("SessionConnection"));
                p.SchemaName = "dbo";
                p.TableName = "SQLSessions";
            });
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = "/";
                options.Cookie.Name = "Test.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
            services.AddSession();
            //Http desteði olmadan paylaþýmlý session iþlemleri yapan servisi kayýt eder.
            services.AddTransient<Baz.SharedSession.ISharedSession, Baz.SharedSession.BaseSharedSession>();
            //Http desteði olan iþlemler için paylaþýmlý session nesnesinin kaydýný yapar.
            //BaseSharedSessionForHttpRequest iþlemleri için öncelikle BaseSharedSession servisi kayýt edilmelidir.
            services.AddTransient<Baz.SharedSession.ISharedSessionForHttpRequest, Baz.SharedSession.BaseSharedSessionForHttpRequest>();
            //////////////////////////////////////////////////////////////////////////////////////
            services.AddScoped<ISistemSayfalariService, SistemSayfalariService>();
            services.AddScoped<ISistemMenuTanimlariGenelService, SistemMenuTanimlariGenelService>();
            services.AddScoped<ISistemMenuTanimlariAyrintilarService, SistemMenuTanimlariAyrintilarService>();
            services.AddScoped<ISistemMenuTanimlariAyrintilarDillerService, SistemMenuTanimlariAyrintilarDillerService>();
            services.AddScoped<IParamUlkelerService, ParamUlkelerService>();
            services.AddScoped<IParamUlkelerSehirlerService, ParamUlkelerSehirlerService>();
            services.AddScoped<IParamUlkelerDillerService, ParamUlkelerDillerService>();
            services.AddScoped<IParamTelefonTipiService, ParamTelefonTipiService>();
            services.AddScoped<IParamTelefonTipiDillerService, ParamTelefonTipiDillerService>();
            services.AddScoped<IParamSistemModulleriService, ParamSistemModulleriService>();
            services.AddScoped<IParamPostaciIslemDurumTipleriService, ParamPostaciIslemDurumTipleriService>();
            services.AddScoped<IParamParaBirimleriService, ParamParaBirimleriService>();
            services.AddScoped<IParamOrganizasyonBirimleriService, ParamOrganizasyonBirimleriService>();
            services.AddScoped<IParamOlcumBirimleriService, ParamOlcumBirimleriService>();
            services.AddScoped<IParamOkulTipiService, ParamOkulTipiService>();
            services.AddScoped<IParamOkulTipiDillerService, ParamOkulTipiDillerService>();
            services.AddScoped<IParamMedyaTipleriService, ParamMedyaTipleriService>();
            services.AddScoped<IParamMedeniHalService, ParamMedeniHalService>();
            services.AddScoped<IParamMedeniHalDillerService, ParamMedeniHalDillerService>();
            services.AddScoped<IParamLokasyonTipleriService, ParamLokasyonTipleriService>();
            services.AddScoped<IParamLokasyonTipleriDillerService, ParamLokasyonTipleriDillerService>();
            services.AddScoped<IParamKurumTipleriService, ParamKurumTipleriService>();
            services.AddScoped<IParamKurumTipleriDillerService, ParamKurumTipleriDillerService>();
            services.AddScoped<IParamKurumSektorleriService, ParamKurumSektorleriService>();
            services.AddScoped<IParamKurumSektorleriDillerService, ParamKurumSektorleriDillerService>();
            services.AddScoped<IParamKurumLokasyonTipiService, ParamKurumLokasyonTipiService>();
            services.AddScoped<IParamKurumLokasyonTipiDillerService, ParamKurumLokasyonTipiDillerService>();
            services.AddScoped<IParamKurumBelgeTipleriService, ParamKurumBelgeTipleriService>();
            services.AddScoped<IParamKurumBelgeTipleriDillerService, ParamKurumBelgeTipleriDillerService>();
            services.AddScoped<IParamKimlikTipleriService, ParamKimlikTipleriService>();
            services.AddScoped<IParamKimlikTipleriDillerService, ParamKimlikTipleriDillerService>();
            services.AddScoped<IParamIliskiTurleriService, ParamIliskiTurleriService>();
            services.AddScoped<IParamIliskiTurleriDillerService, ParamIliskiTurleriDillerService>();
            services.AddScoped<IParamDinlerService, ParamDinlerService>();
            services.AddScoped<IParamDinlerDillerService, ParamDinlerDillerService>();
            services.AddScoped<IParamDilSeviyesiService, ParamDilSeviyesiService>();
            services.AddScoped<IParamDilSeviyesiDillerService, ParamDilSeviyesiDillerService>();
            services.AddScoped<IParamDillerService, ParamDillerService>();
            services.AddScoped<IParamDillerDillerService, ParamDillerDillerService>();
            services.AddScoped<IParamCinsiyetService, ParamCinsiyetService>();
            services.AddScoped<IParamCinsiyetDillerService, ParamCinsiyetDillerService>();
            services.AddScoped<IParamCalisanSayilariService, ParamCalisanSayilariService>();
            services.AddScoped<IParamBankalarSubelerService, ParamBankalarSubelerService>();
            services.AddScoped<IParamBankalarService, ParamBankalarService>();
            services.AddScoped<IParamBankalarDillerService, ParamBankalarDillerService>();
            //services.AddScoped<IParamAdresTipiService, IParamAdresTipiService>();
            services.AddScoped<IParamAdresTipiDillerService, ParamAdresTipiDillerService>();
            services.AddScoped<IMedyaKutuphanesiService, MedyaKutuphanesiService>();
            services.AddScoped<IKisiService, KisiService>();
            services.AddScoped<IIcerikKurumsalSablonTanimlariService, IcerikKurumsalSablonTanimlariService>();
            services.AddScoped<ICografyaTanimService, CografyaTanimService>();
            services.AddScoped<ICografyaAyrintilarService, CografyaAyrintilarService>();
            services.AddScoped<IParamKaynakTipleriService, ParamKaynakTipleriService>();
            services.AddScoped<IKaynakRezerveTanimlariService, KaynakRezerveTanimlariService>();
            services.AddScoped<IKaynakTanimlariService, KaynakTanimlariService>();
            services.AddScoped<IKaynakTanimlariMedyalarService, KaynakTanimlariMedyalarService>();
            services.AddScoped<ITakvimService, TakvimService>();
            services.AddScoped<IKaynakGunIciIstisnaTanimlariService, KaynakGunIciIstisnaTanimlariService>();

            //////////////////////////////////////////////////////////////////////////////////////
            services.AddScoped<Repository.Pattern.IUnitOfWork, Repository.Pattern.Entity.UnitOfWork>();
            services.AddScoped(typeof(Repository.Pattern.IRepository<>), typeof(Repository.Pattern.Entity.Repository<>));
            services.AddScoped(typeof(Service.Base.IService<>), typeof(Service.Base.Service<>));
            services.AddTransient<ILoginUser, LoginUserManager>();

            var types = typeof(Service.Base.IService<>).Assembly.GetTypes();
            var interfaces = types.Where(p => p.IsInterface && p.GetInterface("IService`1") != null).ToList();

            //Exception loglarýný iþleyen Baz.AOP.Logger.ExceptionLog servisinin kaydýný yapar


            services.AddTransient<IRequestHelper, RequestHelper>(provider =>
            {
                return new RequestHelper("", new RequestManagerHeaderHelperForHttp(provider).SetDefaultHeader());
            });

            foreach (var item in interfaces)
            {
                var serviceTypes = types.Where(p => p.GetInterface(item.Name) != null && !p.IsInterface).ToList();
                serviceTypes.ForEach(p => services.AddScoped(item, p));
            }
            services.AddResponseCompression();
            services.AddLocalization(apt => apt.ResourcesPath = "Resources");
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
        }

        ///  <summary>
        /// Bu yöntem çalýþma zamaný tarafýndan çaðrýlýr. HTTP istek ardýþýk düzenini yapýlandýrmak için bu yöntemi kullanýn.
        ///  </summary>
        ///  <param name="app"></param>
        ///  <param name="env"></param>
        ///  <param name="lifetime"></param>
        ///  <param name="cache"></param>
        ///  <param name="serviceProvider"></param>
        public void Configure(IApplicationBuilder app,IWebHostEnvironment env, IHostApplicationLifetime lifetime,
            IDistributedCache cache, IServiceProvider serviceProvider)
        {

            // Configure the Localization middleware
            app.UseRequestLocalization();

            ////////////////////////////////// SESSION SERVER AYARLARI/////////////////////////////////////
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseSession();
            lifetime.ApplicationStarted.Register(() =>
            {
                var currentTimeUTC = DateTime.UtcNow.ToString();
                byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
            });
            /////////////////////////////////////////////////////////////////////////////////////

      
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Baz.IysServiceApi v1");
            });
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseMiddleware<AuthMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
          
        }

        private static Profile GenerateConfiguratedMapper()
        {
            var mapper = Baz.Mapper.Pattern.Entity.DataMapperProfile.GenerateProfile();

            return mapper;
        }

        private static void SetCoreURL(string url)
        {
            LocalPortlar.CoreUrl = url;
        }
    }

    /// <summary>
    /// Connection Stringi gizlemeyi amaçlayan sýnýf
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConnectionProtect
    {
        private readonly IDataProtector _protector;

        // the 'provider' parameter is provided by DI
        /// <summary>
        /// Connection Stringi gizlemeyi amaçlayan sýnýfýn yapýcý methodu
        /// </summary>
        /// <param name="provider"></param>
        public ConnectionProtect(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(typeof(Startup).Assembly.FullName);
        }

        /// <summary>
        /// Connection stringi gizlemeyi amaçlayan method.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string UnProtect(string val) { return _protector.Unprotect(val); }

        //public static class HttpHelper
        //{
        //    static IHttpContextAccessor _accessor;
        //    public static void Configure(IHttpContextAccessor httpContextAccessor)
        //    {
        //        _accessor = httpContextAccessor; // why this is never called ?
        //    }

        //    public static HttpContext Current => _accessor.HttpContext;
        //}
    }
}