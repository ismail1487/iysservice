using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// Genel parametre kontrol test methodlarının bulunduğu sınıftır.
    /// Listeleme ve silme metodları için olumsuz test senaryolarına gerek duyulmamıştır.
    /// </summary>
    [TestClass()]
    public class GenelParametrelerTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Genel parametre kontrol test methodlarının bulunduğu sınıfın yapıcı metodu
        /// </summary>
        public GenelParametrelerTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// GenelParamLists Testi
        /// </summary>
        [TestMethod()]
        public void GenelParamListsTests()
        {
            // Assert Dillerin Listelendiği test
            var dillist
                = _helper.Get<Result<List<ParamDiller>>>($"/api/GenelParametreler/DilList");
            Assert.AreEqual(dillist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(dillist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(dillist.Result);

            // Assert Cinsiyetin Listelendiği test
            var cinsiyetlist = _helper.Get<Result<List<ParamCinsiyet>>>($"/api/GenelParametreler/CinsiyetList");
            Assert.AreEqual(cinsiyetlist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(cinsiyetlist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(cinsiyetlist.Result);

            // Assert Ülkelerin Listelendiği test
            var ulkelist = _helper.Get<Result<List<ParamUlkeler>>>($"/api/GenelParametreler/UlkeList");
            Assert.AreEqual(ulkelist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(ulkelist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(ulkelist.Result);

            var sehirlist = _helper.Get<Result<List<ParamUlkeler>>>($"/api/GenelParametreler/SehirList/1");
            Assert.AreEqual(sehirlist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(sehirlist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(sehirlist.Result);

            // Assert Medeni Halin Listelendiği test
            var medenihallist = _helper.Get<Result<List<ParamMedeniHal>>>($"/api/GenelParametreler/MedeniHalList");
            Assert.AreEqual(medenihallist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(medenihallist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(medenihallist.Result);

            // Assert Adres Tiplerinin Listelendiği test
            var adrestipilist = _helper.Get<Result<List<ParamAdresTipi>>>($"/api/GenelParametreler/AdresTipiList");
            Assert.AreEqual(adrestipilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(adrestipilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(adrestipilist.Result);

            // Assert Okul Tiplerinin Listelendiği test
            var okultipilist = _helper.Get<Result<List<ParamOkulTipi>>>($"/api/GenelParametreler/OkulTipiList");
            Assert.AreEqual(okultipilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(okultipilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(okultipilist.Result);

            // Assert Telefon Tiplerinin Listelendiği test
            var telefontipilist =
                _helper.Get<Result<List<ParamTelefonTipi>>>($"/api/GenelParametreler/TelefonTipiList");
            Assert.AreEqual(telefontipilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(telefontipilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(telefontipilist.Result);

            // Assert Dinlerin listelendiği test
            var dinlerlist = _helper.Get<Result<List<ParamDinler>>>($"/api/GenelParametreler/DinlerList");
            Assert.AreEqual(dinlerlist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(dinlerlist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(dinlerlist.Result);

            // Assert Çalışan sayılarının listelendiği test
            var calisansayisilist =
                _helper.Get<Result<List<ParamCalisanSayilari>>>($"/api/GenelParametreler/CalisanSayilariList");
            Assert.AreEqual(calisansayisilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(calisansayisilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(calisansayisilist.Result);

            // Assert Kurum lokasyon tiplerinin listelendiği test
            var lokasyontipilist =
                _helper.Get<Result<List<ParamKurumLokasyonTipi>>>($"/api/GenelParametreler/KurumLokasyonTipiList");
            Assert.AreEqual(lokasyontipilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(lokasyontipilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(lokasyontipilist.Result);


            // Assert Postacı işlem durum tiplerinin listelendiği test
            var postaciislemdurumtipilist =
                _helper.Get<Result<List<ParamPostaciIslemDurumTipleri>>>(
                    $"/api/GenelParametreler/PostaciIslemDurumTipleriList");
            Assert.AreEqual(postaciislemdurumtipilist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(postaciislemdurumtipilist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(postaciislemdurumtipilist.Result);

            // Assert Ülke Şehir ve lokasyan tipi parametrelerinin getirildiği test
            var getparametrenames = _helper.Post<Result<KurumTemelKayitModel>>(
                $"/api/GenelParametreler/GetParametreNames", new KurumTemelKayitModel
                {
                    AdresListesi = new List<KurumAdresModel>
                    {
                        new KurumAdresModel
                        {
                            Ulke = 1,
                            Sehir = 5,
                            UlkeAdi = "Türkiye",
                            SehirAdi = "İstanbul",
                            LokasyonTipi = 1,
                            LokasyonAdi = "Genel Müdürlük"
                        }
                    },
                    KurumVergiDairesiId = "test",
                    VergiDairesiAdi = "Tuna Vergi Dairesi",
                    KurumAdi = "Orsa",
                    EpostaAdresi = "bilalkose1is@gmail.com"
                });
            Assert.AreEqual(getparametrenames.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getparametrenames.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getparametrenames.Result);

            // Assert Ülke Şehir ve lokasyan tipi parametrelerinin getirildiği test2
            var getparametrenames2 = _helper.Post<Result<KurumTemelKayitModel>>(
                $"/api/GenelParametreler/GetParametreNames", new KurumTemelKayitModel
                {
                    AdresListesi = new List<KurumAdresModel>
                    {
                        new KurumAdresModel
                        {
                            Ulke = 1,
                            Sehir = 5,
                            UlkeAdi = "Türkiye",
                            SehirAdi = "İstanbul",
                            LokasyonTipi = 1,
                            LokasyonAdi = "Genel Müdürlük"
                        }
                    },
                    KurumVergiDairesiId = "test",
                    VergiDairesiAdi = "Tuna Vergi Dairesi",
                    KurumAdi = "Test Kurumu",
                    EpostaAdresi = "b@mail.com",
                    KurumUlkeId = 1,
                    KurumSehirId = 5,
                    BankaListesi = new List<KurumBankaModel>()
                    {
                        new KurumBankaModel()
                        {
                            BankaId = 110,
                            SubeId = 110
                        }
                    }
                });
            Assert.AreEqual(getparametrenames2.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getparametrenames2.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getparametrenames2.Result);

            // Assert Ülke Şehir ve lokasyan tipi parametrelerinin getirildiği test3
            var getparametrenames3 = _helper.Post<Result<KurumTemelKayitModel>>(
                $"/api/GenelParametreler/GetParametreNames", new KurumTemelKayitModel
                {
                    AdresListesi = new List<KurumAdresModel>
                    {
                        new KurumAdresModel
                        {
                            Ulke = 1,
                            Sehir = 5,
                            UlkeAdi = "Türkiye",
                            SehirAdi = "İstanbul",
                            LokasyonTipi = 1,
                            LokasyonAdi = "Genel Müdürlük"
                        }
                    },
                    KurumVergiDairesiId = "test",
                    VergiDairesiAdi = "Tuna Vergi Dairesi",
                    KurumAdi = "Test Kurumu",
                    EpostaAdresi = "b@mail.com",
                    KurumUlkeId = 100,
                    KurumSehirId = 5,
                    BankaListesi = new List<KurumBankaModel>()
                    {
                        new KurumBankaModel()
                        {
                            BankaId = 1,
                            SubeId = 2
                        }
                    }
                });
            Assert.AreEqual(getparametrenames3.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getparametrenames3.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getparametrenames3.Result);

            // Assert Kurum idari parametre profillerinin getirildiği metod test
            var GetIdariParametreNames = _helper.Post<Result<KurumIdariProfilModel>>(
                $"/api/GenelParametreler/GetIdariParametreNames", new KurumIdariProfilModel
                {
                    KurumKisiLisansListesi = new()
                    {
                        new KurumKisiLisansBilgileri()
                        {
                            LisansAboneKisiId = 130,
                            LisansGenelTanimId = 24,
                        }
                    },
                    KurumBankaListesi = new()
                    {
                        new KurumIdariBankaModel()
                        {
                            BankaId = 1,
                            SubeId = 2
                        },
                        new KurumIdariBankaModel()
                        {
                            BankaId = 100,
                            SubeId = 2
                        }
                    },
                    KurumAdi = "Orsa",
                    EpostaAdresi = "b@mail.com",
                });
            Assert.AreEqual(GetIdariParametreNames.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(GetIdariParametreNames.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(GetIdariParametreNames.Result);

            // Assert Kurum ilişki türlerinin listelendiği test
            var iliskiturulist =
                _helper.Get<Result<List<ParamIliskiTurleri>>>($"/api/GenelParametreler/KodaGoreIliskiTurleri/F");
            Assert.AreEqual(iliskiturulist.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(iliskiturulist.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(iliskiturulist.Result);

            // Assert  Param Tanım ve Dil Id'ye Parametreleri listeleme test methodu
            var listP = _helper.Post<Result<List<Baz.Model.Entity.ViewModel.ParametreResult>>>(
                $"/api/GenelParametreler/List", new ParametreRequest
                {
                    ModelName = "ParamDinler",
                    DilID = 1,
                    Tanim = "sdf"
                });
            Assert.AreEqual(listP.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(listP.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(listP.Result);
        }

    }
}