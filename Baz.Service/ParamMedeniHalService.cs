using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Medeni halin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamMedeniHalService : IService<ParamMedeniHal>
    {
    }

    /// <summary>
    /// ParamMedeniHal ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamMedeniHalService : Service<ParamMedeniHal>, IParamMedeniHalService
    {
        /// <summary>
        /// ParamMedeniHal ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamMedeniHalService(IRepository<ParamMedeniHal> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamMedeniHalService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}