using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Adres tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamAdresTipiDillerService : IService<ParamAdresTipiDiller>
    {


    }

    /// <summary>
    /// ParamAdresTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamAdresTipiDillerService : Service<ParamAdresTipiDiller>, IParamAdresTipiDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamAdresTipiDillerService(IRepository<ParamAdresTipiDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamAdresTipiDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}