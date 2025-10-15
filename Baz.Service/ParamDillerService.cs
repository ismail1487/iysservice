using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// Param Diller servisi için gerekli methodların yer aldığı sınıf.
    /// </summary>
    public interface IParamDillerService : Baz.Service.Base.IService<ParamDiller>
    {
        /// <summary>
        /// Dil koduna göre mevcut dil verisini getiren method.
        /// </summary>
        /// <param name="dilKodu">dil kodu.</param>
        /// <returns></returns>
        Result<ParamDiller> SingleOrDefault(string dilKodu);

        /// <summary>
        /// Dil Id'ye göre dil kodunu getiren metod
        /// </summary>
        /// <param name="dilID"></param>
        /// <returns></returns>
        Result<string> ParamKodGetir(int dilID);
    }

    /// <summary>
    /// ParamDiller ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamDillerService : Base.Service<ParamDiller>, IParamDillerService
    {
        /// <summary>
        /// ParamDiller ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamDillerService(IRepository<ParamDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// Dil koduna göre mevcut dil verisini getiren method.
        /// </summary>
        /// <param name="dilKodu">dil kodu.</param>
        /// <returns></returns>
        public Result<ParamDiller> SingleOrDefault(string dilKodu)
        {
            List(p => p.SistemParamMi);
            return List(p => p.ParamKod == dilKodu).Value.FirstOrDefault().ToResult();
        }

        /// <summary>
        /// Dil Id'ye göre dil kodunu getiren metod
        /// </summary>
        /// <param name="dilID"></param>
        /// <returns></returns>
        public Result<string> ParamKodGetir(int dilID)
        {
            var result = List(p => p.TabloID == dilID).Value.FirstOrDefault();
            return result.ParamKod.ToResult();
        }
    }
}