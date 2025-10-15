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
    public interface IKaynakRezerveTanimlariService : Base.IService<KaynakRezerveTanimlari>
    {
    }

    public class KaynakRezerveTanimlariService : Base.Service<KaynakRezerveTanimlari>, IKaynakRezerveTanimlariService
    {

        public KaynakRezerveTanimlariService(IRepository<KaynakRezerveTanimlari> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KaynakRezerveTanimlariService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
