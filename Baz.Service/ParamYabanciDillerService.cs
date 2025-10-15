using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
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
    public interface IParamYabanciDillerService : IService<ParamYabanciDiller>
    {
        public Result<List<ParamYabanciDiller>> YabanciDilList();

    }
    public class ParamYabanciDillerService : Service<ParamYabanciDiller>, IParamYabanciDillerService
    {
        public ParamYabanciDillerService(IRepository<ParamYabanciDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamMedeniHalDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
        public Result<List<ParamYabanciDiller>> YabanciDilList()
        {
            var yabanciDillerList = this.List(x => x.AktifMi == 1 && x.SilindiMi == 0);
            return yabanciDillerList;
        }

    }
}
