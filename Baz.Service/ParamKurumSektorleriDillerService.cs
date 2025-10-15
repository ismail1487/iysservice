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
    public interface IParamKurumSektorleriDillerService : Base.IService<ParamKurumSektorleriDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamKurumSektorleriDillerService : Base.Service<ParamKurumSektorleriDiller>, IParamKurumSektorleriDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumSektorleriDillerService(IRepository<ParamKurumSektorleriDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumSektorleriDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}