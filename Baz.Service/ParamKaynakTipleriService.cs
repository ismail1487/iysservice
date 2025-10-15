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
    public interface IParamKaynakTipleriService : Base.IService<ParamKaynakTipleri>
    {
    }

    public class ParamKaynakTipleriService : Base.Service<ParamKaynakTipleri>, IParamKaynakTipleriService
    {

        public ParamKaynakTipleriService(IRepository<ParamKaynakTipleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKaynakTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
