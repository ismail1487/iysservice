using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;

namespace Baz.Service
{
    /// <summary>
    /// İçerik kurumsal şablon tanımlarına ait metotların yer aldığı servis sınıfıdır
    /// </summary>
    public interface IIcerikKurumsalSablonTanimlariService : IService<IcerikKurumsalSablonTanimlari>
    {
        /// <summary>
        /// Id ile ilgili içerik kurumsal şablon tanımını silindi durumuna getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns>silinen veriyi döndürür.</returns>
        Result<IcerikKurumsalSablonTanimlari> SetDeleted(int id);
    }

    /// <summary>
    /// IcerikKurumsalSablonTanimlari ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    public class IcerikKurumsalSablonTanimlariService : Service<IcerikKurumsalSablonTanimlari>, IIcerikKurumsalSablonTanimlariService
    {
        /// <summary>
        /// IcerikKurumsalSablonTanimlari ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public IcerikKurumsalSablonTanimlariService(IRepository<IcerikKurumsalSablonTanimlari> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<IcerikKurumsalSablonTanimlariService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// Id ile ilgili içerik kurumsal şablon tanımını silindi durumuna getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns>silinen veriyi döndürür.</returns>
        public Result<IcerikKurumsalSablonTanimlari> SetDeleted(int id)
        {
            var result = this.SingleOrDefault(id);
            var sablon = result.Value;
            sablon.AktifMi = 0;
            sablon.SilindiMi = 1;
            this.Update(sablon);
            return result;
        }
    }
}