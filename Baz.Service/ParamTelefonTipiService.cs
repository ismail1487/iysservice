using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Telefon tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamTelefonTipiService : IService<ParamTelefonTipi>
    {
    }

    /// <summary>
    /// ParamTelefonTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamTelefonTipiService : Service<ParamTelefonTipi>, IParamTelefonTipiService
    {
        /// <summary>
        /// ParamTelefonTipi ile ilgili işlemleri yöneten servıs sınıfının yapıcı mnetodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamTelefonTipiService(IRepository<ParamTelefonTipi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamTelefonTipiService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}