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
    public interface IKaynakRezervasyonCariDegerlerService : IService<KaynakRezervasyonCariDegerler>
    {
    }

    public class KaynakRezervasyonCariDegerlerService : Service<KaynakRezervasyonCariDegerler>, IKaynakRezervasyonCariDegerlerService
    {
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public KaynakRezervasyonCariDegerlerService(IRepository<KaynakRezervasyonCariDegerler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KaynakRezervasyonCariDegerlerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
