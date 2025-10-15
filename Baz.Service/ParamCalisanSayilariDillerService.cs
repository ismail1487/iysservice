using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Çalışan sayılarının parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamCalisanSayilariDillerService : IService<ParamCalisanSayilariDiller> { }

    /// <summary>
    /// ParamCalisanSayilari ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamCalisanSayilariDillerService : Service<ParamCalisanSayilariDiller>, IParamCalisanSayilariDillerService
    {
        /// <summary>
        /// ParamCalisanSayilari ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamCalisanSayilariDillerService(IRepository<ParamCalisanSayilariDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamCalisanSayilariDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}