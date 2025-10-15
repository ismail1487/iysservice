using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
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
    /// Coğrafya Tanım Test Classıdır.
    /// </summary>
    [TestClass()]
    public class CografyaTanimTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// oğrafya Tanım Test Classı
        /// </summary>
        public CografyaTanimTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Coğrafya Tanım Crud Test Methodu.
        /// </summary>
        [TestMethod()]
        public void CrudTests()
        {
            //Assert-1 Add
            var add = _helper.Post<Result<int>>($"/api/CografyaTanim/CografyaTanimKayit", new CografyaListViewModel
            {
                CografyaTanim = "Unit XXX1" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test1" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumId = 82,
                KisiId = 129,
                UlkeId = 1,
                SehirlerIDList = new() { 1, 2, 3 }
            });
            Assert.AreEqual(add.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(add.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(add.Result);

            //Assert-2 NegativeAdd

            var negativeAdd = _helper.Post<Result<int>>($"/api/CografyaTanim/CografyaTanimKayit", new CografyaListViewModel
            {
                KurumId = 82,
                KisiId = 130,
            });
            Assert.AreEqual(0, negativeAdd.Result.Value);
            Assert.IsFalse(negativeAdd.Result.IsSuccess);

            //Assert-3 Update

            var update = _helper.Post<Result<CografyaKutuphanesi>>($"/api/CografyaTanim/CografyaTanimGuncelle", new CografyaListViewModel
            {
                KisiId = 129,
                CografyaKutupanesiId = add.Result.Value,
                CografyaTanim = "Unit XX" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test2" + Guid.NewGuid().ToString().Substring(0, 10),
                UlkeId = 1,
                SehirlerIDList = new() { 1, 2, 3, 4 }
            });
            Assert.AreEqual(update.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(update.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(update.Result);

            //Assert-4 NegativeUpdate

            var negativeUpdate = _helper.Post<Result<CografyaKutuphanesi>>($"/api/CografyaTanim/CografyaTanimGuncelle", new CografyaListViewModel
            {
                KisiId = 130,
                CografyaKutupanesiId = add.Result.Value,
                CografyaTanim = "Unit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test GuncelUnit Test Guncel",
                CografyaAciklama = "Unit Test Açıklama Guncel",
            });
            Assert.IsNull(negativeUpdate.Result.Value);
            Assert.IsFalse(negativeUpdate.Result.IsSuccess);

            //Assert-5 List

            var list = _helper.Get<Result<List<CografyaKutuphanesi>>>($"/api/CografyaTanim/CografyaTanimListeleme");
            Assert.AreEqual(list.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(list.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Result);

            //Assert-6 negativeGetById

            var negativeGetById = _helper.Get<Result<CografyaKutuphanesi>>($"/api/CografyaTanim/CografyaTanimIdyeGoreGetir/" + 0);
            Assert.IsNull(negativeGetById.Result.Value);
            Assert.IsFalse(negativeGetById.Result.IsSuccess);

            //Assert-7 GetById

            var getById = _helper.Get<Result<CografyaKutuphanesi>>($"/api/CografyaTanim/CografyaTanimIdyeGoreGetir/" + add.Result.Value);
            Assert.AreEqual(getById.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(getById.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(getById.Result);

            //Assert-8 negativeDelete

            var negativeDelete = _helper.Get<Result<bool>>($"/api/CografyaTanim/CografyaTanimSil/" + 0);
            Assert.IsFalse(negativeDelete.Result.Value);

            //Assert-9 Delete

            var delete = _helper.Get<Result<bool>>($"/api/CografyaTanim/CografyaTanimSil/" + add.Result.Value);
            Assert.AreEqual(delete.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(delete.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.Result.Value);
        }
    }
}