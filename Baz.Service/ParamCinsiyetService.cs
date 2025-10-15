using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Cinsiyetin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamCinsiyetService : IService<ParamCinsiyet>
    {
    }

    /// <summary>
    /// ParamCinsiyet ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamCinsiyetService : Service<ParamCinsiyet>, IParamCinsiyetService
    {
        /// <summary>
        /// ParamCinsiyet ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamCinsiyetService(IRepository<ParamCinsiyet> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamCinsiyetService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}