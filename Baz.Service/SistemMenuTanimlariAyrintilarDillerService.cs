using Baz.AOP.Logger.ExceptionLog;
using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// Sistem Menü Tanım Ayrıntıları Dil tanımları buradan elde edilecek
    /// </summary>
    public interface ISistemMenuTanimlariAyrintilarDillerService : Base.IService<SistemMenuTanimlariAyrintilarDiller>
    {
    }


    /// <summary>
    /// Sistem Menü Tanım Ayrıntıları Dil tanımları buradan elde edilecek
    /// </summary>
    public class SistemMenuTanimlariAyrintilarDillerService : Base.Service<SistemMenuTanimlariAyrintilarDiller>, ISistemMenuTanimlariAyrintilarDillerService
    {
        /// <summary>
        /// yapıcı metod
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public SistemMenuTanimlariAyrintilarDillerService(IRepository<SistemMenuTanimlariAyrintilarDiller> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<SistemMenuTanimlariAyrintilarDillerService> logger) : base(repository, dataMapper, serviceProvider, logger)
        {

        }
    }
}