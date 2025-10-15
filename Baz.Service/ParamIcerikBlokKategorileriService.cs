using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baz.Service
{
    public interface IParamIcerikBlokKategorileriService : IService<ParamIcerikBlokKategorileri>
    {
    }

    /// <summary>
    /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamIcerikBlokKategorileriService : Service<ParamIcerikBlokKategorileri>, IParamIcerikBlokKategorileriService
    {
        /// <summary>
        /// ParamBankalar ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamIcerikBlokKategorileriService(IRepository<ParamIcerikBlokKategorileri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamIcerikBlokKategorileriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
