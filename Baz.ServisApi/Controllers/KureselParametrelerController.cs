using Baz.AletKutusu;
using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Authorization;

using Baz.AOP.Logger.ExceptionLog;
using Baz.Repository.Common;

namespace Baz.IysServiceApi.Controllers
{
    /// <summary>
    /// Küresel parametreler ile ilgili pi contollerı methodlarının yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    [ApiController]
    public class KureselParametrelerController : Controller
    {
        private readonly IParamKureselParametrelerService _paramKureselParametrelerService;
        private readonly ISistemKureselParametreDegerleriService _sistemKureselParametreDegerleriService;
        private readonly List<Type> _paramTypes;
        private readonly List<Type> _paramModelTypes;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Kuresel paramterler api contollerı sınıfının yapıcı methodu
        /// </summary>
        /// <param name="paramKureselParametrelerService"></param>
        /// <param name="sistemKureselParametreDegerleriService"></param>
        /// <param name="serviceProvider"></param>
        public KureselParametrelerController(IParamKureselParametrelerService paramKureselParametrelerService, ISistemKureselParametreDegerleriService sistemKureselParametreDegerleriService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _paramTypes = System.Reflection.Assembly.GetAssembly(typeof(Baz.Service.IParamDillerService)).GetTypes().Where(p => p.IsInterface).ToList();
            _paramModelTypes = System.Reflection.Assembly.GetAssembly(typeof(Baz.Model.Entity.CografyaKutuphanesi)).GetTypes().Where(p => p.IsClass && p.IsPublic && !p.IsInterface && p.Name.ToLower().Contains("param")).ToList();
            _paramKureselParametrelerService = paramKureselParametrelerService;
            _sistemKureselParametreDegerleriService = sistemKureselParametreDegerleriService;
        }

        /// <summary>
        /// İsme göre parametre getirme methodu.
        /// </summary>
        /// <param name="paramTanim">Parametre tanımı.</param>
        /// <returns></returns>
        [Route("IsmeGoreParamGetir")]
        [HttpPost]
        [AllowAnonymous]
        public Result<KureselParametreModel> IsmeGoreParamGetir([FromBody] string paramTanim)
        {
            var result = _sistemKureselParametreDegerleriService.IsmeGoreSistemParamDegeriGetir(paramTanim);
            return result;
        }

        /// <summary>
        /// Kuruma ait parametre değeri tanımlıysa method bunu getirir.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns></returns>
        [Route("KurumParamDegeriTanimliysaGetir")]
        [HttpPost]
        [AllowAnonymous]
        public Result<SistemKureselParametreDegerleri> KurumParamDegeriTanimliysaGetir(KureselParametreModel model)
        {
            var result = _sistemKureselParametreDegerleriService.KurumParamDegeriTanimliysaGetir(model.ParamTanim, model.KurumID);
            return result;
        }

        /// <summary>
        /// Parametre tanımı ekleme ve güncelleme metodu.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns></returns>
        [Route("ParamTanimiEkleGuncelle")]
        [HttpPost]
        public Result<bool> ParamTanimiEkleGuncelle([FromBody] SistemKureselParametreDegeriView model)
        {
            var result = _sistemKureselParametreDegerleriService.ParamTanimiEkleGuncelle(model);
            return result;
        }

        /// <summary>
        /// Kurum Id'ye göre sistem parametre değerlerini listeleyen metod
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        [Route("IdGoreSistemParamDegeriGetir/{kurumID}")]
        [HttpGet]
        public Result<List<SistemKureselParametreDegerleri>> IdGoreSistemParamDegeriListele(int kurumID)
        {
            var result = _sistemKureselParametreDegerleriService.IdGoreSistemParamDegeriListele(kurumID);
            return result;
        }

        /// <summary>
        /// System Parametre Silme metodu tes için
        /// </summary>
        /// <param name="tabloId"></param>
        /// <returns></returns>
        [Route("SystemParamTestDelete/{tabloId}")]
        [HttpGet]
        public Result<SistemKureselParametreDegerleri> SystemParamTestDelete(int tabloId)
        {
            var result = _sistemKureselParametreDegerleriService.Delete(tabloId);
            return result;
        }

        /// <summary>
        /// Tablo Id'ye göre Parametre adını getiren metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [Route("IdGoreParamAdiGetir/{tabloID}")]
        [HttpGet]
        public Result<string> IdGoreParamAdiGetir(int tabloID)
        {
            var result = _paramKureselParametrelerService.SingleOrDefault(tabloID).Value;
            return result.ParamTanim.ToResult();
        }

        /// <summary>
        /// Sms ayarlarını listeleyen metod
        /// </summary>
        /// <returns></returns>
        [Route("SmsAyarlariListele")]
        [HttpGet]
        public Result<SmsAyarlariView> SmsAyarlariListele()
        {
            var result = _sistemKureselParametreDegerleriService.SmsAyarlariListele();
            return result;
        }

        /// <summary>
        /// Organizasyon birimi ekleme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "dsa")]
        [Route("Add")]
        [HttpPost]
        public Result<int> Add(Model.Entity.ViewModel.ParametreRequest request)
        {
            List<ParametreResult> pList = new();
            //Expression<Func<ParamZamanDilimleri, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && !p.SistemParamMi;
            Expression<Func<ParamAdresTipi, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && !p.SistemParamMi;
            //Model ismine göre servisi dinamik olarak getirme
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var modelType = _paramModelTypes.FirstOrDefault(p => p.Name == modelName);

            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            //Servisi çağırma
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }

            var method = service.GetType().GetMethod("ListForFunc");
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResultList = method2.Invoke(service, new[] { newExpression });
            if (serviceResultList != null)
            {
                var value = serviceResultList.GetType().GetProperty("Value").GetValue(serviceResultList);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreResult();
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("ParamTanim"));
                        item.GetType().GetProperty("ParamKod").SetValue(item, ae.Current._GetProperty("ParamKod"));
                        item.GetType().GetProperty("UstId").SetValue(item, ae.Current._GetProperty("UstId"));
                        item.GetType().GetProperty("DilID").SetValue(item, ae.Current._GetProperty("DilID"));
                        item.GetType().GetProperty("EsDilID").SetValue(item, ae.Current._GetProperty("EsDilID"));
                        pList.Add(item);
                    }
                }
            }
            if (pList.Any(x => x.Tanim == request.Tanim))
            {
                throw new OctapullException(OctapullExceptions.DuplicateDataError);
            }
            var modeObj = Activator.CreateInstance(modelType);
            modeObj.GetType().GetProperty("UstId").SetValue(modeObj, request.UstId);
            modeObj.GetType().GetProperty("KurumID").SetValue(modeObj, request.KurumId);
            modeObj.GetType().GetProperty("ParamTanim").SetValue(modeObj, request.Tanim);
            modeObj.GetType().GetProperty("ParamKod").SetValue(modeObj, request.ParamKod);
            modeObj.GetType().GetProperty("DilID").SetValue(modeObj, request.DilID);
            modeObj.GetType().GetProperty("EsDilID").SetValue(modeObj, request.EsDilID);
            modeObj.GetType().GetProperty("AktifMi").SetValue(modeObj, 1);
            modeObj.GetType().GetProperty("GuncellenmeTarihi").SetValue(modeObj, DateTime.Now);
            modeObj.GetType().GetProperty("KayitTarihi").SetValue(modeObj, DateTime.Now);

            //Servisde Add metodunu çağırarak ekleme işlemi yapılması
            var serviceResult = service.GetType().GetMethod("Add").Invoke(service, new[] { modeObj });
            var result = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
            if (request.EsDilID == 0)
            {
                var tabloId = Convert.ToInt32(result.GetType().GetProperty("TabloID").GetValue(result));
                result.GetType().GetProperty("EsDilID").SetValue(result, tabloId);
                var updateResult = service.GetType().GetMethod("Update").Invoke(service, new[] { result });
                var updateValue = updateResult.GetType().GetProperty("Value").GetValue(updateResult);
                return Convert.ToInt32(updateValue.GetType().GetProperty("TabloID").GetValue(updateValue)).ToResult();
            }
            return Convert.ToInt32(result.GetType().GetProperty("TabloID").GetValue(result)).ToResult();
        }

        /// <summary>
        /// Organizasyon birimi güncelleme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "Update")]
        [Route("Update")]
        [HttpPost]
        public Result<int> Update(Model.Entity.ViewModel.ParametreRequest request)
        {
            //Model ismine göre servisi dinamik olarak getirme
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var modelType = _paramModelTypes.FirstOrDefault(p => p.Name == modelName);
            var modeObj = Activator.CreateInstance(modelType);
            modeObj.GetType().GetProperty("TabloID").SetValue(modeObj, request.TabloID);
            modeObj.GetType().GetProperty("KurumID").SetValue(modeObj, request.KurumId);
            modeObj.GetType().GetProperty("UstId").SetValue(modeObj, request.UstId);
            modeObj.GetType().GetProperty("ParamTanim").SetValue(modeObj, request.Tanim);
            modeObj.GetType().GetProperty("ParamKod").SetValue(modeObj, request.ParamKod);
            modeObj.GetType().GetProperty("DilID").SetValue(modeObj, request.DilID);
            modeObj.GetType().GetProperty("EsDilID").SetValue(modeObj, request.EsDilID);
            modeObj.GetType().GetProperty("AktifMi").SetValue(modeObj, 1);
            modeObj.GetType().GetProperty("GuncellenmeTarihi").SetValue(modeObj, DateTime.Now);
            modeObj.GetType().GetProperty("KayitTarihi").SetValue(modeObj, DateTime.Now);
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            //Servisde Update metodunu çağırarak güncelleme işlemi yapılması
            var serviceResult = service.GetType().GetMethod("Update").Invoke(service, new[] { modeObj });
            var result = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
            return Convert.ToInt32(result.GetType().GetProperty("TabloID").GetValue(result)).ToResult();
        }

        /// <summary>
        /// Organizasyon birimi silme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "Delete")]
        [Route("Delete")]
        [HttpPost]
        public Result<int> Delete(Model.Entity.ViewModel.ParametreRequest request)
        {
            //Model ismine göre servisi dinamik olarak getirme
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";

            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            object id = request.TabloID;

            var singleResult = service.GetType().GetMethod("SingleOrDefault").Invoke(service, new[] { id });
            var modeObj = singleResult.GetType().GetProperty("Value").GetValue(singleResult);
            modeObj.GetType().GetProperty("AktifMi").SetValue(modeObj, 0);
            modeObj.GetType().GetProperty("SilindiMi").SetValue(modeObj, 1);
            //Servisde Update metodunu çağırarak güncelleme işlemi yapılması
            var serviceResult = service.GetType().GetMethod("Update").Invoke(service, new[] { modeObj });
            var result = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
            return Convert.ToInt32(result.GetType().GetProperty("TabloID").GetValue(result)).ToResult();
        }

        /// <summary>
        /// Param tanım ve dil Id'ye göre parametreleri listeleme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "List")]
        [Route("List")]
        [HttpPost]
        public Result<List<Baz.Model.Entity.ViewModel.ParametreResult>> List(Model.Entity.ViewModel.ParametreRequest request)
        {

            // diller değerleri varsa getir.

            var sonuc = GetParamDiller(request);
            if (sonuc.IsSuccess)
            {
                var diller = sonuc.Value;
            }
            // Buradaki yöntem değiştirildi. Dil değeri girilmişse buna bakılıyordu.
            // Artık varsayılan dil seçilecek, dil girilmişse ona set edilecek.
            List<Baz.Model.Entity.ViewModel.ParametreResult> result = new();
            Expression<Func<ParamAdresTipi, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && p.DilID == 1 && p.UstId == request.UstId; //varsayılan dili kullanıyoruz.
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var method = service.GetType().GetMethod("ListForFunc");
            //Servisde List metodunu çağırarak listeleme işlemi yapılması
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResult = method2.Invoke(service, new[] { newExpression });
            if (serviceResult != null)
            {
                var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreResult();
                        // EsDilID - Eşleşen Dil ID yöntemi değilti, Eşleşen diller artık farklı tablolardan geliyor. Yeni senaryoda TabloID yine TabloID dir.
                        //item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("EsDilID"));
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("ParamTanim"));
                        item.GetType().GetProperty("ParamKod").SetValue(item, ae.Current._GetProperty("ParamKod"));
                        item.GetType().GetProperty("UstId").SetValue(item, ae.Current._GetProperty("UstId"));
                        item.GetType().GetProperty("DilID").SetValue(item, ae.Current._GetProperty("DilID"));
                        item.GetType().GetProperty("EsDilID").SetValue(item, ae.Current._GetProperty("EsDilID"));

                        if (sonuc.IsSuccess)
                        {
                            if (sonuc.Value != null)
                            {
                                foreach (var ditem in sonuc.Value)
                                {
                                    if (ditem.ParamID == item.TabloID && ditem.ParamDilID == request.DilID && ditem.Tanim != null)
                                    {
                                        item.GetType().GetProperty("Tanim").SetValue(item, ditem.Tanim);
                                        item.GetType().GetProperty("DilID").SetValue(item, request.DilID);
                                    }
                                }
                            }
                        }

                        result.Add(item);
                    }
                }
            }



            /*
            //Model ismine göre servisi dinamik olarak getirme
            List<Baz.Model.Entity.ViewModel.ParametreResult> result = new();
            //Expression<Func<ParamZamanDilimleri, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && p.DilID == request.DilID && p.UstId == request.UstId; //p.KurumID == request.KurumId; 19.11.2020 Behiç abi kurum filtresinin kalkmasını istedi.
            Expression<Func<ParamAdresTipi, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && p.DilID == request.DilID && p.UstId == request.UstId; //p.KurumID == request.KurumId; 19.11.2020 Behiç abi kurum filtresinin kalkmasını istedi.
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var method = service.GetType().GetMethod("ListForFunc");
            //Servisde List metodunu çağırarak listeleme işlemi yapılması
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResult = method2.Invoke(service, new[] { newExpression });
            if (serviceResult != null)
            {
                var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreResult();
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("EsDilID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("ParamTanim"));
                        item.GetType().GetProperty("ParamKod").SetValue(item, ae.Current._GetProperty("ParamKod"));
                        item.GetType().GetProperty("UstId").SetValue(item, ae.Current._GetProperty("UstId"));
                        item.GetType().GetProperty("DilID").SetValue(item, ae.Current._GetProperty("DilID"));
                        item.GetType().GetProperty("EsDilID").SetValue(item, ae.Current._GetProperty("EsDilID"));

                        if (sonuc.IsSuccess)
                        {
                            if (sonuc.Value != null)
                            {
                                foreach(var ditem in sonuc.Value)
                                {
                                    if (ditem.ParamID==item.TabloID && ditem.ParamDilID == item.DilID && ditem.Tanim!=null)
                                    {
                                        item.GetType().GetProperty("Tanim").SetValue(item,ditem.Tanim);
                                    }
                                }
                            }
                        }

                        result.Add(item);
                    }
                }
            }
            */
            return result.ToResult();
        }

        private string GetParamDilValue(String ModelName,int paramId, int dilId, string defaultValue)
        {


            return null;
        }



        /// <summary>
        /// Hakan - Param tablosuna bağlı diller tablosundaki tüm değerleri getirir. Panel için kullanılır
        /// Bu metodun, GetParamDil muadili vardır ve ilgili parametrenin dil değerlerini getirir.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="OctapullException"></exception>
        [ProcessName(Name = "GetParamDiller")]
        [Route("GetParamDiller")]
        [HttpPost]
        public Result<List<Baz.Model.Entity.ViewModel.ParametreDilTanimlar>> GetParamDiller(Model.Entity.ViewModel.ParametreRequest request)
        {
            

            //ParametreDiller result = new ParametreDiller();
            // *********** HAKAN ****************
            //Model ismine göre servisi dinamik olarak getirme
            List<Baz.Model.Entity.ViewModel.ParametreDilTanimlar> result = new();
            Expression<Func<ParamCinsiyetDiller, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0;//&& p.KurumID == request.KurumId;
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "DillerService";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var method = service.GetType().GetMethod("ListForFunc");
            //Servisde List metodunu çağırarak listeleme işlemi yapılması
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResult = method2.Invoke(service, new[] { newExpression });
            if (serviceResult != null)
            {
                var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreDilTanimlar();
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
                        item.GetType().GetProperty("ParamDilID").SetValue(item, ae.Current._GetProperty("ParamDilID"));
                        item.GetType().GetProperty("ParamID").SetValue(item, ae.Current._GetProperty("ParamID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("Tanim"));
                        
                        result.Add(item);
                    }
                }
            }
            return result.ToResult();
            // *********** HAKAN ****************
        }


        [ProcessName(Name = "SaveParamDiller")]
        [Route("SaveParamDiller")]
        [HttpPost]
        public Result<bool> SaveParamDiller(ParametreDiller request)
        {
            // *********** HAKAN ****************
            //Model ismine göre servisi dinamik olarak getirme
            string modelName = request.ModelName + "Diller";
            string servisName = "I" + modelName + "Service";
            var modelType = _paramModelTypes.FirstOrDefault(p => p.Name == modelName);
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            foreach (var item in request.Tanimlar)
            {
                // eğer tanım boşsa o zaman onu silecek odu yazmalıyız.
                /* if (item.Tanim==null || item.Tanim == "")
                {
                    continue;
                } */

                var modeObj = Activator.CreateInstance(modelType);
                modeObj.GetType().GetProperty("KurumID").SetValue(modeObj, request.KurumId);
                modeObj.GetType().GetProperty("ParamID").SetValue(modeObj, item.ParamID);
                modeObj.GetType().GetProperty("ParamDilID").SetValue(modeObj, item.ParamDilID);
                modeObj.GetType().GetProperty("Tanim").SetValue(modeObj, item.Tanim);
                modeObj.GetType().GetProperty("AktifMi").SetValue(modeObj, 1);
                modeObj.GetType().GetProperty("GuncellenmeTarihi").SetValue(modeObj, DateTime.Now);
                modeObj.GetType().GetProperty("KayitTarihi").SetValue(modeObj, DateTime.Now);
                //Servisde Update metodunu çağırarak güncelleme işlemi yapılması

                if (item.TabloID == 0)
                {
                    var serviceResult = service.GetType().GetMethod("Add").Invoke(service, new[] { modeObj });
                    var result = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                }
                else
                {
                    modeObj.GetType().GetProperty("TabloID").SetValue(modeObj, item.TabloID);
                    var serviceResult = service.GetType().GetMethod("Update").Invoke(service, new[] { modeObj });
                    var result = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);

                    
                }
            }

            _paramKureselParametrelerService.DataContextConfiguration().Commit();


            //Servisde Update metodunu çağırarak güncelleme işlemi yapılması
            return true.ToResult();
            // *********** HAKAN ****************
        }


        [ProcessName(Name = "GetParamDil")]
        [Route("GetParamDil")]
        [HttpPost]
        public Result<List<Baz.Model.Entity.ViewModel.ParametreDilTanimlar>> GetParamDil(ParamDilRequest request)
        {
            // Buradaki parametreler Modele Dönüştürülecek.
            //ParametreDiller result = new ParametreDiller();
            // *********** HAKAN ****************
            //Model ismine göre servisi dinamik olarak getirme
            List<Baz.Model.Entity.ViewModel.ParametreDilTanimlar> result = new();
            Expression<Func<ParamCinsiyetDiller, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && p.KurumID == request.KurumID && p.ParamID == request.ParamID;
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "DillerService";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var method = service.GetType().GetMethod("ListForFunc");
            //Servisde List metodunu çağırarak listeleme işlemi yapılması
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResult = method2.Invoke(service, new[] { newExpression });
            if (serviceResult != null)
            {
                var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreDilTanimlar();
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
                        item.GetType().GetProperty("ParamDilID").SetValue(item, ae.Current._GetProperty("ParamDilId"));
                        item.GetType().GetProperty("ParamID").SetValue(item, ae.Current._GetProperty("ParamID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("Tanim"));
                        result.Add(item);
                    }
                }
            }
            return result.ToResult();
            // *********** HAKAN ****************
        }


        /// <summary>
        /// Param tanım ve dil Id'ye göre parametreleri listeleme methodu.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "ListParam")]
        [Route("ListParam")]
        [HttpPost]
        public Result<List<Baz.Model.Entity.ViewModel.ParametreResult>> ListParam(Model.Entity.ViewModel.ParametreRequest request)
        {
            //Model ismine göre servisi dinamik olarak getirme
            List<Baz.Model.Entity.ViewModel.ParametreResult> result = new();
            //Expression<Func<ParamZamanDilimleri, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && !p.SistemParamMi;
            Expression<Func<ParamAdresTipi, bool>> expression = p => p.AktifMi == 1 && p.SilindiMi == 0 && !p.SistemParamMi;
            string modelName = request.ModelName;
            string servisName = "I" + modelName + "Service";
            var serviceType = _paramTypes.FirstOrDefault(p => p.Name == servisName);
            if (serviceType == null)
            {
                return null;
                //throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var service = _serviceProvider.GetService(serviceType);
            if (service == null)
            {
                return null;
                //throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            //Servisde List metodunu çağırarak listeleme işlemi yapılması
            var method = service.GetType().GetMethod("ListForFunc");
            var method2 = service.GetType().GetMethods().Where(p => p.Name == "List").ToList()[1];
            var parametreType = method.GetParameters()[0].ParameterType;
            var newExpression = expression.ConvertImpl(parametreType);
            var serviceResult = method2.Invoke(service, new[] { newExpression });
            if (serviceResult != null)
            {
                var value = serviceResult.GetType().GetProperty("Value").GetValue(serviceResult);
                if (value != null)
                {
                    var ae = (value as IEnumerable).GetEnumerator();
                    while (ae.MoveNext())
                    {
                        var item = new ParametreResult();
                        item.GetType().GetProperty("TabloID").SetValue(item, ae.Current._GetProperty("TabloID"));
                        item.GetType().GetProperty("Tanim").SetValue(item, ae.Current._GetProperty("ParamTanim"));
                        item.GetType().GetProperty("ParamKod").SetValue(item, ae.Current._GetProperty("ParamKod"));
                        item.GetType().GetProperty("UstId").SetValue(item, ae.Current._GetProperty("UstId"));
                        item.GetType().GetProperty("DilID").SetValue(item, ae.Current._GetProperty("DilID"));
                        item.GetType().GetProperty("EsDilID").SetValue(item, ae.Current._GetProperty("EsDilID"));
                        result.Add(item);
                    }
                }
            }
            return result.ToResult();
        }
    }
}