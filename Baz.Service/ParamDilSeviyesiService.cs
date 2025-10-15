using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Dinlerin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamDilSeviyesiService : IService<ParamDilSeviyesi>
    {
    }

    /// <summary>
    /// ParamDilSeviyesi ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamDilSeviyesiService : Service<ParamDilSeviyesi>, IParamDilSeviyesiService
    {
        /// <summary>
        /// ParamDilSeviyesi ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamDilSeviyesiService(IRepository<ParamDilSeviyesi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamDilSeviyesi> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}