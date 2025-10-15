using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Dil kontrolü için gerekli methodların yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class DilController : ControllerBase
    {
        private readonly IParamDillerService _paramDillerService;
        private readonly IParamYabanciDillerService _paramYabanciDillerService;

        /// <summary>
        ///  Dil kontrolü için gerekli methodların yer aldığı sınıfının yapıcı metodu
        /// </summary>
        /// <param name="paramDillerService"></param>
        public DilController(IParamDillerService paramDillerService, IParamYabanciDillerService paramYabanciDillerService)
        {
            _paramDillerService = paramDillerService;
            _paramYabanciDillerService = paramYabanciDillerService;
        }

        /// <summary>
        /// Dilleri listeleyen method.
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public Result<List<ParamDiller>> List()
        {
            return _paramDillerService.List();
        }
        /// <summary>
        /// Kişinin bildiği yabanci dilleri listeleyen method.
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("YabanciDilList")]
        [HttpGet]
        public Result<List<ParamYabanciDiller>> YabanciDilList()
        {
            return _paramYabanciDillerService.YabanciDilList();
        }

        /// <summary>
        /// Dil Koduna göre Id'i getiren metod
        /// </summary>
        /// <param name="dilKodu"></param>
        /// <returns></returns>
        [Route("Get/{dilKodu}")]
        [HttpGet]
        public int GetDilID(string dilKodu)
        {
            var dilID = _paramDillerService.SingleOrDefault(dilKodu);
            if (dilID.Value == null)
                return 0;
            else
                return dilID.Value.TabloID;
        }

        /// <summary>
        /// Dil Id'ye göre dil kodunu getiren metod
        /// </summary>
        /// <param name="dilID"></param>
        /// <returns></returns>
        [Route("GetParamKod/{dilID}")]
        [HttpGet]
        [AllowAnonymous]
        public Result<string> GetParamKod(int dilID)
        {
            var dilkod = _paramDillerService.ParamKodGetir(dilID);
            return dilkod;
        }
    }


}