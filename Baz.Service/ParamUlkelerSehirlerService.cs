using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Baz.Service
{
    /// <summary>
    /// Ülkeler ve şehirlerin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamUlkelerSehirlerService : IService<ParamUlkeler>
    {
        /// <summary>
        /// Ülkelerin listelendiği method
        /// </summary>
        /// <returns></returns>
        Result<List<ParamUlkeler>> UlkeListesi();

        /// <summary>
        /// Şehirlerin listelendiği method
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        Result<List<ParamUlkeler>> SehirListesi(int ulkeID);
    }

    /// <summary>
    /// ParamUlkeler ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamUlkelerSehirlerService : Service<ParamUlkeler>, IParamUlkelerSehirlerService
    {
        /// <summary>
        /// ParamUlkeler ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamUlkelerSehirlerService(IRepository<ParamUlkeler> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamUlkelerSehirlerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// Ülkelerin listelendiği method
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamUlkeler>> UlkeListesi()
        {
            var list = this.List(x => x.UstId == 0);
            return list;
        }

        /// <summary>
        /// Şehirlerin listelendiği method
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        public Result<List<ParamUlkeler>> SehirListesi(int ulkeID)
        {
            var list = this.List(x => x.UstId == ulkeID);
            return list;
        }
    }
}