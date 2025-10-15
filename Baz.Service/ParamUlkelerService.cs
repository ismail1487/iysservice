using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Ülkelerin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamUlkelerService : Base.IService<ParamUlkeler>
    {
    }

    /// <summary>
    /// ParamUlkeler ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamUlkelerService : Base.Service<ParamUlkeler>, IParamUlkelerService
    {
        /// <summary>
        /// ParamUlkeler ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamUlkelerService(IRepository<ParamUlkeler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamUlkelerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}