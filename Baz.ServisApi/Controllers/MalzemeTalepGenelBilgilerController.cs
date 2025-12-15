using System.Collections.Generic;
using System.Threading.Tasks;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Malzeme Talep Genel Bilgileri controller'ı
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MalzemeTalepGenelBilgilerController : ControllerBase
    {
        private readonly IMalzemeTalepGenelBilgilerService _malzemeTalepGenelBilgilerService;

        /// <summary>
        /// MalzemeTalepGenelBilgilerController yapıcı metodu
        /// </summary>
        /// <param name="malzemeTalepGenelBilgilerService"></param>
        public MalzemeTalepGenelBilgilerController(IMalzemeTalepGenelBilgilerService malzemeTalepGenelBilgilerService)
        {
            _malzemeTalepGenelBilgilerService = malzemeTalepGenelBilgilerService;
        }

        /// <summary>
        /// Malzeme taleplerini filtreli olarak getiren method (detaylı miktar bilgileri ile)
        /// </summary>
        /// <param name="request">Filtre parametreleri</param>
        /// <returns>Filtrelenmiş malzeme talep listesi (detaylı miktar bilgileri ile)</returns>
        [Route("MalzemeTalepleriniGetir")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<List<MalzemeTalepDetayResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<List<MalzemeTalepDetayResponse>> MalzemeTalepleriniGetir([FromBody] MalzemeTalepFiltreleRequest request)
        {
            if (request == null)
            {
                return (new List<MalzemeTalepDetayResponse>()).ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeTalepleriniGetir(request);
        }

        /// <summary>
        /// Malzeme talebinde bulunma method
        /// </summary>
        /// <param name="request">Talep parametreleri</param>
        /// <returns>Talep işlemi sonucu</returns>
        [Route("MalzemeTalepEt")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<Result<string>> MalzemeTalepEt([FromBody] MalzemeTalepEtRequest request)
        {
            if (request == null || request.TalepItems == null || request.TalepItems.Count == 0)
            {
                return Task.FromResult("Lütfen en az bir malzeme seçiniz.".ToResult());
            }

            return _malzemeTalepGenelBilgilerService.MalzemeTalepEt(request);
        }

        /// <summary>
        /// Toplu malzemeleri hazırlamak ve statüsünü güncellemek için method
        /// </summary>
        /// <param name="request">Toplu hazırlama parametreleri</param>
        /// <returns>Hazırlama işlemi sonuç mesajı</returns>
        [Route("TopluMalzemeleriHazirla")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> TopluMalzemeleriHazirla([FromBody] TopluMalzemeleriHazirlaRequest request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                return "Lütfen en az bir malzeme seçiniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.TopluMalzemeleriHazirla(request);
        }

        /// <summary>
        /// Malzeme talebini iade etme method
        /// </summary>
        /// <param name="request">İade parametreleri</param>
        /// <returns>İade işlemi sonucu</returns>
        [Route("MalzemeIadeEt")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> MalzemeIadeEt([FromBody] MalzemeIadeEtRequest request)
        {
            if (request == null || request.MalzemeTalepSurecTakipID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeIadeEt(request);
        }

        /// <summary>
        /// Mal kabul etme method
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        [Route("MalKabulEt/{malzemeTalepSurecTakipID}")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID)
        {
            if (malzemeTalepSurecTakipID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalKabulEt(malzemeTalepSurecTakipID);
        }

        /// <summary>
        /// Toplu mal kabul etme method
        /// </summary>
        /// <param name="request">Toplu kabul parametreleri</param>
        /// <returns>Kabul işlemi sonuç mesajı</returns>
        [Route("TopluMalKabulEt")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> TopluMalKabulEt([FromBody] TopluMalKabulEtRequest request)
        {
            if (request == null || request.MalzemeTalepSurecTakipIDler == null || request.MalzemeTalepSurecTakipIDler.Count == 0)
            {
                return "Lütfen en az bir malzeme seçiniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.TopluMalKabulEt(request);
        }

        /// <summary>
        /// Malzemeyi hasarlı olarak işaretleme method
        /// </summary>
        /// <param name="request">Hasarlı işaretleme parametreleri</param>
        /// <returns>İşaretleme işlemi sonucu</returns>
        [Route("HasarliOlarakIsaretle")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> HasarliOlarakIsaretle([FromBody] HasarliOlarakIşaretleRequest request)
        {
            if (request == null || request.MalzemeTalepSurecTakipID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.HasarliOlarakIsaretle(request);
        }

        /// <summary>
        /// Toplu SAT bilgisi güncelleme method
        /// </summary>
        /// <param name="request">Güncelleme parametreleri</param>
        /// <returns>Güncelleme işlemi sonucu</returns>
        [Route("TopluSATBilgisiGuncelle")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> TopluSATBilgisiGuncelle([FromBody] TopluSATBilgisiGuncellemeRequest request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                return "Lütfen en az bir kayıt giriniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.TopluSATBilgisiGuncelle(request);
        }
        
        /// <summary>
        /// Depo kabul işlemi (Kabul butonu)
        /// </summary>
        /// <param name="request">Kabul parametreleri</param>
        /// <returns>Kabul işlemi sonucu</returns>
        [Route("TopluDepoKabul")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> TopluDepoKabul([FromBody] TopluDepoKararRequest request)
        {
            if (request == null || request.MalzemeTalepSurecTakipIDler == null || request.MalzemeTalepSurecTakipIDler.Count == 0)
            {
                return "Lütfen en az bir kayıt seçiniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.TopluDepoKabul(request);
        }

        /// <summary>
        /// Depo red işlemi (Red butonu)
        /// </summary>
        /// <param name="request">Red parametreleri</param>
        /// <returns>Red işlemi sonucu</returns>
        [Route("TopluDepoRed")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> TopluDepoRed([FromBody] TopluDepoKararRequest request)
        {
            if (request == null || request.MalzemeTalepSurecTakipIDler == null || request.MalzemeTalepSurecTakipIDler.Count == 0)
            {
                return "Lütfen en az bir kayıt seçiniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.TopluDepoRed(request);
        }

        /// <summary>
        /// MalzemeTalepEt işleminin son işlemini geri alma method
        /// Kullanıcının en son yaptığı talep işlemini otomatik bulup geri alır
        /// </summary>
        /// <returns>Geri alma işlemi sonuç mesajı</returns>
        [Route("MalzemeTalepEtSonIslemGeriAl")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> MalzemeTalepEtSonIslemGeriAl()
        {
            return _malzemeTalepGenelBilgilerService.MalzemeTalepEtSonIslemGeriAl();
        }

        /// <summary>
        /// Depo hazırlama işleminin son işlemini geri alma method
        /// Kullanıcının en son yaptığı hazırlama işlemini otomatik bulup geri alır
        /// </summary>
        /// <returns>Geri alma işlemi sonuç mesajı</returns>
        [Route("DepoHazirlamaSonIslemGeriAl")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> DepoHazirlamaSonIslemGeriAl()
        {
            return _malzemeTalepGenelBilgilerService.DepoHazirlamaSonIslemGeriAl();
        }

        /// <summary>
        /// Üretim mal kabul işleminin son işlemini geri alma method
        /// Kullanıcının en son yaptığı mal kabul/iade işlemini otomatik bulup geri alır
        /// </summary>
        /// <returns>Geri alma işlemi sonuç mesajı</returns>
        [Route("UretimMalKabulSonIslemGeriAl")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> UretimMalKabulSonIslemGeriAl()
        {
            return _malzemeTalepGenelBilgilerService.UretimMalKabulSonIslemGeriAl();
        }

        /// <summary>
        /// Kalite kontrol işleminin son işlemini geri alma method
        /// Kullanıcının en son yaptığı kalite onay/hasarlı işlemini otomatik bulup geri alır
        /// </summary>
        /// <returns>Geri alma işlemi sonuç mesajı</returns>
        [Route("KaliteKontrolSonIslemGeriAl")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> KaliteKontrolSonIslemGeriAl()
        {
            return _malzemeTalepGenelBilgilerService.KaliteKontrolSonIslemGeriAl();
        }

        /// <summary>
        /// Üretim İade Depo Karar işleminin son işlemini geri alma endpoint'i
        /// Kullanıcının en son yaptığı depo kabul/red işlemini otomatik bulup geri alır
        /// </summary>
        /// <returns>Geri alma işlemi sonuç mesajı</returns>
        [Route("DepoKararSonIslemGeriAl")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<string> DepoKararSonIslemGeriAl()
        {
            return _malzemeTalepGenelBilgilerService.DepoKararSonIslemGeriAl();
        }
    }
}