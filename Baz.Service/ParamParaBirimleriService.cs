using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Para birimlerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamParaBirimleriService : Base.IService<ParamParaBirimleri>
    {
    }

    /// <summary>
    /// ParamParaBirimleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamParaBirimleriService : Base.Service<ParamParaBirimleri>, IParamParaBirimleriService
    {
        /// <summary>
        /// ParamParaBirimleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamParaBirimleriService(IRepository<ParamParaBirimleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamParaBirimleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}