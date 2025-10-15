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
    public interface IParamOlcumKategorileriService : Base.IService<ParamOlcumKategorileri>
    {
    }


    public class ParamOlcumKatergorileriService : Base.Service<ParamOlcumKategorileri>, IParamOlcumKategorileriService
    {

        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamOlcumKatergorileriService(IRepository<ParamOlcumKategorileri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamOlcumKatergorileriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
