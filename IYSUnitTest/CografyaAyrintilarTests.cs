using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using Baz.Model.Entity;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// Coğrafya Ayrıntı Test Classıdır.
    /// </summary>
    [TestClass()]
    public class CografyaAyrintilarTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Coğrafya Ayrıntı Test Classıdır.
        /// </summary>
        public CografyaAyrintilarTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Coğrafya Ayrıntı Crud test methodudur.
        /// </summary>
        [TestMethod()]
        public void CrudTest()
        {
            //Assert-1 Add (Tanim eklenmeden Ayrinti eklemesi yapılamaz.)
            var addTanim = _helper.Post<Result<int>>($"/api/CografyaTanim/CografyaTanimKayit", new CografyaListViewModel
            {
                CografyaTanim = "Unit XY" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test Açıklama" + Guid.NewGuid().ToString().Substring(0, 10),
                KisiId = 130,
                KurumId = 82,
                UlkeId = 1,
                SehirlerIDList = new()
                {
                    1,
                    2,
                    3
                }
            });
            var add = _helper.Post<Result<CografyaListViewModel>>($"/api/CografyaAyrintilar/CografyaAyrintilarKayit", new CografyaListViewModel
            {
                CografyaTanim = "Unit XY" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test Açıklama" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaKutupanesiId = addTanim.Result.Value,
                KisiId = 130,
                KurumId = 82,
                UlkeId = 1,
                SehirlerIDList = new List<int> { 34, 35 }
            });
            Assert.AreEqual(add.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(add.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(add.Result);

            //Assert-2 NegativeAdd

            var negativeAdd = _helper.Post<Result<CografyaListViewModel>>($"/api/CografyaAyrintilar/CografyaAyrintilarKayit", new CografyaListViewModel
            {
            });
            Assert.IsNull(negativeAdd.Result.Value);
            Assert.IsFalse(negativeAdd.Result.IsSuccess);

            //Assert-3 Update

            var update = _helper.Post<Result<CografyaListViewModel>>($"/api/CografyaAyrintilar/CografyaAyrintilarGuncelle", new CografyaListViewModel
            {
                CografyaTanim = "Unit XYY" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test Açıklama" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaKutupanesiId = addTanim.Result.Value,
                KisiId = 130,
                KurumId = 82,
                UlkeId = 1,
                SehirlerIDList = new List<int> { 36, 37 }
            });
            Assert.AreEqual(update.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(update.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(update.Result);

            //Assert-4 NegativeUpdate (CografyaAlanTanim 100 karakter sınırı aşımı)

            var negativeUpdate = _helper.Post<Result<CografyaListViewModel>>($"/api/CografyaAyrintilar/CografyaAyrintilarGuncelle", new CografyaListViewModel
            {
            });
            Assert.IsNull(negativeUpdate.Result.Value);
            Assert.IsFalse(negativeUpdate.Result.IsSuccess);

            //Assert-5 Listing

            var listing = _helper.Get<Result<List<CografyaListViewModel>>>($"/api/CografyaAyrintilar/CografyaAyrintilarListeleme");
            Assert.AreEqual(listing.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(listing.Result.IsSuccess);

            //Assert-6 negativeGetById

            //var negativeGetById = _helper.Get<Result<List<CografyaKutuphanesiAyrintilar>>>($"/api/CografyaAyrintilar/CografyaAyrintilarGetirIdyeGore/" + 0);
            //Assert.IsNull(negativeGetById.Result);

            //Assert-7 GetById

            var getById = _helper.Get<Result<List<CografyaKutuphanesiAyrintilar>>>($"/api/CografyaAyrintilar/CografyaAyrintilarGetirIdyeGore/" + addTanim.Result.Value);
            Assert.AreEqual(getById.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getById.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getById.Result.Value);

            //Assert-8 negativeDelete

            //var negativeDelete = _helper.Get<Result<bool>>($"/api/CografyaAyrintilar/CografyaAyrintilarSil/" + 0);
            //Assert.IsFalse(negativeDelete.Result.Value);

            //Assert-9 Delete

            var delete = _helper.Get<Result<bool>>($"/api/CografyaAyrintilar/CografyaAyrintilarSil/" + addTanim.Result.Value);
            Assert.AreEqual(delete.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(delete.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.Result.Value);
        }
    }
}