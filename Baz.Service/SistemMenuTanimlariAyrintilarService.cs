using Baz.AOP.Logger.ExceptionLog;
using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// Sistem Menü Tanım Ayrıntıları Servisi için gerekli methodların yer aldığı sınıftır.
    /// </summary>
    public interface ISistemMenuTanimlariAyrintilarService : Base.IService<SistemMenuTanimlariAyrintilar>
    {
        /// <summary>
        /// Listeleme methodu
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns></returns>
        Result<List<MenuListResponse>> List(MenuListRequest request);

        /// <summary>
        /// Yetki merkezi için sayfa verilerini getiren method
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Result<List<SistemSayfalari>> YetkiIcinSayfaGetir(string dilKodu);
    }

    /// <summary>
    /// Sistem Menü Tanım Ayrıntıları Servisi ile ilgili işlemleri yöneten servis sınıfı
    /// </summary>
    public class SistemMenuTanimlariAyrintilarService : Base.Service<SistemMenuTanimlariAyrintilar>, ISistemMenuTanimlariAyrintilarService
    {
        private readonly ISistemSayfalariService _sistemSayfalariService;
        private readonly IParamDillerService _paramDillerService;
        private readonly ISistemMenuTanimlariGenelService _sistemMenuTanimlariGenelService;
        private readonly ISistemMenuTanimlariAyrintilarDillerService _sistemMenuTanimlariAyrintilarDillerService;
        /// <summary>
        /// Sistem Menü Tanım Ayrıntıları Servisi ile ilgili işlemleri yöneten servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="paramDillerService"></param>
        /// <param name="sistemMenuTanimlariGenelService"></param>
        /// <param name="sistemSayfalariService"></param>
        public SistemMenuTanimlariAyrintilarService(IRepository<SistemMenuTanimlariAyrintilar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<SistemMenuTanimlariAyrintilarService> logger, IParamDillerService paramDillerService, ISistemMenuTanimlariGenelService sistemMenuTanimlariGenelService, ISistemSayfalariService sistemSayfalariService, ISistemMenuTanimlariAyrintilarDillerService sistemMenuTanimlariAyrintilarDillerService) : base(repository, dataMapper, serviceProvider, logger)
        {
            _sistemSayfalariService = sistemSayfalariService;
            _paramDillerService = paramDillerService;
            _sistemMenuTanimlariGenelService = sistemMenuTanimlariGenelService;
            _sistemMenuTanimlariAyrintilarDillerService = sistemMenuTanimlariAyrintilarDillerService;
        }

        /// <summary>
        /// Listeleme methodu
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns></returns>
        public Result<List<MenuListResponse>> List(MenuListRequest request)
        {
            if (request.DilId == 0 && string.IsNullOrEmpty(request.DilKodu))
            {
                throw new OctapullException(OctapullExceptions.MissingDataError);
            }

            // DilId belirleme
            int dilId = request.DilId;
            if (dilId == 0)
            {
                if (string.IsNullOrEmpty(request.DilKodu))
                    throw new OctapullException(OctapullExceptions.MissingDataError);

                var dilItem = _paramDillerService.SingleOrDefault(request.DilKodu);
                if (dilItem?.Value?.TabloID > 0)
                    dilId = dilItem.Value.TabloID;
                else
                    throw new OctapullException(OctapullExceptions.MissingDataError);
            }

            // MenuId belirleme
            int menuId = request.MenuId;
            if (menuId == 0)
            {
                if (string.IsNullOrEmpty(request.Name))
                    throw new OctapullException(OctapullExceptions.MissingDataError);

                var menuItem = _sistemMenuTanimlariGenelService.SingleOrDefault(request.Name);
                if (menuItem?.Value?.TabloID > 0)
                    menuId = menuItem.Value.TabloID;
                else
                    throw new OctapullException(OctapullExceptions.MissingDataError);
            }

            //Menu ve dilIdye göre sayfaları getirme
            //var items = List(p => p.SistemMenuTanimId == menuId && p.DilID == dilId && p.GorunsunMu && p.AktifMi == 1 && p.SilindiMi == 0).Value.ToList();
            //items = items.OrderBy(x => x.SiraNo).ToList();
            //var result = new List<MenuListResponse>();
            //foreach (var item in items)
            //{
            //    var entity = new MenuListResponse()
            //    {
            //        Id = item.TabloID,
            //        UstId = item.UstId.Value,
            //        Tanim = item.MenuBasligi,
            //    };
            //    if (item.MenuBaglantiliSistemSayfaId != null)
            //    {
            //        var sayfa = _sistemSayfalariService.SingleOrDefault(item.MenuBaglantiliSistemSayfaId.Value);
            //        if (sayfa?.Value != null)
            //        {
            //            entity.Url = sayfa.Value.SayfaUrl;
            //            entity.Identity = sayfa.Value.SayfaTanimi;

            //            var baslik = _sistemMenuTanimlariAyrintilarDillerService.List(x => x.ParamDilId == dilId && x.SistemMenuTanimAyrintiId == item.TabloID).Value;
            //            if (baslik != null && baslik.Count > 0)
            //            {
            //                entity.Tanim = baslik.FirstOrDefault().Tanim;
            //            } 
            //            if (sayfa.Value.AktifMi == 1 && sayfa.Value.SilindiMi == 0)
            //            {
            //                result.Add(entity);
            //            }
            //        }
            //    }

            //}
            //o(n) çalışma zamanından kurtulmak için optimize edildi. Sorun çıkarmaz ise yukarıdaki yorum satırı kaldırılacak.
            var items = List(p => p.SistemMenuTanimId == menuId && p.DilID == dilId && p.GorunsunMu && p.AktifMi == 1 && p.SilindiMi == 0).Value.OrderBy(x => x.SiraNo).ToList();

            if (!items.Any())
                return new List<MenuListResponse>().ToResult();

            // Sayfalar
            var sayfaIds = items.Where(i => i.MenuBaglantiliSistemSayfaId.HasValue).Select(i => i.MenuBaglantiliSistemSayfaId.Value).ToList();

            var sayfalar = _sistemSayfalariService.List(s => sayfaIds.Contains(s.TabloID)).Value.Where(s => s.AktifMi == 1 && s.SilindiMi == 0).ToList();

            //Başlıklar
            var itemIds = items.Select(i => i.TabloID).ToList();

            var basliklar = _sistemMenuTanimlariAyrintilarDillerService.List(x => x.ParamDilId == dilId && itemIds.Contains(x.SistemMenuTanimAyrintiId)).Value.ToList();

            //tabloyu birleştir
            var result = (from item in items
                          join sayfa in sayfalar on item.MenuBaglantiliSistemSayfaId equals sayfa.TabloID into s
                          from sayfa in s.DefaultIfEmpty()
                          join baslik in basliklar on item.TabloID equals baslik.SistemMenuTanimAyrintiId into b
                          from baslik in b.DefaultIfEmpty()
                          where sayfa != null
                          select new MenuListResponse
                          {
                              Id = item.TabloID,
                              UstId = item.UstId ?? 0,
                              Tanim = baslik?.Tanim ?? item.MenuBasligi,
                              Url = sayfa.SayfaUrl,
                              Identity = sayfa.SayfaTanimi
                          }).ToList();

            return result.ToResult();
        }

        /// <summary>
        /// Yetki merkezi için sayfa verilerini getiren method
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public Result<List<SistemSayfalari>> YetkiIcinSayfaGetir(string dilKodu)
        {
            var dilItem = _paramDillerService.SingleOrDefault(dilKodu);
            var dilId = 1;
            if (dilItem != null && dilItem.Value != null && dilItem.Value.TabloID > 0)
                dilId = dilItem.Value.TabloID;

            var webAltSayfalar = this.List(a => a.SistemMenuTanimId == 1 && a.DilID == dilId /*a.GorunsunMu==true*/).Value;

            var sayfalarList = new List<SistemSayfalari>();
            foreach (var item in webAltSayfalar)
            {
                var sayfaId = Convert.ToInt32(item.MenuBaglantiliSistemSayfaId);
                var sayfaDetay = _sistemSayfalariService.SingleOrDefault(sayfaId).Value;
                sayfaDetay.SayfaTanimi = item.MenuBasligi;

                var baslik = _sistemMenuTanimlariAyrintilarDillerService.List(x => x.ParamDilId == dilId && x.SistemMenuTanimAyrintiId == item.TabloID && x.AktifMi == 1).Value;
                if (baslik != null && baslik.Count > 0)
                {
                    sayfaDetay.SayfaTanimi = baslik.FirstOrDefault().Tanim;
                }



                if (!sayfalarList.Any(a => a.SayfaTanimi == item.MenuBasligi))
                {
                    sayfalarList.Add(sayfaDetay);
                }
            }

            return sayfalarList.ToResult();
        }
    }
}