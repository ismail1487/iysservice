using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Kurum sektörlerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamKurumSektorleriService : Base.IService<ParamKurumSektorleri>
    {
    }

    /// <summary>
    /// ParamKurumSektorleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamKurumSektorleriService : Base.Service<ParamKurumSektorleri>, IParamKurumSektorleriService
    {
        /// <summary>
        /// ParamKurumSektorleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumSektorleriService(IRepository<ParamKurumSektorleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumSektorleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}