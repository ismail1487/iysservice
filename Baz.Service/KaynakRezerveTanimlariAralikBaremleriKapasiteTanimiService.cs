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
    public interface IKaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService : Base.IService<KaynakRezerveTanimlariAralikBaremleriKapasiteTanimi>
    {
    }


    public class KaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService : Base.Service<KaynakRezerveTanimlariAralikBaremleriKapasiteTanimi>, IKaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService
    {

        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public KaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService(IRepository<KaynakRezerveTanimlariAralikBaremleriKapasiteTanimi> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<KaynakRezerveTanimlariAralikBaremleriKapasiteTanimiService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}
