using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Kimlik tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamKimlikTipleriService : Base.IService<ParamKimlikTipleri>
    {
    }

    /// <summary>
    /// ParamKimlikTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamKimlikTipleriService : Base.Service<ParamKimlikTipleri>, IParamKimlikTipleriService
    {
        /// <summary>
        /// ParamKimlikTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKimlikTipleriService(IRepository<ParamKimlikTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKimlikTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}