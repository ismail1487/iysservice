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
using Baz.AOP.Logger.ExceptionLog;

namespace Baz.Service
{
    /// <summary>
    /// Coğrafya Tanım İşlemlerinin yapıldığı interface
    /// </summary>
    public interface ICografyaTanimService : IService<CografyaKutuphanesi>
    {
        /// <summary>
        /// Coğrafya Tanım kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<int> CografyaTanimKayit(CografyaListViewModel model);

        /// <summary>
        /// Coğrafya Tanım Listesi getirme
        /// </summary>
        /// <returns></returns>
        Result<List<CografyaListViewModel>> CografyaTanimListesiGetir();

        /// <summary>
        /// Coğrafya Tanım Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<CografyaKutuphanesi> CografyaTanimGuncelle(CografyaListViewModel model);

        /// <summary>
        /// Coğrafya Tanım id'sine göre getirme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<CografyaListViewModel> CografyaTanimIdyeGoreGetir(int cografyaKutuphanesiId);

        /// <summary>
        /// Coğrafta Tanım Silme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<bool> CografyaTanimSil(int cografyaKutuphanesiId);
    }

    /// <summary>
    /// Coğrafya Tanım İşlemlerinin yapıldığı service
    /// </summary>
    public class CografyaTanimService : Service<CografyaKutuphanesi>, ICografyaTanimService
    {
        private readonly ILoginUser _loginUser;
        private readonly ICografyaAyrintilarService _cografyaAyrintilarService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="loginUser"></param>
        /// <param name="cografyaAyrintilarService"></param>
        public CografyaTanimService(IRepository<CografyaKutuphanesi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<CografyaTanimService> logger, ILoginUser loginUser, ICografyaAyrintilarService cografyaAyrintilarService) : base(repository, dataMapper, serviceProvider, logger)
        {
            _loginUser = loginUser;
            _cografyaAyrintilarService = cografyaAyrintilarService;
        }

        /// <summary>
        /// Coğrafya Tanım kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> CografyaTanimKayit(CografyaListViewModel model)
        {
            try
            {
                _repository.DataContextConfiguration().BeginNewTransactionIsNotFound();
                var tanimList = this.List().Value;
                bool varMi = tanimList.Any(x =>
                    x.CografyaAlaniTanimi == model.CografyaTanim && x.AktifMi == 1 && x.SilindiMi == 0);
                if (varMi)
                {
                    throw new OctapullException(OctapullExceptions.DuplicateDataError);
                }
                var cografyaTanim = new CografyaKutuphanesi
                {
                    GuncellenmeTarihi = DateTime.Now,
                    CografyaAlaniAciklama = model.CografyaAciklama,
                    CografyaAlaniTanimi = model.CografyaTanim,
                    CografyaDuzeyiUlkeMiSehirMiIlceMiMahalleMiSokakMi = 2,
                    KayitTarihi = DateTime.Now,
                    SilindiMi = 0,
                    AktifMi = 1,
                    KisiID = _loginUser.KisiID,
                    KurumID = _loginUser.KurumID,
                };
                var result = this.Add(cografyaTanim);
                model.CografyaKutupanesiId = result.Value.TabloID;
                _cografyaAyrintilarService.CografyaAyrintilarKayit(model);
                _repository.DataContextConfiguration().Commit();
                return result.Value.TabloID.ToResult();
            }
            catch (Exception ex)
            {
                _repository.DataContextConfiguration().RollBack();
                throw ex;
            }

        }

        /// <summary>
        /// Coğrafya Tanım Listesi getirme
        /// </summary>
        /// <returns></returns>
        public Result<List<CografyaListViewModel>> CografyaTanimListesiGetir()
        {
            var result = new List<CografyaListViewModel>();
            var tanimList = this.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value;

            var cografyaKutuphanesiList = _cografyaAyrintilarService.List(x => x.AktifMi == 1 && x.SilindiMi == 0).Value;
            foreach (var cografyaTanim in tanimList)
            {
                var cografyaAyrintilarView = cografyaKutuphanesiList.Where(x => x.CografyaKutuphanesiId == cografyaTanim.TabloID).ToList();
                var cografyaViewModel = new CografyaListViewModel()
                {
                    CografyaAciklama = cografyaTanim.CografyaAlaniAciklama,
                    CografyaKutupanesiId = cografyaTanim.TabloID,
                    SehirSayisi = cografyaAyrintilarView.Count,
                    SehirlerIDList = cografyaAyrintilarView.Select(x => x.UlkelerId).ToList(),
                    CografyaTanim = cografyaTanim.CografyaAlaniTanimi,
                };
                result.Add(cografyaViewModel);
            }

            return result.ToResult();
        }

        /// <summary>
        /// Coğrafta Tanım Silme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<bool> CografyaTanimSil(int cografyaKutuphanesiId)
        {
            try
            {
                _repository.DataContextConfiguration().BeginNewTransactionIsNotFound();
                var entity = this.SingleOrDefault(cografyaKutuphanesiId).Value;
                if (entity==null)
                {
                    throw new OctapullException(OctapullExceptions.MissingDataError);
                }
                entity.SilindiMi = 1;
                entity.AktifMi = 0;
                entity.SilinmeTarihi = DateTime.Now;
                this.Update(entity);
                _cografyaAyrintilarService.CografyaAyrintilarSil(cografyaKutuphanesiId);
                _repository.DataContextConfiguration().Commit();
                return true.ToResult();
            }
            catch (Exception ex)
            {
                _repository.DataContextConfiguration().RollBack();
                throw ex;
            }
        }

        /// <summary>
        /// Coğrafya Tanım Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<CografyaKutuphanesi> CografyaTanimGuncelle(CografyaListViewModel model)
        {
            try
            {
                _repository.DataContextConfiguration().BeginNewTransactionIsNotFound();
                var tanimList = this.List().Value;
                bool varMi = tanimList.Any(x => x.CografyaAlaniTanimi == model.CografyaTanim && x.AktifMi == 1 && x.SilindiMi == 0 && x.TabloID != model.CografyaKutupanesiId);
                if (varMi)
                {
                    throw new OctapullException(OctapullExceptions.DuplicateDataError);
                }
                var entity = this.SingleOrDefault(model.CografyaKutupanesiId).Value;
                entity.CografyaAlaniAciklama = model.CografyaAciklama;
                entity.CografyaAlaniTanimi = model.CografyaTanim;
                entity.GuncellenmeTarihi = DateTime.Now;
                entity.GuncelleyenKisiID = model.KisiId == 0 ? _loginUser.KisiID : model.KisiId;
                var result = this.Update(entity);
                _cografyaAyrintilarService.CografyaAyrintilarGuncelle(model);
                _repository.DataContextConfiguration().Commit();
                return result;
            }
            catch (Exception ex)
            {
                _repository.DataContextConfiguration().RollBack();
                throw ex;
            }
        }

        /// <summary>
        /// Coğrafya Tanım id'sine göre getirme
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<CografyaListViewModel> CografyaTanimIdyeGoreGetir(int cografyaKutuphanesiId)
        {

            var cografyaKutuphanesi = this.SingleOrDefault(cografyaKutuphanesiId).Value;
            var cografyaAyrintilar =
                _cografyaAyrintilarService.CografyaAyrintilarGetirIdsineGore(cografyaKutuphanesiId).Value;
            if (cografyaKutuphanesi==null)
            {
                throw new OctapullException(OctapullExceptions.MissingDataError);
            }
            var cografyaViewModel = new CografyaListViewModel()
            {
                CografyaAciklama = cografyaKutuphanesi.CografyaAlaniAciklama,
                CografyaTanim = cografyaKutuphanesi.CografyaAlaniTanimi,
                SehirSayisi = cografyaAyrintilar.Count,
                SehirlerIDList = cografyaAyrintilar.Select(x => x.UlkelerId).ToList(),
                CografyaKutupanesiId = cografyaKutuphanesi.TabloID,
                UlkeId = cografyaAyrintilar.FirstOrDefault().IlgiliUlkeID
            };
            return cografyaViewModel.ToResult();
        }
    }
}