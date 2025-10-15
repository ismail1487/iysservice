using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Kurum tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamKurumTipleriService : Base.IService<ParamKurumTipleri>
    {
    }

    /// <summary>
    /// ParamKurumTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamKurumTipleriService : Base.Service<ParamKurumTipleri>, IParamKurumTipleriService
    {
        /// <summary>
        /// ParamKurumTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumTipleriService(IRepository<ParamKurumTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}