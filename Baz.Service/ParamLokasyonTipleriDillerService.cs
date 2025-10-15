using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Lokasyon tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamLokasyonTipleriDillerService : Base.IService<ParamLokasyonTipleriDiller>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParamLokasyonTipleriDillerService : Base.Service<ParamLokasyonTipleriDiller>, IParamLokasyonTipleriDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamLokasyonTipleriDillerService(IRepository<ParamLokasyonTipleriDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamLokasyonTipleriDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}