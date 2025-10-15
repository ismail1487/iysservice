using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Baz.IysServiceApi.Controllers
{    /// <summary>
     /// Takvim kontrolü için gerekli methodların yer aldığı sınıftır.
     /// </summary>
     /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class TakvimController : Controller
    {
        private readonly ITakvimService _takvimService;

        public TakvimController(ITakvimService takvimService)
        {
            _takvimService = takvimService;
        }
        /// <summary>
        /// Takvimdeki kaynak rezerve verilerini getirir.
        /// </summary>
        [Route("KaynakRezerveTakvimVeriGetir")]
        [HttpGet]
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir()
        {
            var response = _takvimService.KaynakRezerveTakvimVeriGetir();
            return response;
        }

        /// <summary>
        /// Takvimdeki kaynak rezerve verilerini getirir.
        /// </summary>
        [Route("KaynakSelectVeriGetir")]
        [HttpPost]
        public Result<List<KaynakTanimlariRezerveVM>> KaynakSelectVeriGetir([FromBody] List<int> id)
        {
            var response = _takvimService.KaynakSelectVeriGetir(id);
            return response;
        }
        /// <summary>
        /// Rezervasyon Kayıt 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EventKaydet")]
        [HttpPost]
        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet([FromBody] KaynakRezervasyonCariDegerlerVM model)
        {
            var result = _takvimService.EventKaydet(model);
            return result;
        }
        /// <summary>
        /// EVent Guncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("EventGuncelle")]
        [HttpPost]
        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle([FromBody] KaynakRezervasyonCariDegerlerVM model)
        {
            var result = _takvimService.EventGuncelle(model);
            return result;
        }
        /// <summary>
        /// Event Listeleme
        /// </summary>
        /// <returns></returns>
        [Route("EventListele")]
        [HttpGet]
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele()
        {
            var result = _takvimService.EventListele();
            return result;
        }
        /// <summary>
        /// Event Veri DOldurma
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("EventVeriGetir/{id}")]
        [HttpGet]
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id)
        {
            var result = _takvimService.EventVeriGetir(id);
            return result;
        }

        [Route("EventSil/{id}")]
        [HttpPost]
        public Result<bool> EventSil(int id)
        {
            var result = _takvimService.EventSil(id);
            return result;
        }

    }
}
