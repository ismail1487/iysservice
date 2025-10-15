using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Sistem modüllerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamSistemModulleriService : Base.IService<ParamSistemModulleri>
    {
    }

    /// <summary>
    /// ParamSistemModulleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamSistemModulleriService : Base.Service<ParamSistemModulleri>, IParamSistemModulleriService
    {
        /// <summary>
        /// ParamSistemModulleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamSistemModulleriService(IRepository<ParamSistemModulleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamSistemModulleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}