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
    public interface IParamAdresTipiService : IService<ParamAdresTipi>
    {
    }

    /// <summary>
    /// ParamAdresTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamAdresTipleriService : Service<ParamAdresTipi>, IParamAdresTipiService
    {
        /// <summary>
        /// ParamAdresTipi ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamAdresTipleriService(IRepository<ParamAdresTipi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamAdresTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}