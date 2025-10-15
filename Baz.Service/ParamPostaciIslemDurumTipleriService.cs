using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Postacı işlem durum tiplerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamPostaciIslemDurumTipleriService : Base.IService<ParamPostaciIslemDurumTipleri>
    {
    }

    /// <summary>
    /// ParamPostaciIslemDurumTipleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamPostaciIslemDurumTipleriService : Base.Service<ParamPostaciIslemDurumTipleri>, IParamPostaciIslemDurumTipleriService
    {
        /// <summary>
        /// ParamPostaciIslemDurumTipleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamPostaciIslemDurumTipleriService(IRepository<ParamPostaciIslemDurumTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamPostaciIslemDurumTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}