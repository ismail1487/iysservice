using Baz.IysServiceApi.Controllers;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Baz.Model.Entity.Constants;
using Baz.RequestManager;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// Küresel Parametrelerin test methodlarının olduğu sınıftır.
    /// Listeleme ve silme metodları için olumsuz test senaryolarına gerek duyulmamıştır.
    /// </summary>
    [TestClass()]
    public class KureselParametrelerTest
    {
        private readonly IRequestHelper _helper;
        private readonly IRequestHelper _helper3;

        /// <summary>
        /// Küresel Parametrelerin test methodlarının olduğu sınıfın yapıcı metodu
        /// </summary>
        public KureselParametrelerTest()
        {
            _helper = TestServerRequestHelper.CreateHelper();
            _helper3 = TestServerRequestHelper.CreateHelper2(new TestLoginUserManager(0, 0));
        }

        #region OrganizasyonBirimTests

        /// <summary>
        /// Organizasyon birim testi
        /// </summary>
        [TestMethod]
        public void OrganizasyonBirimTests()
        {
            //Assert AddTest Organizasyon birim ekleme test methodu
            var addT = _helper.Post<Result<int>>($"/api/KureselParametreler/Add", new ParametreRequest
            {
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "TestDin",
                UstId = 0
            });
            Assert.AreEqual(addT.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(addT.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(addT.Result);

            //Assert UpdatePTest Organizasyon birim güncelleme test methodu
            var updateP = _helper.Post<Result<int>>($"/api/KureselParametreler/Update", new ParametreRequest
            {
                TabloID = addT.Result.Value,
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "DinTest",
                UstId = 0
            });
            Assert.AreEqual(updateP.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(updateP.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(updateP.Result);

            //Assert DeletePTest Organizasyon birim silme test methodu
            var deleteP = _helper.Post<Result<int>>($"/api/KureselParametreler/Delete", new ParametreRequest
            {
                TabloID = addT.Result.Value,
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "test"
            });
            Assert.AreEqual(deleteP.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(deleteP.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(deleteP.Result);
        }

        #endregion OrganizasyonBirimTests

        #region SystemCompanyCrudTest

        /// <summary>
        /// SistemKureselParametreDegerler CRUD test metodu
        /// </summary>
        [TestMethod()]
        public void SystemCompanyCrudTest()
        {
            // Kullanılacak Model
            var model = new SistemKureselParametreDegeriView()
            {
                KurumID = 82,
                SistemMi = 2,
                KureselParams = new List<ParamKureselParam>()
                {
                    new ParamKureselParam()
                    {
                        ID = 29,
                        Adi = "Session Timeout",
                        Deger = 12200
                    }
                }
            };

            //defaultdegerler Modele göre
            var modellist = new List<ParamKureselParam>();
            foreach (var item in model.KureselParams)
            {
                var x = _helper.Post<Result<KureselParametreModel>>("/api/KureselParametreler/IsmeGoreParamGetir", item.Adi).Result.Value;
                var y = new ParamKureselParam()
                {
                    Adi = x.ParamTanim,
                    Deger = x.ParametreBaslangicDegeri.Value,
                    MetinDegeri = x.ParametreMetinDegeri
                };
                modellist.Add(y);
            }

            //Assert-1 Ekle
            var add = _helper.Post<Result<bool>>("/api/KureselParametreler/ParamTanimiEkleGuncelle", model);
            Assert.AreEqual(add.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(add.Result.Value);

            //Assert-2 Listele KurumID
            var list = _helper.Get<Result<List<SistemKureselParametreDegerleri>>>("/api/KureselParametreler/IdGoreSistemParamDegeriGetir/" + 0);
            Assert.AreEqual(list.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(list.Result.Value);

            //Assert-3 Id'ye göre ismi getiren test metodu
            var GetAd = new List<string>();
            foreach (var ls in list.Result.Value)
            {
                GetAd.Add(_helper.Get<Result<string>>("/api/KureselParametreler/IdGoreParamAdiGetir/" + ls.KureselParametreId).Result.Value);
            }
            Assert.IsNotNull(GetAd);

            //Assert-3.1 Id'ye göre ismi getiren olumsuz test metodu
            var GetAdNegative = new List<string>();
            foreach (var _ in list.Result.Value)
            {
                GetAdNegative.Add(_helper.Get<Result<string>>("/api/KureselParametreler/IdGoreParamAdiGetir/" + 0).Result.Value);
            }
            Assert.IsNull(GetAdNegative[0]);

            //Assert-4 Sms Ayarları getir
            var sms = _helper.Get<Result<SmsAyarlariView>>("/api/KureselParametreler/SmsAyarlariListele");
            Assert.AreEqual(sms.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(sms.Result.Value);

            //Assert-5 Kurum List
            var kurumlist = _helper.Post<Result<SistemKureselParametreDegerleri>>("/api/KureselParametreler/KurumParamDegeriTanimliysaGetir", new KureselParametreModel()
            {
                KurumID = 0,
                ParamTanim = "Session Timeout"
            });
            Assert.AreEqual(kurumlist.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(kurumlist.Result.Value);

            //Assert-6
            var paramdeg = new List<KureselParametreModel>();
            foreach (var ad in GetAd)
            {
                paramdeg.Add(_helper.Post<Result<KureselParametreModel>>("/api/KureselParametreler/IsmeGoreParamGetir", ad).Result.Value);
            }
            Assert.IsNotNull(paramdeg);

            //Assert 7 Default Degerlere Çevirme
            var toDefaultModel = new SistemKureselParametreDegeriView()
            {
                KurumID = model.KurumID,
                SistemMi = model.SistemMi,
                KureselParams = model.KureselParams
            };
            var update = _helper.Post<Result<bool>>("/api/KureselParametreler/ParamTanimiEkleGuncelle", toDefaultModel);
            Assert.AreEqual(update.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(update.Result.Value);

            var model2 = new SistemKureselParametreDegeriView()
            {
                KurumID = 82,
                SistemMi = 2,
                KureselParams = new List<ParamKureselParam>()
                {
                    new ParamKureselParam()
                    {
                        ID = 0,
                        Adi = "Session Timeout",
                        Deger = 12200
                    }
                }
            };
            var add2 = _helper.Post<Result<bool>>("/api/KureselParametreler/ParamTanimiEkleGuncelle", model2);
            Assert.AreEqual(add2.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(add2.Result.Value);

            var list2 = _helper.Get<Result<List<SistemKureselParametreDegerleri>>>("/api/KureselParametreler/IdGoreSistemParamDegeriGetir/" + model2.KurumID);
            var id1 = list2.Result.Value.Last().TabloID;

            var delete1 = _helper.Get<Result<List<SistemKureselParametreDegerleri>>>("/api/KureselParametreler/SystemParamTestDelete/" + id1);
            Assert.AreEqual(delete1.StatusCode, HttpStatusCode.OK);

            var model3 = new SistemKureselParametreDegeriView()
            {
                KurumID = 0,
                SistemMi = 1,
                KureselParams = new List<ParamKureselParam>()
                {
                    new ParamKureselParam()
                    {
                        ID = 0,
                        Adi = "Session Timeout",
                        Deger = 12200
                    }
                }
            };
            var add3 = _helper3.Post<Result<bool>>("/api/KureselParametreler/ParamTanimiEkleGuncelle", model3);
            Assert.AreEqual(add3.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(add3.Result.Value);

            var list3 = _helper.Get<Result<List<SistemKureselParametreDegerleri>>>("/api/KureselParametreler/IdGoreSistemParamDegeriGetir/" + 0);
            var id2 = list3.Result.Value.Last().TabloID;

            model3.KureselParams[0].ID = id2;
            model3.KureselParams[0].Deger = 12211;
            var toDefaultModel2 = new SistemKureselParametreDegeriView()
            {
                KurumID = model3.KurumID,
                SistemMi = model3.SistemMi,
                KureselParams = model3.KureselParams
            };
            var update2 = _helper3.Post<Result<bool>>("/api/KureselParametreler/ParamTanimiEkleGuncelle", toDefaultModel2);
            Assert.AreEqual(update2.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(update2.Result.Value);

            var delete2 = _helper.Get<Result<List<SistemKureselParametreDegerleri>>>("/api/KureselParametreler/SystemParamTestDelete/" + id2);
            Assert.AreEqual(delete2.StatusCode, HttpStatusCode.OK);
        }

        #endregion SystemCompanyCrudTest

        /// <summary>
        /// Parametre ismine göre listeleme testi
        /// </summary>
        [TestMethod()]
        public void List()
        {
            var list = _helper.Post<Result<List<ParametreResult>>>($"/api/KureselParametreler/List", new ParametreRequest()
            {
                ModelName = "ParamDinler",
                DilID = 1,
                EsDilID = 1,
                Tanim = "test"
            });
            Assert.AreEqual(list.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(list.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Result);
        }

        /// <summary>
        ///Parametre ismine göre listeleme testi
        /// </summary>
        [TestMethod()]
        public void ListParam()
        {
            var listParam = _helper.Post<Result<List<ParametreResult>>>($"/api/KureselParametreler/ListParam", new ParametreRequest()
            {
                ModelName = "ParamDinler",
                DilID = 1,
                EsDilID = 1,
                Tanim = "test"
            });
            Assert.AreEqual(listParam.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(listParam.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(listParam.Result);
        }

        /// <summary>
        ///Tüm parametreler servislerinin testi
        /// </summary>
        [TestMethod]
        public void ListParamAllServices()
        {
            var paramNamesList = new List<string>() { "ParamAdresTipi"
,"ParamAksiyonGerceklesmeSafhalari"
,"ParamAksiyonOnayStatuleri"
,"ParamAksiyonTipleri"
,"ParamBankalar"
,"ParamCalisanSayilari"
,"ParamCinsiyet"
,"ParamDinler"
,"ParamDisSistemTokenTipleri"
,"ParamGorusmeCanliNotIkonlari"
,"ParamGorusmeKatilimCihazTipleri"
,"ParamGorusmeOdasiAyrilisNedenleri"
,"ParamGorusmeOdasiDavetleriKabulDurumlari"
,"ParamGorusmeOdasiEklentileri"
,"ParamGorusmeOdasiKategorileri"
,"ParamGorusmeOdasiRolleri"
,"ParamGorusmeOdasiTipleri"
,"ParamGorusmeOdasiTrafikParametreleri"
,"ParamHesapDeAktifEtmeSebebleriKisisel"
,"ParamHesapDeAktifEtmeSebepleriKurumsal"
,"ParamIcerikKategorileri"
,"ParamIcerikSablonTipleri"
,"ParamIcerikTipleri"
,"ParamIliskiTurleri"
,"ParamKimlikTipleri"
,"ParamKureselParametreler"
,"ParamKurumBelgeTipleri"
,"ParamKurumSektorleri"
,"ParamKurumTipleri"
,"ParamLisansVektorTipleri"
,"ParamLogHataKayitTipleri"
,"ParamLokasyonTipleri"
,"ParamMedeniHal"
,"ParamMedyaTipleri"
,"ParamMolaTipleri"
,"ParamOkulTipi"
,"ParamOlcumBirimleri"
,"ParamOrganizasyonBirimleri"
,"ParamParaBirimleri"
,"ParamPostaciIslemDurumTipleri"
,"ParamSanalPosdurumStatuleri"
,"ParamSanalPosNoktalari"
,"ParamSistemKisiEtkilesimTipleri"
,"ParamSistemModulleri"
,"ParamTakvimDilimlendirmeGunTipleri"
,"ParamTakvimDilimTipleri"
,"ParamTarayiciTipleri"
,"ParamTekrarlamaPeriyodlari"
,"ParamTelefonTipi"
,"ParamUlkeler"
,"ParamVergiDaireleri"
,"ParamYabanciDiller"
,"ParamZamanDilimleri"
,"ParamKurumLokasyonTipi"
,"ParamTakvimOzelGunTipleri"
,"ParamUrunKategorileri"
,"ParamOlcumKategorileri"
,"ParamUrunMarkalar"
,"ParamUrunModelleri"
,"ParamUrunSerileri"};
            foreach (var param in paramNamesList)
            {
                var listParam = _helper.Post<Result<List<ParametreResult>>>($"/api/KureselParametreler/ListParam", new ParametreRequest()
                {
                    ModelName = param,
                    DilID = 1,
                    EsDilID = 1,
                    Tanim = "test"
                });
                Assert.AreEqual(listParam.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(listParam.Result.StatusCode, (int)ResultStatusCode.Success);
                Assert.IsNotNull(listParam.Result);
            }
        }
    }
}