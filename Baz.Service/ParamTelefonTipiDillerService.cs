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
    public interface IParamTelefonTipiDillerService : IService<ParamTelefonTipiDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamTelefonTipiDillerService : Service<ParamTelefonTipiDiller>, IParamTelefonTipiDillerService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamTelefonTipiDillerService(IRepository<ParamTelefonTipiDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamTelefonTipiDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}