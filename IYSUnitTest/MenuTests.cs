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
    /// Menü kontrolü için gerekli test metotların bulunduğu sınıftır.
    /// </summary>
    [TestClass()]
    public class MenuTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Menü kontrolü için gerekli test metotların bulunduğu sınıf yapıcı metodu
        /// </summary>
        public MenuTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        /// <summary>
        /// Menü getirme test metotu
        /// </summary>
        [TestMethod()]
        public void MenuTestsMethods()
        {
            //Assert getmenu
            var getmenu = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", new MenuListRequest
            {
                DilId = 1,
                DilKodu = "Türkçe",
                MenuId = 1,
                Name = "GENEL"
            }
            );
            Assert.AreEqual(getmenu.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.AreEqual(getmenu.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(getmenu.Result.Value);

            //Assert getmenunegative
            var getmenunegative = _helper.Post<Result<List<MenuListResponse>>>($"/api/Menu/List", new MenuListRequest
            {
                DilId = 0,
                DilKodu = "",
                MenuId = 1,
                Name = "GENEL"
            }
            );
            Assert.IsFalse(getmenunegative.Result.IsSuccess);

            //Assert yetkiicinsayfa
            var yetkiicinsayfa = _helper.Get<Result<List<SistemSayfalari>>>($"/api/Menu/YetkiIcinSayfaGetir/{1}");
            Assert.AreEqual(yetkiicinsayfa.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(yetkiicinsayfa.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(yetkiicinsayfa.Result.Value);
        }
    }
}