

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
    public interface IParamIcerikKategorilerService : IService<ParamIcerikKategoriler>
    {
    }

    /// <summary>
    /// ParamAdresTipi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamIcerikKategorilerService : Service<ParamIcerikKategoriler>, IParamIcerikKategorilerService
    {
        /// <summary>
        /// ParamAdresTipi ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamIcerikKategorilerService(IRepository<ParamIcerikKategoriler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamIcerikKategoriler> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}