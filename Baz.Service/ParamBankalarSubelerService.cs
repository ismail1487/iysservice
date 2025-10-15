using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Baz.Service
{
    /// <summary>
    /// Bankaların ve şubelerin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamBankalarSubelerService : IService<ParamBankalar>
    {
    }

    /// <summary>
    /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamBankalarSubelerService : Service<ParamBankalar>, IParamBankalarSubelerService
    {
        /// <summary>
        /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamBankalarSubelerService(IRepository<ParamBankalar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamBankalarSubelerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}