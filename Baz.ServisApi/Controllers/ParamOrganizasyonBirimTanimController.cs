using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Param Organizasyon Birim Tanımlarının kontrol methodlarının yer aldığı sınıftır.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ParamOrganizasyonBirimTanimController : ControllerBase
    {
        private readonly IParamOrganizasyonBirimleriService _service;

        /// <summary>
        /// Param Organizasyon Birim Tanımlarının kontrol methodlarının yer aldığı sınıfın yapıcı methodudur.
        /// </summary>
        /// <param name="service"></param>
        public ParamOrganizasyonBirimTanimController(IParamOrganizasyonBirimleriService service)
        {
            _service = service;
        }

        /// <summary>
        /// Birim tanımlarını listeleyen method
        /// </summary>
        /// <returns></returns>
        [Route("ListForView")]
        [HttpGet]
        public Result<List<ParamBirimTanimView>> LitForView()
        {
            return _service.ListForView();
        }

        /// <summary>
        /// name parametresine göre birim tip id döndüren method
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("GetTipId/{name}")]
        public Result<int> GetTipId(string name)
        {
            return _service.GetTipId(name);
        }
    }
}