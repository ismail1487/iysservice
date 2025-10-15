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
    /// Sistem Menü Tanım Genel Servisi için gerekli methodların yer aldığı sınıftır.
    /// </summary>
    public interface ISistemMenuTanimlariGenelService : Base.IService<SistemMenuTanimlariGenel>
    {
        /// <summary>
        /// isme göre sistem menü tanımlarını getiren method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Result<SistemMenuTanimlariGenel> SingleOrDefault(string name);
    }

    /// <summary>
    /// Sistem Menü Tanım Genel Servisi ile ilgili işlemleri yöneten servis sınıfı
    /// </summary>
    public class SistemMenuTanimlariGenelService : Base.Service<SistemMenuTanimlariGenel>, ISistemMenuTanimlariGenelService
    {
        /// <summary>
        /// Sistem Menü Tanım Genel Servisi ile ilgili işlemleri yöneten servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public SistemMenuTanimlariGenelService(IRepository<SistemMenuTanimlariGenel> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<SistemMenuTanimlariGenelService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {
        }

        /// <summary>
        /// isme göre sistem menü tanımlarını getiren method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Result<SistemMenuTanimlariGenel> SingleOrDefault(string name)
        {
            return List(p => p.MenuTanimi == name).Value.FirstOrDefault().ToResult();
        }
    }
}