using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Menü kontrolü için gerekli methodların bulunduğu sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly Service.ISistemMenuTanimlariAyrintilarService _sistemMenuTanimlariAyrintilarService;

        /// <summary>
        /// Menü kontrolü için gerekli methodların bulunduğu sınıfın yapıcı methodudur
        /// </summary>
        /// <param name="sistemMenuTanimlariAyrintilarService"></param>
        public MenuController(Service.ISistemMenuTanimlariAyrintilarService sistemMenuTanimlariAyrintilarService)
        {
            _sistemMenuTanimlariAyrintilarService = sistemMenuTanimlariAyrintilarService;
        }

        /// <summary>
        /// Menü getirme methodu.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("List")]
        public Result<List<MenuListResponse>> GetMenu([FromBody] MenuListRequest request)
        {
            return _sistemMenuTanimlariAyrintilarService.List(request);
        }
        /// <summary>
        /// Login olmunmasa bile Ziyaretci İçin menu dönen metot.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ZiyaretciMenuList")]
        [AllowAnonymous]
        public Result<List<MenuListResponse>> ZiyaretciMenuList([FromBody] MenuListRequest request)
        {
            return _sistemMenuTanimlariAyrintilarService.List(request);
        }

        /// <summary>
        /// Yetki merkezi için sayfa verilerini getiren method
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("YetkiIcinSayfaGetir/{dilKodu}")]
        public Result<List<SistemSayfalari>> YetkiIcinSayfaGetir(string dilKodu)
        {
            var result = _sistemMenuTanimlariAyrintilarService.YetkiIcinSayfaGetir(dilKodu);
            return result;
        }
    }
}