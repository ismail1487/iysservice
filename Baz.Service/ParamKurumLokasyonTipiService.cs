using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Kurum lokasyon tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamKurumLokasyonTipiService : IService<ParamKurumLokasyonTipi>
    {
    }

    /// <summary>
    /// ParamKurumLokasyonTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamKurumLokasyonTipiService : Service<ParamKurumLokasyonTipi>, IParamKurumLokasyonTipiService
    {
        /// <summary>
        /// ParamKurumLokasyonTipi ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumLokasyonTipiService(IRepository<ParamKurumLokasyonTipi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumLokasyonTipiService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}