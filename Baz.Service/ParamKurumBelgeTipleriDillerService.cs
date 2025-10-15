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
    public interface IParamKurumBelgeTipleriDillerService : Base.IService<ParamKurumBelgeTipleriDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamKurumBelgeTipleriDillerService : Base.Service<ParamKurumBelgeTipleriDiller>, IParamKurumBelgeTipleriDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumBelgeTipleriDillerService(IRepository<ParamKurumBelgeTipleriDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumBelgeTipleriDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}