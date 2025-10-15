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
                
    public interface IKaynakTanimlariMedyalarService : IService<KaynakTanimlariMedyalar>
    {
    }
    public class KaynakTanimlariMedyalarService : Service<KaynakTanimlariMedyalar>, IKaynakTanimlariMedyalarService
    {

        public KaynakTanimlariMedyalarService(IRepository<KaynakTanimlariMedyalar> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KaynakTanimlariMedyalarService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

    }
}
