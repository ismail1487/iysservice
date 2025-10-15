using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Ölçüm birimlerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamOlcumBirimleriService : Base.IService<ParamOlcumBirimleri>
    {
    }

    /// <summary>
    /// ParamOlcumBirimleri ile ilgili işlemleri yöneten servıs sınıfın
    /// </summary>
    public class ParamOlcumBirimleriService : Base.Service<ParamOlcumBirimleri>, IParamOlcumBirimleriService
    {
        /// <summary>
        /// ParamOlcumBirimleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamOlcumBirimleriService(IRepository<ParamOlcumBirimleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamOlcumBirimleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}