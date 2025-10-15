using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Kurum belge tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamKurumBelgeTipleriService : Base.IService<ParamKurumBelgeTipleri>
    {
    }

    /// <summary>
    /// ParamKurumBelgeTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamKurumBelgeTipleriService : Base.Service<ParamKurumBelgeTipleri>, IParamKurumBelgeTipleriService
    {
        /// <summary>
        /// ParamKurumBelgeTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumBelgeTipleriService(IRepository<ParamKurumBelgeTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumBelgeTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}