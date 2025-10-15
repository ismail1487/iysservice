using Baz.IysServiceApi.Controllers;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// IcerikKurumsalSablonTanimlari test classı
    /// </summary>
    [TestClass]
    public class IcerikKurumsalSablonTanimlariTests
    {
        private readonly IRequestHelper _helper;
        private readonly IRequestHelper _globalHelper;

        /// <summary>
        /// IcerikKurumsalSablonTanimlari test classı yapıcı metodu
        /// </summary>
        public IcerikKurumsalSablonTanimlariTests()
        {
            _globalHelper = TestServerRequestHelperNoHeader.CreateHelper();
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// IcerikKurumsalSablonTanimlari CRUD testi
        /// </summary>
        [TestMethod]
        public void IcerikKurumsalSablonTanimlariTestsCrud()
        {
            //Assert negativeAdd
            var negativeAdd = _globalHelper.Post<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Add", new IcerikKurumsalSablonTanimlari
                {
                    IcerikTamMetin = "Test metni",
                    KurumID = 82,
                    AktifMi = 1,
                    IcerikBaslik = "Test başlık",
                    IcerikTanim = "Test icerik tanım",
                });

            Assert.IsNull(negativeAdd.Result.Value);
            Assert.IsFalse(negativeAdd.Result.IsSuccess);

            //Assert
            var add = _helper.Post<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Add", new IcerikKurumsalSablonTanimlari
                {
                    GonderimTipi = "test",
                    SablonIcerikTipiId = 1,
                    IcerikTamMetin = "Test metni",
                    KurumID = 82,
                    AktifMi = 1,
                    KisiID = 129,
                    AktiflikTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    IcerikBaslik = "Test başlık",
                    KayitTarihi = DateTime.Now,
                    IcerikTanim = "Test icerik tanım",
                });

            Assert.AreEqual(add.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(add.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(add.IsSuccess);
            Assert.IsNotNull(add.Result.Value);

            //Assert negativeget
            var negativeGet = _helper.Get<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Get/" + 0);
            Assert.IsFalse(negativeGet.Result.IsSuccess);
            Assert.IsNull(negativeGet.Result.Value);

            //Assert get
            var get = _helper.Get<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Get/" + add.Result.Value.TabloID);

            Assert.AreEqual(get.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(get.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(get.Result.IsSuccess);
            Assert.IsNotNull(get.Result.Value);

            //Assert negativeUpdate
            var negativeUpdate = _helper.Post<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Update", new IcerikKurumsalSablonTanimlari
                {
                    IcerikTamMetin = "Test metni",
                    KurumID = 82,
                    AktifMi = 1,
                    KisiID = 129,
                    IcerikBaslik = "Test başlık",
                    IcerikTanim = "Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım ",
                    TabloID = add.Result.Value.TabloID
                });

            Assert.IsNull(negativeUpdate.Result.Value);
            Assert.IsFalse(negativeUpdate.Result.IsSuccess);

            //Assert update
            var update = _helper.Post<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Update", new IcerikKurumsalSablonTanimlari
                {
                    GonderimTipi = "test",
                    SablonIcerikTipiId = 1,
                    IcerikTamMetin = "Güncel metin",
                    KurumID = 82,
                    KisiID = 129,
                    AktifMi = 1,
                    AktiflikTarihi = DateTime.Now,
                    GuncellenmeTarihi = DateTime.Now,
                    KayitTarihi = add.Result.Value.KayitTarihi,
                    IcerikBaslik = "Güncel başlık",
                    IcerikTanim = "Güncel Tanim",
                    TabloID = add.Result.Value.TabloID
                });
            Assert.AreEqual(update.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(update.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(update.IsSuccess);
            Assert.IsNotNull(update.Result);

            //Assert negativeDelete
            var negativeDelete = _helper.Get<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Delete/" + 0);
            Assert.IsFalse(negativeDelete.Result.IsSuccess);
            Assert.IsNull(negativeDelete.Result.Value);

            // ListForSistem Sistem için getir
            var ListForSistem = _helper.Get<Result<List<ParamIcerikSablonTipleri>>>("/api/IcerikKurumsalSablonTanimlari/ListForSistem");
            Assert.AreEqual(ListForSistem.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(ListForSistem.Result.IsSuccess);

            // ListForKurum Kurum Id ile getir
            var ListForKurum = _helper.Get<Result<List<ParamIcerikSablonTipleri>>>("/api/IcerikKurumsalSablonTanimlari/ListForKurum/" + add.Result.Value.KurumID);
            Assert.AreEqual(ListForKurum.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(ListForKurum.Result.IsSuccess);

            //Assert setdelete
            var setdelete = _helper.Get<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/SetDeleted/" + add.Result.Value.TabloID);

            Assert.AreEqual(setdelete.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(setdelete.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(setdelete.IsSuccess);
            Assert.IsNotNull(setdelete.Result);

            //Assert delete
            var delete = _helper.Get<Result<IcerikKurumsalSablonTanimlari>>(
                $"/api/IcerikKurumsalSablonTanimlari/Delete/" + add.Result.Value.TabloID);

            Assert.AreEqual(delete.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(delete.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(delete.IsSuccess);
            Assert.IsNotNull(delete.Result);
        }

        /// <summary>
        ///ListBildirimTipleri getirme testi
        /// </summary>
        [TestMethod()]
        public void ListBildirimTipleri()
        {
            var listBildirim = _helper.Get<Result<List<ParamIcerikSablonTipleri>>>("/api/IcerikSablonTipleri/ListBildirimTipleri");
            Assert.AreEqual(listBildirim.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(listBildirim.Result.IsSuccess);
        }
    }
}