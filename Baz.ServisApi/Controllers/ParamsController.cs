using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Castle.DynamicProxy.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Baz.Repository;
using Baz.Repository.Pattern;
using Microsoft.EntityFrameworkCore;

namespace Baz.IysServiceApi.Controllers
{

    public class Hakan
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Yas { get; set; }
    }

    ///// <summary>
    ///// Menü kontrolü için gerekli methodların bulunduğu sınıftır.
    ///// </summary>
    ///// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class ParamsController : Controller
    {
        [HttpPost]
        [Route("List")]
        public Result<List<string>> GetMenu()
        {

            ControllerContext.GetType().GetProperties();

            var sonuc = new List<string>();
            sonuc.Add("test1");
            sonuc.Add("test1");

            var aa = new Hakan();          
            var bb = aa.GetType().GetProperties();





            /*
            var query = user.GetType()
                .GetProperties()
                .Select(p => p.GetValue(user))
                .Select(o => Object.ReferenceEquals(o, null)
                          ? default(string)
                          : o.ToString()
                       ); */


            return  sonuc.ToResult();
            //return _sistemMenuTanimlariAyrintilarService.List(request);
        }
    }



    //public class MenuController : ControllerBase
    //{
    //    private readonly Service.ISistemMenuTanimlariAyrintilarService _sistemMenuTanimlariAyrintilarService;

    //    /// <summary>
    //    /// Menü kontrolü için gerekli methodların bulunduğu sınıfın yapıcı methodudur
    //    /// </summary>
    //    /// <param name="sistemMenuTanimlariAyrintilarService"></param>
    //    public MenuController(Service.ISistemMenuTanimlariAyrintilarService sistemMenuTanimlariAyrintilarService)
    //    {
    //        _sistemMenuTanimlariAyrintilarService = sistemMenuTanimlariAyrintilarService;
    //    }

    //    /// <summary>
    //    /// Menü getirme methodu.
    //    /// </summary>
    //    /// <param name="request">Request.</param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [Route("List")]
    //    public Result<List<MenuListResponse>> GetMenu([FromBody] MenuListRequest request)
    //    {
    //        return _sistemMenuTanimlariAyrintilarService.List(request);
    //    }

    //    /// <summary>
    //    /// Yetki merkezi için sayfa verilerini getiren method
    //    /// </summary>
    //    /// <param></param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("YetkiIcinSayfaGetir/{dilKodu}")]
    //    public Result<List<SistemSayfalari>> YetkiIcinSayfaGetir(string dilKodu)
    //    {
    //        var result = _sistemMenuTanimlariAyrintilarService.YetkiIcinSayfaGetir(dilKodu);
    //        return result;
    //    }
    //}
}