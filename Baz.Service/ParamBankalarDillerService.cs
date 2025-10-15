using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// Banka, şube ve şube kodlarının parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamBankalarDillerService : Base.IService<ParamBankalarDiller>
    {
    }

    /// <summary>
    /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamBankalarDillerService : Base.Service<ParamBankalarDiller>, IParamBankalarDillerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamBankalarDillerService(IRepository<ParamBankalarDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamBankalarDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {

        }
    }
}