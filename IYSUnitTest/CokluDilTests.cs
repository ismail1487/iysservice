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
    /// Çoklu Dil Testlerinin yapıldığı sınıftır.
    /// Listeleme ve silme metodları için olumsuz test senaryolarına gerek duyulmamıştır.
    /// </summary>
    [TestClass()]
    public class CokluDilTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Çoklu Dil Testlerinin yapıldığı sınıfı yapcı metodu
        /// </summary>
        public CokluDilTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Menu testi
        /// </summary>
        [TestMethod()]
        public void GetMenuTest()
        {
            //Act-1  MenuId-DilId
            var menuListRequest = new MenuListRequest
            {
                MenuId = 1,
                DilId = 1
            };
            var getmenu = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest);
            Assert.AreEqual(getmenu.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(getmenu.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(getmenu.Result.Value);

            //Act-2  MenuId-DilId
            var menuListRequest1 = new MenuListRequest
            {
                MenuId = 1,
                DilId = 2
            };
            var dil1 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest1);
            Assert.AreEqual(dil1.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil1.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil1.Result.Value);

            //Act-3  MenuId-DilKodu
            var menuListRequest2 = new MenuListRequest
            {
                MenuId = 1,
                DilKodu = "tr-TR"
            };
            var dil2 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest2);
            Assert.AreEqual(dil2.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil2.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil2.Result.Value);

            //Act-4  MunuId-DilKodu
            var menuListRequest3 = new MenuListRequest
            {
                MenuId = 1,
                DilKodu = "en-US"
            };
            var dil3 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest3);
            Assert.AreEqual(dil3.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil3.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil3.Result.Value);

            //Act-5  MenuAdı-DilId
            var menuListRequest4 = new MenuListRequest
            {
                Name = "GENEL",
                DilId = 1
            };
            var dil4 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest4);
            Assert.AreEqual(dil4.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil4.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil4.Result.Value);

            //Act-6  MenuAdı-DilId
            var menuListRequest5 = new MenuListRequest
            {
                Name = "GENEL",
                DilId = 2
            };
            var dil5 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest5);
            Assert.AreEqual(dil5.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil5.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil5.Result.Value);

            //Act-7  MenuAdı-DilKodu
            var menuListRequest6 = new MenuListRequest
            {
                Name = "GENEL",
                DilKodu = "tr-TR"
            };
            var dil6 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest6);
            Assert.AreEqual(dil6.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil6.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil6.Result.Value);

            //Act-8  MenuAdı-DilKodu
            var menuListRequest7 = new MenuListRequest
            {
                Name = "GENEL",
                DilKodu = "en-US"
            };
            var dil7 = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", menuListRequest7);
            Assert.AreEqual(dil7.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(dil7.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(dil7.Result.Value);

            //Act-9 Coklu Dil List

            var cokluDilList = _helper.Get<Result<List<ParamDiller>>>($"/api/Dil/List");
            Assert.AreEqual(cokluDilList.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(cokluDilList.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(cokluDilList.Result);

            //Act-10 Coklu Dil Get
            var getById = _helper.Get<int>($"/api/Dil/Get/" + "tr-TR");
            Assert.AreEqual(getById.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(getById.IsSuccess);

            //Act-10.1 Coklu Dil Get Negatif
            var getByIdnegative = _helper.Get<int>($"/api/Dil/Get/" + 0);
            Assert.AreEqual(getByIdnegative.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(getByIdnegative.IsSuccess);

            //Act-11 Coklu Dil GetParamKod
            var getParamKod = _helper.Get<Result<string>>($"/api/Dil/GetParamKod/" + menuListRequest.DilId);
            Assert.AreEqual(getParamKod.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(getParamKod.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getParamKod.Result);

            //Act-11.1 Coklu Dil GetParamKod Negatif
            var getParamKodNegative = _helper.Get<Result<string>>($"/api/Dil/GetParamKod/" + -1);
            Assert.IsFalse(getParamKodNegative.Result.IsSuccess);
        }
    }
}