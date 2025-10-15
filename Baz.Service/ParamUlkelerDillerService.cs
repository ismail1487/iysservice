using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IParamUlkelerDillerService : Base.IService<ParamUlkelerDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamUlkelerDillerService : Base.Service<ParamUlkelerDiller>, IParamUlkelerDillerService
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="repository"></param>
       /// <param name="dataMapper"></param>
       /// <param name="serviceProvider"></param>
       /// <param name="logger"></param>
        public ParamUlkelerDillerService(IRepository<ParamUlkelerDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamUlkelerDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}