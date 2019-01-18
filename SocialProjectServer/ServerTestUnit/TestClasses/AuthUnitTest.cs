using System;
using Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerTestUnit.Containers;

namespace ServerTestUnit
{
    [TestClass]
    public class AuthUnitTest
    {
        public IAuthService authService { get; set; }
        public AuthUnitTest()
        {
            authService = TestContainer.container.GetInstance<IAuthService>();
        }
        [TestMethod]
        public void TestToken()
        {
            string userId = "Shahaf";
            string token = authService.GetNewToken(userId);
            Assert.AreEqual(true, authService.IsTokenValid(token));
        }
    }
}
