using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// Param Diller servisi için gerekli methodların yer aldığı sınıf.
    /// </summary>
    public interface IParamDillerDillerService : Baz.Service.Base.IService<ParamDillerDiller>
    {
        
    }

    /// <summary>
    /// ParamDiller ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamDillerDillerService : Base.Service<ParamDillerDiller>, IParamDillerDillerService
    {
        /// <summary>
        /// ParamDiller ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamDillerDillerService(IRepository<ParamDillerDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamDillerDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        
    }
}