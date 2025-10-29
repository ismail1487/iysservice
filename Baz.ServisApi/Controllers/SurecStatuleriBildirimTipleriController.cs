using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// SurecStatuleriBildirimTipleri ile ilgili işlemleri yöneten controller sınıfı
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SurecStatuleriBildirimTipleriController : ControllerBase
    {
        private readonly Service.ISurecStatuleriBildirimTipleriService _surecStatuleriBildirimTipleriService;

        /// <summary>
        /// SurecStatuleriBildirimTipleriController yapıcı metodu
        /// </summary>
        /// <param name="surecStatuleriBildirimTipleriService"></param>
        public SurecStatuleriBildirimTipleriController(Service.ISurecStatuleriBildirimTipleriService surecStatuleriBildirimTipleriService)
        {
            _surecStatuleriBildirimTipleriService = surecStatuleriBildirimTipleriService;
        }

        /// <summary>
        /// SurecStatuleriBildirimTipleri listesini döner
        /// </summary>
        /// <returns></returns>
        [HttpGet("SurecStatuleriBildirimTipleriList/{paramTalepSurecStatuID}")]
        public IActionResult SurecStatuleriBildirimTipleriList(int paramTalepSurecStatuID)   
        {
            var result = _surecStatuleriBildirimTipleriService.List(s => s.ParamTalepSurecStatuID == paramTalepSurecStatuID);
            return Ok(result);
        }
    }
}
