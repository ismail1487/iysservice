using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IParamCinsiyetDillerService : IService<ParamCinsiyetDiller>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamCinsiyetDillerService : Service<ParamCinsiyetDiller>, IParamCinsiyetDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamCinsiyetDillerService(IRepository<ParamCinsiyetDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamCinsiyetDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}