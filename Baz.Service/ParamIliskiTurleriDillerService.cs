using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Baz.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IParamIliskiTurleriDillerService : Base.IService<ParamIliskiTurleriDiller>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParamIliskiTurleriDillerService : Base.Service<ParamIliskiTurleriDiller>, IParamIliskiTurleriDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamIliskiTurleriDillerService(IRepository<ParamIliskiTurleriDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamIliskiTurleriDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}