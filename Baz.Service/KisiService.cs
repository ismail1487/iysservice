using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Baz.RequestManager.Abstracts;
using System.Linq;
using Baz.Model.Entity.Constants;

namespace Baz.Service
{
    /// <summary>
    /// Kişi servisinin tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IKisiService : Base.IService<KisiTemelBilgiler>
    {
        /// <summary>
        /// Kişilerin resim urlleri ile birlikte döndüren met
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Result<KisiTemelBilgiler> KisiResimUrlileGetir(int userID);

        /// <summary>
        /// Kişi temel bilgiler listeleyen metod
        /// </summary>
        /// <returns></returns>
        public IQueryable<KisiTemelBilgiler> ListForQery();

        /// <summary>
        /// kisi Id değeri ile kendisine bağlı sanal kişileri getiren metot.
        /// </summary>
        /// <param name="aktifKisiId"></param>
        /// <returns></returns>
        public Result<List<int>> KisiSanalKisileriniGetir(int aktifKisiId);
    }

    /// <summary>
    /// KisiTemelBilgiler ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class KisiService : Base.Service<KisiTemelBilgiler>, IKisiService
    {
        private readonly string medyaUrl = LocalPortlar.MedyaKutuphanesiService;
        private readonly IRequestHelper _requestHelper;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;

        /// <summary>
        /// KisiTemelBilgiler ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="requestHelper"></param>
        public KisiService(IRepository<KisiTemelBilgiler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KisiService> logger, IRequestHelper requestHelper, IMedyaKutuphanesiService medyaKutuphanesiService) : base(repository, dataMapper, serviceProvider, logger)
        {
            _requestHelper = requestHelper; _medyaKutuphanesiService = medyaKutuphanesiService;
        }

        /// <summary>
        /// Kişilerin resim urlleri ile birlikte döndüren met
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Result<KisiTemelBilgiler> KisiResimUrlileGetir(int userID)
        {
            var user = List(a => a.TabloID == userID && a.AktifMi == 1).Value.FirstOrDefault();

            if (user != null && user.KisiResimId > 0)
            {
                var ppResult = _medyaKutuphanesiService.SingleOrDefault(user.KisiResimId);
                if (ppResult.IsSuccess)
                    user.KisiResimUrl = medyaUrl + ppResult.Value.MedyaUrl;
            }

            return user.ToResult();
        }

        /// <summary>
        /// Kişi temel bilgiler listeleyen metod
        /// </summary>
        /// <returns></returns>
        public IQueryable<KisiTemelBilgiler> ListForQery()
        {
            return _repository.List();
        }

        /// <summary>
        /// kisi Id değeri ile kendisine bağlı sanal kişileri getiren metot.
        /// </summary>
        /// <param name="aktifKisiId"></param>
        /// <returns></returns>
        public Result<List<int>> KisiSanalKisileriniGetir(int aktifKisiId)
        {
            //var result = _repository.List(a => a.AktifKisiHesabiID == aktifKisiId && a.AktifMi == 1).Select(a => a.TabloID).ToList();
            //return result.ToResult();
            // AktifKisiHesabıID alanı kaldırıldı, bu metod geçersiz.
            var result = new List<int>();
            return result.ToResult();
        }
    }
}