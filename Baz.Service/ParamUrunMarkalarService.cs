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
    public interface IParamUrunMarkalarService : Base.IService<ParamUrunMarkalar>
    {
    }


    public class ParamUrunMarkalarService : Base.Service<ParamUrunMarkalar>, IParamUrunMarkalarService
    {

        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamUrunMarkalarService(IRepository<ParamUrunMarkalar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamUrunMarkalarService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
