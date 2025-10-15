using Baz.AOP.Logger.ExceptionLog;
using Baz.Mapper.Pattern;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.Repository.Pattern;
using Baz.Service.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Baz.Service
{
    /// <summary>
    /// Sistem Küresel Parametre Değerleri servisi için gerekli methodların yer aldığı sınıf.
    /// </summary>
    public interface ISistemKureselParametreDegerleriService : IService<SistemKureselParametreDegerleri>
    {
        /// <summary>
        /// Kurum için parametre değeri tanımlıysa getiren metod
        /// </summary>
        /// <param name="paramTanim">paramTanim.</param>
        /// <param name="kurumID">kurumID.</param>
        /// <returns></returns>
        Result<SistemKureselParametreDegerleri> KurumParamDegeriTanimliysaGetir(string paramTanim, int kurumID);

        /// <summary>
        /// Parametre tanımı ekleme methodu.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns></returns>
        Result<bool> ParamTanimiEkleGuncelle(SistemKureselParametreDegeriView model);

        /// <summary>
        /// İsme göre sistem parametre değeri getirme methodu.
        /// </summary>
        /// <param name="paramTanim">paramTanim.</param>
        /// <returns></returns>
        Result<KureselParametreModel> IsmeGoreSistemParamDegeriGetir(string paramTanim);

        /// <summary>
        /// Kurum Id'ye göre sistem parametre değerlerini listeleyen metod
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        Result<List<SistemKureselParametreDegerleri>> IdGoreSistemParamDegeriListele(int kurumID);

        /// <summary>
        /// Sms Ayarlarını listeleyen metod
        /// </summary>
        /// <returns></returns>
        Result<SmsAyarlariView> SmsAyarlariListele();
    }

    /// <summary>
    /// SistemKureselParametreDegerleri ile ilgili işlemleri yöneten servıs sınıfı
    /// </summary>
    internal class SistemKureselParametreDegerleriService : Service<SistemKureselParametreDegerleri>, ISistemKureselParametreDegerleriService
    {
        private readonly IParamKureselParametrelerService _paramKureselParametrelerService;

        /// <summary>
        /// SistemKureselParametreDegerleri ile ilgili işlemleri yöneten servıs sınıfının yapıcı metodu
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="dataMapper"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="paramKureselParametrelerService"></param>
        public SistemKureselParametreDegerleriService(IRepository<SistemKureselParametreDegerleri> repository, IDataMapper dataMapper, IServiceProvider serviceProvider, ILogger<SistemKureselParametreDegerleriService> logger, IParamKureselParametrelerService paramKureselParametrelerService) : base(repository, dataMapper, serviceProvider, logger)
        {
            _paramKureselParametrelerService = paramKureselParametrelerService;
        }

        /// <summary>
        /// Kurum için parametre değeri tanımlıysa getiren metod
        /// </summary>
        /// <param name="paramTanim">paramTanim.</param>
        /// <param name="kurumID">kurumID.</param>
        /// <returns></returns>

        public Result<SistemKureselParametreDegerleri> KurumParamDegeriTanimliysaGetir(string paramTanim, int kurumID)
        {
            var returnModel = new SistemKureselParametreDegerleri();
            var paramTanimModel = _paramKureselParametrelerService.IsmeGoreParamGetir(paramTanim).Value;
            returnModel = List(p => p.KurumID == kurumID && p.KureselParametreId == paramTanimModel.KureselParametreId).Value.FirstOrDefault();
             return returnModel.ToResult();
            
        }

        /// <summary>
        /// Parametre tanımı ekleme methodu.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <returns></returns>

        public Result<bool> ParamTanimiEkleGuncelle(SistemKureselParametreDegeriView model)
        {
            //Kurum için göre kürsel parametre değerleri ekleme güncelleme
            if (model.KurumID > 0)
            {
                foreach (var md in model.KureselParams)
                {
                    if (md.Adi != null || md.Adi != "")
                    {
                        var kureselParamID = _paramKureselParametrelerService.IsmeGoreParamGetir2(md.Adi);
                        if (md.ID == 0)
                        {
                            var b = new SistemKureselParametreDegerleri()
                            {
                                KurumID = model.KurumID,
                                ParametreKurumsalMiSistemMi = 2,
                                KureselParametreId = kureselParamID.Value.KureselParametreId,
                                ParametreBaslangicDegeri = md.Deger,

                                AktifMi = 1,
                                KayitTarihi = DateTime.Now,
                                GuncellenmeTarihi = DateTime.Now
                            };
                            Add(b);
                        }
                        else
                        {
                            var c = new SistemKureselParametreDegerleri()
                            {
                                TabloID = md.ID,
                                KurumID = model.KurumID,
                                ParametreKurumsalMiSistemMi = 2,
                                KureselParametreId = kureselParamID.Value.KureselParametreId,
                                ParametreBaslangicDegeri = md.Deger,

                                AktifMi = 1,
                                KayitTarihi = DateTime.Now,
                                GuncellenmeTarihi = DateTime.Now
                            };

                            Update(c);
                        }
                    }
                }
            }
            //Sistem için göre küresel parametre değerleri ekleme güncelleme
            else
            {
                foreach (var md in model.KureselParams)
                {
                    if (md.Adi != null || md.Adi != "")
                    {
                        var kureselParamID = _paramKureselParametrelerService.IsmeGoreParamGetir2(md.Adi);
                        if (md.ID == 0)
                        {
                            var b = new SistemKureselParametreDegerleri()
                            {
                                KurumID = model.KurumID,
                                ParametreKurumsalMiSistemMi = 1,
                                KureselParametreId = kureselParamID.Value.KureselParametreId,
                                ParametreBaslangicDegeri = md.Deger,
                                ParametreMetinDegeri = md.MetinDegeri,

                                AktifMi = 1,
                                KayitTarihi = DateTime.Now,
                                GuncellenmeTarihi = DateTime.Now
                            };
                            Add(b);
                        }
                        else
                        {
                            var c = new SistemKureselParametreDegerleri()
                            {
                                TabloID = md.ID,
                                KurumID = model.KurumID,
                                ParametreKurumsalMiSistemMi = 1,
                                KureselParametreId = kureselParamID.Value.KureselParametreId,
                                ParametreBaslangicDegeri = md.Deger,
                                ParametreMetinDegeri = md.MetinDegeri,

                                AktifMi = 1,
                                KayitTarihi = DateTime.Now,
                                GuncellenmeTarihi = DateTime.Now
                            };
                            Update(c);
                        }
                    }
                }
            }
            return true.ToResult();
        }

        /// <summary>
        /// İsme göre sistem parametre değeri getirme methodu.
        /// </summary>
        /// <param name="paramTanim">paramTanim.</param>
        /// <returns></returns>

        public Result<KureselParametreModel> IsmeGoreSistemParamDegeriGetir(string paramTanim)
        {
            var paramModel = _paramKureselParametrelerService.IsmeGoreParamGetir(paramTanim).Value;
            var result = List(f => f.KureselParametreId == paramModel.KureselParametreId && f.KurumID == 0).Value.FirstOrDefault();
            paramModel.ParametreBaslangicDegeri = result.ParametreBaslangicDegeri;
            paramModel.ParametreBitisDegeri = result.ParametreBitisDegeri;
            paramModel.ParametreMetinDegeri = result.ParametreMetinDegeri;
            paramModel.ParametreTarihBaslangicDegeri = result.ParametreTarihBaslangicDegeri;
            paramModel.ParametreTarihBitisDegeri = result.ParametreTarihBitisDegeri;
            paramModel.ParametreKurumsalMiSistemMi = result.ParametreKurumsalMiSistemMi;
            return paramModel.ToResult();
        }

        /// <summary>
        /// Kurum Id'ye göre sistem parametre değerlerini listeleyen metod
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>

        public Result<List<SistemKureselParametreDegerleri>> IdGoreSistemParamDegeriListele(int kurumID)
        {
            var x = List(t => t.KurumID == kurumID && t.AktifMi == 1);
            return x;
        }

        /// <summary>
        /// Sms Ayarlarını listeleyen metod
        /// </summary>
        /// <returns></returns>
        public Result<SmsAyarlariView> SmsAyarlariListele()
        {
            var x = IdGoreSistemParamDegeriListele(0).Value;
            var sms = new SmsAyarlariView();
            foreach (var a in x)
            {
                var m = _paramKureselParametrelerService.SingleOrDefault(Convert.ToInt32(a.KureselParametreId)).Value.ParamTanim;
                if (m == "SmsBasligi")
                {
                    sms.SmsBasligi = a.ParametreMetinDegeri;
                }
                else if (m == "SmsApiUrl")
                {
                    sms.SmsApiUrl = a.ParametreMetinDegeri;
                }
                else if (m == "SmsApiKullaniciAdi")
                {
                    sms.SmsApiKullaniciAdi = a.ParametreMetinDegeri;
                }

                sms.SmsApiSifresi = a.ParametreMetinDegeri;
            }

            return sms.ToResult();
        }
    }
}