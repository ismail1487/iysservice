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
    public interface IParamOkulTipiDillerService : IService<ParamOkulTipiDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamOkulTipiDillerService : Service<ParamOkulTipiDiller>, IParamOkulTipiDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamOkulTipiDillerService(IRepository<ParamOkulTipiDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamOkulTipiDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}