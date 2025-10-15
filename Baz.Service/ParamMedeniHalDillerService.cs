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
    public interface IParamMedeniHalDillerService : IService<ParamMedeniHalDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamMedeniHalDillerService : Service<ParamMedeniHalDiller>, IParamMedeniHalDillerService
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="repository"></param>
       /// <param name="dataMapper"></param>
       /// <param name="serviceProvider"></param>
       /// <param name="logger"></param>
        public ParamMedeniHalDillerService(IRepository<ParamMedeniHalDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamMedeniHalDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}