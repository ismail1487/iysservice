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
    public interface IParamDinlerDillerService : IService<ParamDinlerDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamDinlerDillerService : Service<ParamDinlerDiller>, IParamDinlerDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamDinlerDillerService(IRepository<ParamDinlerDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamDinlerDiller> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}