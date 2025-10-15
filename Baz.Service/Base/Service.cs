using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.Repository.Common;
using Baz.Repository.Pattern;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Baz.AOP.Logger.ExceptionLog;

namespace Baz.Service.Base
{
    /// <summary>
    /// Ekleme,düzenleme,silme listeleme vb işlemlerin yer aldığı sınftır.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Baz.Service.Base.IService{TEntity}" />
    public class Service<TEntity> : IService<TEntity> where TEntity : class, Baz.Model.Pattern.IBaseModel
    {
        /// <summary>
        /// Reposiitory değişkeni
        /// </summary>
        protected readonly IRepository<TEntity> _repository;

        /// <summary>
        /// Model mapper
        /// </summary>
        protected IDataMapper _dataMapper;

        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger _logger;

        /// <summary>
        /// Servis collector
        /// </summary>
        protected IServiceProvider _serviceProvider;

        private readonly ILoginUser _loginUser;
        private bool _disposed;

        /// <summary>
        /// Baz servisin yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public Service(IRepository<TEntity> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger logger)
        {
            _repository = repository;
            _dataMapper = dataMapper;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _loginUser = _serviceProvider.GetService<ILoginUser>();
        }

        /// <summary>
        /// Ekleme işleminin yapıldığı methodtur.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual Result<TEntity> Add(TEntity entity)
        {
            if (IslemYetkisiVarMi(entity))
            {
                var result = _repository.Add(entity);
                if (_repository.SaveChanges() > 0)
                    return result.ToResult();
                throw new OctapullException(OctapullExceptions.AddError);
            }
            throw new OctapullException(OctapullExceptions.AuthError);
        }

        /// <summary>
        /// Silme işleminin yapıldığı methodtur.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual Result<TEntity> Delete(int id)
        {
            var entity = _repository.SingleOrDefault(id).ToResult();
            if (IslemYetkisiVarMi(entity.Value))
            {
                var result = _repository.Delete(id);
                if (_repository.SaveChanges() > 0)
                    return result.ToResult();
                throw new OctapullException(OctapullExceptions.DeleteError);
            }
            throw new OctapullException(OctapullExceptions.AuthError);
        }

        /// <summary>
        /// Listeleme yapılan methodtur.
        /// </summary>
        /// <returns></returns>
        public virtual Result<List<TEntity>> List()
        {
            return _repository.List(p => p.TabloID > 0).ToList().ToResult();
        }

        /// <summary>
        /// Alınan parametreye göre listelemenin yapıldığı methodtur.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public virtual Result<List<TEntity>> List(Expression<Func<TEntity, bool>> expression)
        {
            return _repository.List(expression).ToList().ToResult();
        }

        /// <summary>
        /// Parametreye göre listeleme
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Result<List<TEntity>> ListForFunc(Func<TEntity, bool> expression)
        {
            return _repository.ListForFunc(expression).ToList().ToResult();
        }

        /// <summary>
        /// Id'ye göre sonucun döndürüldüğü methodtur.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual Result<TEntity> SingleOrDefault(int id)
        {
            var entity = _repository.SingleOrDefault(id).ToResult();
            if (IslemYetkisiVarMi(entity.Value))
            {
                return entity;
            }
            throw new OctapullException(OctapullExceptions.AuthError);
        }

        /// <summary>
        /// Düzenleme işleminin yapıldığı methodtur.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual Result<TEntity> Update(TEntity entity)
        {
            if (IslemYetkisiVarMi(entity))
            {
                TEntity result = null;
                int saveResult = 0;
                _repository.DataContextConfiguration().AutoDetectChangesEnable();
                var dbItem = _repository.SingleOrDefault(entity.TabloID);
                if (dbItem != null)
                {
                    result = _dataMapper.Map(dbItem, entity);
                    saveResult = _repository.SaveChanges();
                }
                _repository.DataContextConfiguration().AutoDetectChangesDisable();
                if (saveResult > 0)
                    return result.ToResult();
                throw new OctapullException(OctapullExceptions.UpdateError);
            }
            throw new OctapullException(OctapullExceptions.AuthError);
        }

        /// <summary>
        /// Kullanılmayan kaynakları boşa çıkardıktan sonra sonucu true veya false döndüren methodtur.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                _repository.Dispose();
            }
            this._disposed = true;
        }

        /// <summary>
        /// Kullanılmayan kaynakları boşa çıkaran methodtur.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Repositorinin default ayarlarını tanımlar
        /// </summary>
        /// <returns></returns>
        public DataContextConfiguration DataContextConfiguration()
        {
            return _repository.DataContextConfiguration();
        }

        /// <summary>
        /// Geri dönüş biçimi IQueryable olan listeleme metodu
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> AsQuery()
        {
            return _repository.List();
        }

        [ExcludeFromCodeCoverage]
        private /*async*/ bool IslemYetkisiVarMi(TEntity entity)
        {
            if (entity != null)
            {
                var gecicikisiList = _loginUser.YetkiliKisiIdleri;
                var gecicikurumList = _loginUser.YetkiliKurumIdleri;
                switch (entity.GetType().Name)
                {
                    case nameof(CografyaKutuphanesiAyrintilar):
                        {
                            if (gecicikurumList.Any(a => a == entity.KurumID) && gecicikisiList.Any(a => a == entity.KisiID))
                            {
                                return true;
                            }
                            return false;
                        }

                    case nameof(CografyaKutuphanesi):
                        {
                            if (gecicikurumList.Any(a => a == entity.KurumID) && gecicikisiList.Any(a => a == entity.KisiID))
                            {
                                return true;
                            }
                            return false;
                        }

                    case nameof(IcerikKurumsalSablonTanimlari):
                        {
                            if (entity.KurumID == 0)
                            {
                                return true;
                            }
                            if (gecicikurumList.Any(a => a == entity.KurumID) && gecicikisiList.Any(a => a == entity.KisiID))
                            {
                                return true;
                            }
                            return false;
                        }

                    
                    case nameof(KisiTemelBilgiler):
                        {
                            var propKurum = entity.GetType().GetProperty("KisiBagliOlduguKurumId");
                            if (gecicikurumList.Any(a => a == entity.KurumID)
                                && gecicikurumList.Any(a => a == (int?)propKurum.GetValue(entity)))
                            {
                                return true;
                            }
                            return false;
                        }

                    

                    case nameof(MedyaKutuphanesi):
                        {
                            if (gecicikurumList.Any(a => a == entity.KurumID) && gecicikisiList.Any(a => a == entity.KisiID))
                            {
                                return true;
                            }
                            return false;
                        }

                   

                    case string a when a.Contains("Param"):
                        {
                            //if (_loginUser.LisansId == 1031)
                            //{
                            //    return true;
                            //}
                            return true;// return false;
                        }

                    

                    default:
                        return true;
                }
            }
            return true;
        }
    }
}