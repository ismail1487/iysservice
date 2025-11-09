using System.Collections.Generic;
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
        public Result<string> MalzemeTalepEt([FromBody] MalzemeTalepEtRequest request)
        {
            if (request == null || request.TalepItems == null || request.TalepItems.Count == 0)
            {
                return "Lütfen en az bir malzeme seçiniz.".ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeTalepEt(request);
        }

        /// <summary>
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için method
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Hazırlanacak malzeme talep ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        [Route("MalzemeleriHazirla/{malzemeTalepSurecTakipID}")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public Result<bool> MalzemeleriHazirla(int malzemeTalepSurecTakipID)
        {
            if (malzemeTalepSurecTakipID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeleriHazirla(malzemeTalepSurecTakipID);
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
    }
}