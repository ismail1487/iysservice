using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Lokasyon tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamTalepSurecStatuleriService : Base.IService<ParamTalepSurecStatuleri>
    {
    }

    /// <summary>
    /// ParamTalepSurecStatuleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamTalepSurecStatuleriService : Base.Service<ParamTalepSurecStatuleri>, IParamTalepSurecStatuleriService
    {
        /// <summary>
        /// ParamTalepSurecStatuleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamTalepSurecStatuleriService(IRepository<ParamTalepSurecStatuleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamTalepSurecStatuleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}