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
    public interface IParamDilSeviyesiDillerService : IService<ParamDilSeviyesiDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamDilSeviyesiDillerService : Service<ParamDilSeviyesiDiller>, IParamDilSeviyesiDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamDilSeviyesiDillerService(IRepository<ParamDilSeviyesiDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamDilSeviyesiDiller> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}