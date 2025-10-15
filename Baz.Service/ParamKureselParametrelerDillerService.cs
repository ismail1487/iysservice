using Baz.AOP.Logger.ExceptionLog;
using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IParamKureselParametrelerDillerService : IService<ParamKureselParametrelerDiller>
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamKureselParametrelerDillerService : Service<ParamKureselParametrelerDiller>, IParamKureselParametrelerDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKureselParametrelerDillerService(IRepository<ParamKureselParametrelerDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKureselParametrelerDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

    }
}