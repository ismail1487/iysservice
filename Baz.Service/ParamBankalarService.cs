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
    public interface IParamBankalarService : Base.IService<ParamBankalar>
    {
    }

    /// <summary>
    /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamBankalarService : Base.Service<ParamBankalar>, IParamBankalarService
    {
        /// <summary>
        /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamBankalarService(IRepository<ParamBankalar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamBankalarService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}