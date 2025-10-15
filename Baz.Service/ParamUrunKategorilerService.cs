using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baz.Service
{
    public interface IParamUrunKategorilerService : Base.IService<ParamUrunKategoriler>{}

    public class ParamUrunKategorilerService : Base.Service<ParamUrunKategoriler>, IParamUrunKategorilerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamUrunKategorilerService(IRepository<ParamUrunKategoriler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamUrunKategorilerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {

        }
    }

}
