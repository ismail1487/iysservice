using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Coğrafya Tanım API methodularının bulunduğu sınıf
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CografyaTanimController : ControllerBase
    {
        private readonly ICografyaTanimService _cografyaTanimService;

        /// <summary>
        /// Coğrafya Tanım API methodularının bulunduğu classının yapıcı methodu
        /// </summary>
        /// <param name="cografyaTanimService"></param>
        public CografyaTanimController(ICografyaTanimService cografyaTanimService)
        {
            _cografyaTanimService = cografyaTanimService;
        }

        /// <summary>
        /// Coğrafya Tanım kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CografyaTanimKayit")]
        public Result<int> CografyaTanimKayit([FromBody] CografyaListViewModel model)
        {
            var result = _cografyaTanimService.CografyaTanimKayit(model);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım Listeleme
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CografyaTanimListeleme")]
        public Result<List<CografyaListViewModel>> CografyaTanimListeleme()
        {
            var result = _cografyaTanimService.CografyaTanimListesiGetir();
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım id'sine göre getirme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CografyaTanimIdyeGoreGetir/{cografyaKutuphanesiId}")]
        public Result<CografyaListViewModel> CografyaTanimIdyeGoreGetir(int cografyaKutuphanesiId)
        {
            var result = _cografyaTanimService.CografyaTanimIdyeGoreGetir(cografyaKutuphanesiId);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım Silme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CografyaTanimSil/{cografyaKutuphanesiId}")]
        public Result<bool> CografyaTanimSil(int cografyaKutuphanesiId)
        {
            var result = _cografyaTanimService.CografyaTanimSil(cografyaKutuphanesiId);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CografyaTanimGuncelle")]
        public Result<CografyaKutuphanesi> CografyaTanimGuncelle([FromBody] CografyaListViewModel model)
        {
            var result = _cografyaTanimService.CografyaTanimGuncelle(model);
            return result;
        }
    }
}