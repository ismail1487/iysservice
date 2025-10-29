using System.Collections.Generic;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Proje genel bilgileri controller'ı
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjeGenelBilgilerController : ControllerBase
    {
        private readonly IProjeGenelBilgilerService _projeGenelBilgilerService;

        /// <summary>
        /// ProjeGenelBilgilerController yapıcı metodu
        /// </summary>
        /// <param name="projeGenelBilgilerService"></param>
        public ProjeGenelBilgilerController(IProjeGenelBilgilerService projeGenelBilgilerService)
        {
            _projeGenelBilgilerService = projeGenelBilgilerService;
        }

                /// <summary>
        /// Cinsiyetin listelendiği method
        /// </summary>
        /// <returns></returns>
        [Route("ProjeGenelBilgilerList")]
        [HttpGet]
        public Result<List<ProjeGenelBilgiler>> CinsiyetList()
        {
            return _projeGenelBilgilerService.List();
        }
    }
}
