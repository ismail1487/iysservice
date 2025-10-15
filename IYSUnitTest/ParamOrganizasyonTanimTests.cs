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
    ///  Param Organizasyon Birim Tanımlarının kontrol methodlarının testlerinin yer aldığı sınıftır.
    /// </summary>
    [TestClass()]
    public class ParamOrganizasyonTanimTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        ///  Param Organizasyon Birim Tanımlarının kontrol methodlarının testlerinin yer aldığı sınıfının yapıcı metodu
        /// </summary>
        public ParamOrganizasyonTanimTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Param Organizasyon Birim Tanımlarının testlerinin bulunduğu method.
        /// </summary>
        [TestMethod()]
        public void ParamOrganizasyonTanimTestsMethod()
        {
            //Assert listforview
            var listforview = _helper.Get<Result<List<ParamBirimTanimView>>>($"/api/ParamOrganizasyonBirimTanim/ListForView");
            Assert.AreEqual(listforview.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(listforview.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(listforview.Result);

            //Assert gettipid
            var gettipid = _helper.Get<Result<int>>($"/api/ParamOrganizasyonBirimTanim/GetTipId/Departman");
            Assert.AreEqual(gettipid.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(gettipid.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(gettipid.Result);

            //Assert negativegettipid
            var negativegettipid = _helper.Get<Result<int>>($"/api/ParamOrganizasyonBirimTanim/GetTipId/" + "Yönetim Kurulu");
            Assert.IsFalse(negativegettipid.Result.IsSuccess);
        }
    }
}