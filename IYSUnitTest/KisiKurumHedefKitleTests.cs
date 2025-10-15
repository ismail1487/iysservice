using Microsoft.VisualStudio.TestTools.UnitTesting;
using Baz.IysServiceApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System.Net;
using IYSUnitTest.Helper;

namespace IYSUnitTest
{
    /// <summary>
    /// KisiKurum hedef kitle test classı
    /// </summary>
    [TestClass()]
    public class KisiKurumHedefKitleTests
    {
        private readonly IRequestHelper _helper;

        /// <summary>
        ///KisiKurum hedef kitle test classı yapıcı metodu
        /// </summary>
        public KisiKurumHedefKitleTests()
        {
            _helper = TestServerRequestHelper.CreateHelper();
        }

        
    }
}