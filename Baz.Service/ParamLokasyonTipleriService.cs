using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Lokasyon tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamLokasyonTipleriService : Base.IService<ParamLokasyonTipleri>
    {
    }

    /// <summary>
    /// ParamLokasyonTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamLokasyonTipleriService : Base.Service<ParamLokasyonTipleri>, IParamLokasyonTipleriService
    {
        /// <summary>
        /// ParamLokasyonTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamLokasyonTipleriService(IRepository<ParamLokasyonTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamLokasyonTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}