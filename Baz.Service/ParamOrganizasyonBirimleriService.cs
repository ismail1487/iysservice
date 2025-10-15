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

namespace Baz.Service
{
    /// <summary>
    /// Organizasyon birimlerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamOrganizasyonBirimleriService : IService<ParamOrganizasyonBirimleri>
    {
        /// <summary>
        /// Param Birim tanımlarını listeleyen method
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamBirimTanimView>> ListForView();

        /// <summary>
        /// Parametre tanımına göre TipId döndüren method
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result<int> GetTipId(string name);
    }

    /// <summary>
    ///   ParamOrganizasyonBirimleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamOrganizasyonBirimleriService : Service<ParamOrganizasyonBirimleri>, IParamOrganizasyonBirimleriService
    {
        /// <summary>
        ///   ParamOrganizasyonBirimleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamOrganizasyonBirimleriService(IRepository<ParamOrganizasyonBirimleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamOrganizasyonBirimleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// Parametre tanımına göre TipId döndüren method
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result<int> GetTipId(string name)
        {
            var result = List(p => p.ParamTanim.ToLower() == name.ToLower()).Value.FirstOrDefault();
            return result.TabloID.ToResult();
        }

        /// <summary>
        /// Param Birim tanımlarını listeleyen method
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamBirimTanimView>> ListForView()
        {
            return List().Value.Select(p => new ParamBirimTanimView() { TabloId = p.TabloID, Tanim = p.ParamTanim }).ToList().ToResult();
        }
    }
}