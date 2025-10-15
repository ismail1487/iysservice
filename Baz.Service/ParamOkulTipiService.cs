using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Okul tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamOkulTipiService : IService<ParamOkulTipi>
    {
    }

    /// <summary>
    /// ParamOkulTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamOkulTipiService : Service<ParamOkulTipi>, IParamOkulTipiService
    {
        /// <summary>
        /// ParamOkulTipi ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamOkulTipiService(IRepository<ParamOkulTipi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamOkulTipiService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}