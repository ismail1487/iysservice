using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Baz.Model.Pattern;

namespace Baz.Service
{
    /// <summary>
    /// Coğrafya Ayrıntı İşlemlerinin yapıldığı interface
    /// </summary>
    public interface ICografyaAyrintilarService : IService<CografyaKutuphanesiAyrintilar>
    {
        /// <summary>
        /// Coğrafya Ayrıntı kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<CografyaListViewModel> CografyaAyrintilarKayit(CografyaListViewModel model);

        /// <summary>
        /// Coğrafya Ayrıntı Listeleme
        /// </summary>
        /// <returns></returns>
        Result<List<CografyaKutuphanesiAyrintilar>> CografyaTanimAyrintiListeleme();

        /// <summary>
        /// Coğrafya Ayrıntı Silme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<bool> CografyaAyrintilarSil(int cografyaKutuphanesiId);

        /// <summary>
        /// Coğrafya Ayrıntı getirme gelen id'ye göre
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<List<CografyaKutuphanesiAyrintilar>> CografyaAyrintilarGetirIdsineGore(int cografyaKutuphanesiId);

        /// <summary>
        /// Coğrafya Ayrıntı Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<CografyaListViewModel> CografyaAyrintilarGuncelle(CografyaListViewModel model);
    }

    /// <summary>
    /// Coğrafya Ayrıntı İşlemlerinin yapıldığı service
    /// </summary>
    public class CografyaAyrintilarService : Service<CografyaKutuphanesiAyrintilar>, ICografyaAyrintilarService
    {
        private readonly ILoginUser _loginUser;

        /// <summary>
        /// CografyaKutuphanesiAyrintilar ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="loginUser"></param>
        public CografyaAyrintilarService(IRepository<CografyaKutuphanesiAyrintilar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<CografyaAyrintilarService> logger, ILoginUser loginUser) : base(repository, dataMapper, serviceProvider, logger)
        {
            _loginUser = loginUser;
        }

        /// <summary>
        /// Coğrafya Ayrıntı kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<CografyaListViewModel> CografyaAyrintilarKayit(CografyaListViewModel model)
        {
            foreach (var sehirId in model.SehirlerIDList)
            {
                var cografyaAyrinti = new CografyaKutuphanesiAyrintilar
                {
                    AktifMi = 1,
                    CografyaAlaninaDahilEdilmeZamani = DateTime.Now,
                    CografyaKutuphanesiId = model.CografyaKutupanesiId,
                    GuncellenmeTarihi = DateTime.Now,
                    UlkelerId = sehirId,
                    KayitTarihi = DateTime.Now,
                    SilindiMi = 0,
                    IlgiliUlkeID = model.UlkeId,
                    KayitEdenID = model.KisiId,
                    KisiID = _loginUser.KisiID,
                    KurumID = _loginUser.KurumID,
                };
                this.Add(cografyaAyrinti);
            }
            return model.ToResult();
        }

        /// <summary>
        /// Coğrafya Ayrıntı Listeleme
        /// </summary>
        /// <returns></returns>s
        public Result<List<CografyaKutuphanesiAyrintilar>> CografyaTanimAyrintiListeleme()
        {
            var res =  List(x => x.AktifMi == 1 && x.SilindiMi == 0);
            return res;
        }

        /// <summary>
        /// Coğrafya Ayrıntı Silme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<bool> CografyaAyrintilarSil(int cografyaKutuphanesiId)
        {
            var cografyaAyrintilar = this.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.CografyaKutuphanesiId == cografyaKutuphanesiId).Value;

            foreach (var cografyaAyrinti in cografyaAyrintilar)
            {
                cografyaAyrinti.AktifMi = 0;
                cografyaAyrinti.SilindiMi = 1;
                cografyaAyrinti.SilinmeTarihi = DateTime.Now;
                this.Update(cografyaAyrinti);
            }
            return true.ToResult();
        }

        /// <summary>
        /// Coğrafya Ayrıntı getirme gelen id'ye göre
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<List<CografyaKutuphanesiAyrintilar>> CografyaAyrintilarGetirIdsineGore(int cografyaKutuphanesiId)
        {
            return this.List(x => x.AktifMi == 1 && x.SilindiMi == 0 && x.CografyaKutuphanesiId == cografyaKutuphanesiId);
        }

        /// <summary>
        /// Coğrafya Ayrıntı Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<CografyaListViewModel> CografyaAyrintilarGuncelle(CografyaListViewModel model)
        {
            var cografyaAyrintilarViewModel = this.List(x => x.CografyaKutuphanesiId == model.CografyaKutupanesiId).Value;

            foreach (var item in cografyaAyrintilarViewModel)
            {
                item.AktifMi = 0;
                item.SilindiMi = 1;
                this.Update(item);
            }

            foreach (var sehirId in model.SehirlerIDList)
            {
                var cografyaAyrinti = new CografyaKutuphanesiAyrintilar
                {
                    AktifMi = 1,
                    CografyaAlaninaDahilEdilmeZamani = DateTime.Now,
                    CografyaKutuphanesiId = model.CografyaKutupanesiId,
                    GuncellenmeTarihi = DateTime.Now,
                    UlkelerId = sehirId,
                    KayitTarihi = DateTime.Now,
                    SilindiMi = 0,
                    IlgiliUlkeID = model.UlkeId,
                    GuncelleyenKisiID = model.KisiId,
                    KisiID = _loginUser.KisiID,
                    KurumID = _loginUser.KurumID
                };
                this.Add(cografyaAyrinti);
            }
            return model.ToResult();
        }
    }
}