using Microsoft.VisualStudio.TestTools.UnitTesting;
using Baz.IysServiceApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.RequestManager.Abstracts;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using System.Net;
using Baz.Model.Entity;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// Icerik genel test classı
    /// </summary>

    [TestClass()]
    public class IcerikGenelTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Icerik genel test classı yapıcı metodu
        /// </summary>
        public IcerikGenelTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Icerik genel crud testi
        /// </summary>
        [TestMethod()]
        public void IcerikGenelCrud()
        {
            //Assert-1.1 NegativeAdd

            var negativeAdd = _helper.Post<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/AddOrUpdate",
                new IcerikHedefKitleViewModel
                {
                });
            Assert.IsNull(negativeAdd.Result.Value);
            Assert.IsFalse(negativeAdd.Result.IsSuccess);

            // Assert-1 Add
            var add = _helper.Post<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/AddOrUpdate",
                new IcerikHedefKitleViewModel
                {
                    IcerikBaslik = "Test Başlık",
                    IcerikOzetMetni = "Test Özet",
                    IcerikBitisZamani = DateTime.Now,
                    IcerikYayinlanmaZamani = DateTime.Now,
                    IcerikTamMetin = "Test metin 1",
                    IcerikTaslakMi = true,
                    KisiIds = new() { 129, 130 },
                    HedefIds = new() { 124 }
                });
            Assert.AreEqual(add.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(add.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(add.Result.IsSuccess);

            //Assert-2.1 NegativeUpdate

            var negativeUpdate = _helper.Post<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/AddOrUpdate",
                new IcerikHedefKitleViewModel
                {
                });
            Assert.IsNull(negativeUpdate.Result.Value);
            Assert.IsFalse(negativeUpdate.Result.IsSuccess);

            // Assert-2 Add
            var update = _helper.Post<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/AddOrUpdate",
                new IcerikHedefKitleViewModel
                {
                    TabloID = add.Result.Value.TabloID,
                    IcerikBaslik = "Test Başlık",
                    IcerikOzetMetni = "Test Özet",
                    IcerikBitisZamani = DateTime.Now.AddDays(1),
                    IcerikYayinlanmaZamani = DateTime.Now,
                    IcerikTamMetin = "Test metin 2",
                    IcerikTaslakMi = false,
                    KisiIds = new() { 129, 130 },
                    HedefIds = new() { 124 }
                });
            Assert.AreEqual(update.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(update.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(update.Result.IsSuccess);

            //Assert-3.1 negativeGetById

            var negativeGetById = _helper.Get<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/Get/" + 0);
            Assert.IsNotNull(negativeGetById.Result);

            //Assert-3 GetById

            var getById =
                _helper.Get<Result<IcerikHedefKitleViewModel>>($"/api/IcerikGenel/Get/" + add.Result.Value.TabloID);
            Assert.AreEqual(getById.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getById.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getById.Result);

            //Assert-6.1 negativeDelete

            var negativedelete = _helper.Get<Result<bool>>($"/api/IcerikGenel/IcerikKutuphanesiSil/" + 0);
            Assert.IsNotNull(negativedelete.Result);

            //Assert-6 Delete

            var delete =
                _helper.Get<Result<bool>>($"/api/IcerikGenel/IcerikKutuphanesiSil/" + add.Result.Value.TabloID);
            Assert.AreEqual(delete.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(delete.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(delete.Result);
        }

       

    }
}