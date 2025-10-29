using Baz.AletKutusu;
using Baz.AOP.Logger.ExceptionLog;
using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Genel parametre kontrol methodlarının bulunduğu sınıftır.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GenelParametrelerController : ControllerBase
    {
        private readonly IParamCinsiyetService _paramCinsiyetService;
        private readonly IParamUlkelerSehirlerService _paramUlkelerSehirlerService;
        private readonly IParamBankalarSubelerService _paramBankalarSubelerService;
        private readonly IParamMedeniHalService _paramMedeniHalService;
        private readonly IParamAdresTipiService _paramAdresTipleriService;
        private readonly IParamOkulTipiService _paramOkulTipiService;
        private readonly IParamTelefonTipiService _paramTelefonTipiService;
        private readonly IParamDinlerService _paramDinlerService;
        private readonly IParamCalisanSayilariService _paramCalisanSayilariService;
        private readonly IParamKurumLokasyonTipiService _paramKurumLokasyonTipiService;
        private readonly IParamPostaciIslemDurumTipleriService _paramPostaciIslemDurumTipleriService;
        private readonly IParamIliskiTurleriService _paramIliskiTurleriService;
        private readonly IKisiService _kisiService;
        private readonly IParamTalepSurecStatuleriService _paramTalepSurecStatuleriService;

        private readonly List<Type> _paramTypes;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Genel Parametler api contoller sınıfının yapıcı methodudur.
        /// </summary>
        /// <param name="kisiService"></param>
        /// <param name="paramCinsiyetService"></param>
        /// <param name="paramUlkelerSehirlerService"></param>
        /// <param name="paramMedeniHalService"></param>
        /// <param name="paramAdresTipleriService"></param>
        /// <param name="paramOkulTipiService"></param>
        /// <param name="paramTelefonTipiService"></param>
        /// <param name="paramYabanciDillerService"></param>
        /// <param name="paramBankalarSubelerService"></param>
        /// <param name="paramDinlerService"></param>
        /// <param name="paramCalisanSayilariService"></param>
        /// <param name="paramKurumLokasyonTipiService"></param>
        /// <param name="paramVergiDaireleriService"></param>
        /// <param name="paramPostaciIslemDurumTipleriService"></param>
        /// <param name="paramIliskiTurleriService"></param>
        /// <param name="ulkeMaskeleriService"></param>
        /// <param name="serviceProvider"></param>
        public GenelParametrelerController(IKisiService kisiService, IParamCinsiyetService paramCinsiyetService, IParamUlkelerSehirlerService paramUlkelerSehirlerService, IParamMedeniHalService paramMedeniHalService, IParamAdresTipiService paramAdresTipleriService, IParamOkulTipiService paramOkulTipiService, IParamTelefonTipiService paramTelefonTipiService, IParamBankalarSubelerService paramBankalarSubelerService, IParamDinlerService paramDinlerService, IParamCalisanSayilariService paramCalisanSayilariService, IParamKurumLokasyonTipiService paramKurumLokasyonTipiService,  IParamPostaciIslemDurumTipleriService paramPostaciIslemDurumTipleriService, IParamIliskiTurleriService paramIliskiTurleriService,  IServiceProvider serviceProvider, IParamTalepSurecStatuleriService paramTalepSurecStatuleriService)
        {
            _paramCinsiyetService = paramCinsiyetService;
            _paramUlkelerSehirlerService = paramUlkelerSehirlerService;
            _paramMedeniHalService = paramMedeniHalService;
            _paramAdresTipleriService = paramAdresTipleriService;
            _paramOkulTipiService = paramOkulTipiService;
            _paramTelefonTipiService = paramTelefonTipiService;
            _paramDinlerService = paramDinlerService;
            _paramCalisanSayilariService = paramCalisanSayilariService;
            _paramKurumLokasyonTipiService = paramKurumLokasyonTipiService;
            _paramPostaciIslemDurumTipleriService = paramPostaciIslemDurumTipleriService;
            _paramIliskiTurleriService = paramIliskiTurleriService;
            _paramBankalarSubelerService = paramBankalarSubelerService;
            _kisiService = kisiService;
            _paramTalepSurecStatuleriService = paramTalepSurecStatuleriService;
       
            _serviceProvider = serviceProvider;
            _paramTypes = System.Reflection.Assembly.GetAssembly(typeof(Baz.Service.IParamDillerService)).GetTypes().Where(p => p.IsInterface).ToList();
        }

       

       

        /// <summary>
        /// Cinsiyetin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("CinsiyetList")]
        [HttpGet]
        public Result<List<ParamCinsiyet>> CinsiyetList()
        {
            return _paramCinsiyetService.List();
        }

        /// <summary>
        /// Ülkelerin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("UlkeList")]
        [HttpGet]
        public Result<List<ParamUlkeler>> UlkeList()
        {
            return _paramUlkelerSehirlerService.UlkeListesi();
        }

        /// <summary>
        /// Şehirlerin listelendiği method
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        [Route("SehirList/{ulkeID}")]
        [HttpGet]
        public Result<List<ParamUlkeler>> SehirList(int ulkeID)
        {
            return _paramUlkelerSehirlerService.SehirListesi(ulkeID);
        }

        /// <summary>
        /// Medeni halin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("MedeniHalList")]
        [HttpGet]
        public Result<List<ParamMedeniHal>> MedeniHalList()
        {
            return _paramMedeniHalService.List();
        }

        /// <summary>
        /// Adres tiplerinin listelendiği method
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "AdresTipiList")]
        [Route("AdresTipiList")]
        [HttpGet]
        public Result<List<ParamAdresTipi>> AdresTipiList()
        {
            return _paramAdresTipleriService.List();
        }

        /// <summary>
        /// Okul tiplerinin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("OkulTipiList")]
        [HttpGet]
        public Result<List<ParamOkulTipi>> OkulTipiList()
        {
            return _paramOkulTipiService.List();
        }

        /// <summary>
        /// Telefon tiplerinin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("TelefonTipiList")]
        [HttpGet]
        public Result<List<ParamTelefonTipi>> TelefonTipiList()
        {
            return _paramTelefonTipiService.List();
        }

        /// <summary>
        /// Dinlerin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("DinlerList")]
        [HttpGet]
        public Result<List<ParamDinler>> DinlerList()
        {
            return _paramDinlerService.List();
        }

        /// <summary>
        /// Çalışan sayılarının listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("CalisanSayilariList")]
        [HttpGet]
        public Result<List<ParamCalisanSayilari>> CalisanSayilariList()
        {
            return _paramCalisanSayilariService.List();
        }

        /// <summary>
        /// Çalışan sayılarının listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("TalepSurecStatuleriList")]
        [HttpGet]
        public Result<List<ParamTalepSurecStatuleri>> TalepSurecStatuleriList()
        {
            return _paramTalepSurecStatuleriService.List();
        }

        /// <summary>
        /// Kurum lokasyon tiplerinin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("KurumLokasyonTipiList")]
        [HttpGet]
        public Result<List<ParamKurumLokasyonTipi>> KurumLokasyonTipiList()
        {
            return _paramKurumLokasyonTipiService.List();
        }

       

        /// <summary>
        /// Postacı işlem durum tiplerinin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("PostaciIslemDurumTipleriList")]
        [HttpGet]
        public Result<List<ParamPostaciIslemDurumTipleri>> PostaciIslemDurumTipleriList()
        {
            return _paramPostaciIslemDurumTipleriService.List();
        }

        /// <summary>
        /// Ülke Şehir, lokasyan tipi ve vergi daireleri parametrelerinin getirildiği metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "GetParametreNames")]
        [Route("GetParametreNames")]
        [HttpPost]
        public Result<KurumTemelKayitModel> GetParametreNames(KurumTemelKayitModel model)
        {
            if (model == null) return model.ToResult();
            var kurumUlke = _paramUlkelerSehirlerService.List(p => p.TabloID == model.KurumUlkeId).Value;
            var kurumSehir = _paramUlkelerSehirlerService.List(p => p.TabloID == model.KurumSehirId).Value;
            if (kurumUlke != null && kurumUlke.FirstOrDefault() != null)
                model.KurumUlkeAdi = kurumUlke.FirstOrDefault()?.ParamTanim;

            if (kurumSehir != null && kurumSehir.FirstOrDefault() != null)
                model.KurumSehirAdi = kurumSehir.FirstOrDefault()?.ParamTanim;

            if (model.AdresListesi != null)
            {
                foreach (var item in model.AdresListesi)
                {
                    var ulke = _paramUlkelerSehirlerService.List(p => p.TabloID == item.Ulke).Value;
                    var sehir = _paramUlkelerSehirlerService.List(p => p.TabloID == item.Sehir).Value;
                    var lokasyon = _paramKurumLokasyonTipiService.List(p => p.TabloID == item.LokasyonTipi).Value;
                    if (ulke != null && ulke.FirstOrDefault() != null)
                        item.UlkeAdi = ulke.FirstOrDefault()?.ParamTanim;
                    else
                        item.UlkeAdi = "";
                    if (sehir != null && sehir.FirstOrDefault() != null)
                        item.SehirAdi = sehir.FirstOrDefault()?.ParamTanim;
                    if (lokasyon != null && lokasyon.FirstOrDefault() != null)
                        item.LokasyonAdi = lokasyon.FirstOrDefault()?.ParamTanim;
                }
            }

            if (model.BankaListesi == null) return model.ToResult();
            foreach (var item in model.BankaListesi)
            {
                var banka = _paramBankalarSubelerService.List(p => p.TabloID == item.BankaId).Value;
                var sube = _paramBankalarSubelerService.List(p => p.TabloID == item.SubeId).Value;
                if (banka != null && banka.FirstOrDefault() != null)
                    item.BankaAdi = banka.FirstOrDefault()?.ParamTanim;
                else
                    item.BankaAdi = "";
                if (sube != null && sube.FirstOrDefault() != null)
                    item.SubeAdi = sube.FirstOrDefault()?.ParamTanim;
            }
            return model.ToResult();
        }

        /// <summary>
        /// Kurum İdari parametre isimlerini getiren metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "GetIdariParametreNames")]
        [Route("GetIdariParametreNames")]
        [HttpPost]
        public Result<KurumIdariProfilModel> GetIdariParametreNames(KurumIdariProfilModel model)
        {
            if (model != null)
            {
                foreach (var item in model.KurumKisiLisansListesi)
                {
                    var kisi = _kisiService.SingleOrDefault(Convert.ToInt32(item.LisansAboneKisiId)).Value;
                    if (kisi != null)
                    {
                        item.LisansAboneKisiAdi = kisi.KisiAdi + " " + kisi.KisiSoyadi;
                        item.LisansAboneKisiMail = kisi.KisiEposta;
                        //item.Name = _kurumLisansService.SingleOrDefault(Convert.ToInt32(item.LisansGenelTanimId)).Value
                        //    .Name;
                    }
                }

                foreach (var item in model.KurumBankaListesi)
                {
                    var banka = _paramBankalarSubelerService.List(p => p.TabloID == item.BankaId).Value;
                    var sube = _paramBankalarSubelerService.List(p => p.TabloID == item.SubeId).Value;
                    if (banka != null && banka.FirstOrDefault() != null)
                        item.BankaAdi = banka.FirstOrDefault()?.ParamTanim;
                    else
                        item.BankaAdi = "";
                    if (sube != null && sube.FirstOrDefault() != null)
                        item.SubeAdi = sube.FirstOrDefault()?.ParamTanim;
                }
            }
            return model.ToResult();
        }

        /// <summary>
        /// Koda göre ilişki türlerini listeleyen metot
        /// </summary>
        /// <param name="paramKod"></param>
        /// <returns></returns>
        [ProcessName(Name = "Koda göre ilişki türlerini listeleme")]
        [Route("KodaGoreIliskiTurleri/{paramKod}")]
        [HttpGet]
        public Result<List<ParamIliskiTurleri>> KodaGoreIliskiTurleri(string paramKod)
        {
            var result = _paramIliskiTurleriService.KodaGoreIliskiTurleri(paramKod);
            return result;
        }


        /// <summary>
        /// Param tanım ve dil Id'ye göre parametreleri listeleme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "List")]
        [Route("List")]
        [HttpPost]
        public Result<List<Baz.Model.Entity.ViewModel.ParametreResult>> List(Model.Entity.ViewModel.ParametreRequest request)
        {
            List<Baz.Model.Entity.ViewModel.ParametreResult> result = new();
            //Expression<Func<ParamZamanDilimleri, bool>> expression = p => p.DilID == request.DilID && p.UstId == request.UstId; //p.KurumID == request.KurumId; 19.11.2020 Behiç abi kurum filtresinin kalkmasını istedi.
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var method = service.GetType().GetMethod("ListForFunc");
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            
            
            //var newExpression = expression.ConvertImpl(parametreType);
            //var serviceResult = method2.Invoke(service, new[] { newExpression });
            //if (serviceResult != null)
            //{
            //    var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
            //    if (value != null)
            //    {
            //        var ae = (value as IEnumerable).GetEnumerator();
            //        while (ae.MoveNext())
            //        {
            //            var item = new ParametreResult();
            //            item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
            //            item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("ParamTanim"));
            //            item.GetType().GetProperty("UstId").SetValue(item, ae.Current._GetProperty("UstId"));
            //            item.GetType().GetProperty("DilID").SetValue(item, ae.Current._GetProperty("DilID"));
            //            result.Add(item);
            //        }
            //    }
            //}
            return result.ToResult();
        }
    }
}