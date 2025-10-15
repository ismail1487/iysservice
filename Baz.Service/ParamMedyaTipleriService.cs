using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Medya tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamMedyaTipleriService : Base.IService<ParamMedyaTipleri>
    {
    }

    /// <summary>
    /// ParamMedyaTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamMedyaTipleriService : Base.Service<ParamMedyaTipleri>, IParamMedyaTipleriService
    {
        /// <summary>
        /// ParamMedyaTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamMedyaTipleriService(IRepository<ParamMedyaTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamMedyaTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}