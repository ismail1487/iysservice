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
    public interface IParamKurumLokasyonTipiDillerService : IService<ParamKurumLokasyonTipiDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamKurumLokasyonTipiDillerService : Service<ParamKurumLokasyonTipiDiller>, IParamKurumLokasyonTipiDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamKurumLokasyonTipiDillerService(IRepository<ParamKurumLokasyonTipiDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKurumLokasyonTipiDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}