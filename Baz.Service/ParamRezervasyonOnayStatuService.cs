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
    public interface IParamRezervasyonOnayStatuService : Base.IService<ParamRezervasyonOnayStatu>
    {
    }

    public class ParamRezervasyonOnayStatuService : Base.Service<ParamRezervasyonOnayStatu>, IParamRezervasyonOnayStatuService
    {

        public ParamRezervasyonOnayStatuService(IRepository<ParamRezervasyonOnayStatu> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamKaynakTipleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}