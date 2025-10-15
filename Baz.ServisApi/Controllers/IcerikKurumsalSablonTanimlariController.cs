using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// İçerik kurumsal şablon tanımları için Controller metodlarının yer aldığı sınıftır.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IcerikKurumsalSablonTanimlariController : Controller
    {
        private readonly IIcerikKurumsalSablonTanimlariService _icerikKurumsalSablonTanimlariService;

        /// <summary>
        /// İçerik kurumsal şablon tanımları için Controller metodlarının yer aldığı sınıfının yapıcı metodu
        /// </summary>
        /// <param name="icerikKurumsalSablonTanimlariService"></param>
        public IcerikKurumsalSablonTanimlariController(IIcerikKurumsalSablonTanimlariService icerikKurumsalSablonTanimlariService)
        {
            _icerikKurumsalSablonTanimlariService = icerikKurumsalSablonTanimlariService;
        }

        /// <summary>
        /// İçerik kurumsal şablonlar ekleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        public Result<IcerikKurumsalSablonTanimlari> Add(IcerikKurumsalSablonTanimlari item)
        {
            var result = _icerikKurumsalSablonTanimlariService.Add(item);
            return result;
        }

        /// <summary>
        /// İçerik kurumsal şablonlar güncelleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPost]
        public Result<IcerikKurumsalSablonTanimlari> Update(IcerikKurumsalSablonTanimlari item)
        {
            var result = _icerikKurumsalSablonTanimlariService.Update(item);

            return result;
        }

        /// <summary>
        /// İçerik kurumsal şablonları silindi yapan metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Delete/{id}")]
        [HttpGet]
        public Result<IcerikKurumsalSablonTanimlari> Delete(int id)
        {
            var result = _icerikKurumsalSablonTanimlariService.Delete(id);

            return result;
        }

        /// <summary>
        /// ıd ile içerik kurumsal şablonları getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Get/{id}")]
        [HttpGet]
        public Result<IcerikKurumsalSablonTanimlari> Get(int id)
        {
            var result = new IcerikKurumsalSablonTanimlari();
            result = _icerikKurumsalSablonTanimlariService.SingleOrDefault(id).Value;
            return result.ToResult();
        }

        /// <summary>
        /// kurum Id ile içerik kurumsal şablonları listesi getiren metod
        /// </summary>
        /// <param name="kurumId"></param>
        /// <returns>listeyi döndürür.</returns>
        [Route("ListForKurum/{kurumId}")]
        [HttpGet]
        public Result<List<IcerikKurumsalSablonTanimlari>> ListForKurum(int kurumId)
        {
            var result = new List<IcerikKurumsalSablonTanimlari>();
            result = _icerikKurumsalSablonTanimlariService.List(a => a.KurumID == kurumId && a.AktifMi == 1 && a.SilindiMi == 0).Value;
            return result.ToResult();
        }

        /// <summary>
        /// içerik kurumsal sistem şablonları listesi getiren metod
        /// </summary>
        /// <param></param>
        /// <returns>listeyi döndürür.</returns>
        [Route("ListForSistem")]
        [HttpGet]
        public Result<List<IcerikKurumsalSablonTanimlari>> ListForSistem()
        {
            var result = new List<IcerikKurumsalSablonTanimlari>();
            result = _icerikKurumsalSablonTanimlariService.List(a => a.SistemMi == true && a.AktifMi == 1 && a.SilindiMi == 0).Value;
            return result.ToResult();
        }

        /// <summary>
        /// Id ile ilgili içerik kurumsal şablon tanımını silindi durumuna getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns>silinen veriyi döndürür.</returns>
        [Route("SetDeleted/{id}")]
        [HttpGet]
        public Result<IcerikKurumsalSablonTanimlari> SetDeleted(int id)
        {
            var result = new IcerikKurumsalSablonTanimlari();
            result = _icerikKurumsalSablonTanimlariService.SetDeleted(id).Value;
            return result.ToResult();
        }
    }
}