using System;
using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;

namespace Baz.Service
{
    /// <summary>
    /// Proje genel bilgileri servisi interface'i
    /// </summary>
    public interface IProjeGenelBilgilerService : IService<ProjeGenelBilgiler>
    {
        
    }

    /// <summary>
    /// Proje genel bilgileri servisi
    /// </summary>
    public class ProjeGenelBilgilerService : Service<ProjeGenelBilgiler>, IProjeGenelBilgilerService
    {
        /// <summary>
        /// SistemKureselParametreDegerleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="paramKureselParametrelerService"></param>
        public ProjeGenelBilgilerService(IRepository<ProjeGenelBilgiler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ProjeGenelBilgilerService> logger, IParamKureselParametrelerService paramKureselParametrelerService) : base(repository, dataMapper, serviceProvider, logger)
        {
        }
    }
}