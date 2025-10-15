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
    public interface IParamKurumTipleriDillerService : Base.IService<ParamKurumTipleriDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamKurumTipleriDillerService : Base.Service<ParamKurumTipleriDiller>, IParamKurumTipleriDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumTipleriDillerService(IRepository<ParamKurumTipleriDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumTipleriDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}