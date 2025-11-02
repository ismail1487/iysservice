using System.Collections.Generic;
using Baz.Model.Entity;
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
        /// Malzeme taleplerini listeleyen method (koşullu filtreleme ile)
        /// </summary>
        /// <param name="malzemeTalepEtGetir">True ise: [1,2] statüleri veya kalan miktar > 0 olanları getirir</param>
        /// <param name="talepSurecStatuIDs">Talep süreç statü ID'leri (malzemeTalepEtGetir=false iken kullanılır)</param>
        /// <returns>Filtrelenmiş malzeme talep listesi</returns>
        [Route("MalzemeTalepList")]
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<MalzemeTalepGenelBilgiler>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<List<MalzemeTalepGenelBilgiler>> MalzemeTalepList([FromQuery] bool malzemeTalepEtGetir = false, [FromQuery] List<int> talepSurecStatuIDs = null)
        {
            return _malzemeTalepGenelBilgilerService.MalzemeTalepList(malzemeTalepEtGetir, talepSurecStatuIDs);
        }

        /// <summary>
        /// Malzeme talebinde bulunma method
        /// </summary>
        /// <param name="request">Talep parametreleri</param>
        /// <returns>Talep işlemi sonucu</returns>
        [Route("MalzemeTalepEt")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> MalzemeTalepEt([FromBody] MalzemeTalepEtRequest request)
        {
            if (request == null)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeTalepEt(request);
        }

        /// <summary>
        /// Malzemeleri hazırlamak ve statüsünü güncellemek için method
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Hazırlanacak malzeme talep ID'si</param>
        /// <returns>Hazırlama işlemi sonucu</returns>
        [Route("MalzemeleriHazirla/{malzemeTalebiEssizID}")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> MalzemeleriHazirla(int malzemeTalebiEssizID)
        {
            if (malzemeTalebiEssizID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeleriHazirla(malzemeTalebiEssizID);
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
            if (request == null || request.MalzemeTalebiEssizID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalzemeIadeEt(request);
        }

        /// <summary>
        /// Mal kabul etme method
        /// </summary>
        /// <param name="malzemeTalebiEssizID">Kabul edilecek malzeme talep ID'si</param>
        /// <returns>Kabul işlemi sonucu</returns>
        [Route("MalKabulEt/{malzemeTalebiEssizID}")]
        [HttpPost]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Result<bool> MalKabulEt(int malzemeTalebiEssizID)
        {
            if (malzemeTalebiEssizID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.MalKabulEt(malzemeTalebiEssizID);
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
            if (request == null || request.MalzemeTalebiEssizID <= 0)
            {
                return false.ToResult();
            }

            return _malzemeTalepGenelBilgilerService.HasarliOlarakIsaretle(request);
        }
    }
}