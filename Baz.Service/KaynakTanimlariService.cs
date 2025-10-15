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
    public interface IKaynakTanimlariService : Base.IService<KaynakTanimlari>
    {
    }


    public class KaynakTanimlariService : Base.Service<KaynakTanimlari>, IKaynakTanimlariService
    {

        public KaynakTanimlariService(IRepository<KaynakTanimlari> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KaynakTanimlariService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
