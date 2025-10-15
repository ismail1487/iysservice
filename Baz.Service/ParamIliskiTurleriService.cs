using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Baz.Service
{
    /// <summary>
    /// İlişki türlerinin parametre olarak tanımlandığı servis sınıfıdır.
    /// </summary>
    public interface IParamIliskiTurleriService : Base.IService<ParamIliskiTurleri>
    {
        /// <summary>
        /// Koda Gore Iliski Turleri getirme
        /// </summary>
        /// <param name="paramKod"></param>
        /// <returns></returns>
        Result<List<ParamIliskiTurleri>> KodaGoreIliskiTurleri(string paramKod);
    }

    /// <summary>
    /// ParamParaBirimleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class ParamIliskiTurleriService : Base.Service<ParamIliskiTurleri>, IParamIliskiTurleriService
    {
        /// <summary>
        /// ParamIliskiTurleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public ParamIliskiTurleriService(IRepository<ParamIliskiTurleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<ParamIliskiTurleriService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// Koda Gore Iliski Turleri getirme
        /// </summary>
        /// <param name="paramKod"></param>
        /// <returns></returns>
        public Result<List<ParamIliskiTurleri>> KodaGoreIliskiTurleri(string paramKod)
        {
            var result = List(x => x.ParamKod == paramKod);
            return result;
        }
    }
}