using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.Models;

namespace ScwSvc.Tests
{
    public class SysDbInteractorTests
    {
        private DbSysContext _sysDb;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _sysDb = new DbSysContext(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
