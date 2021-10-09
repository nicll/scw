using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.Models;

namespace ScwSvc.Tests.Unit
{
    public class MultiRepositoryTests
    {
        private DbSysContext _sysDb;
        private DbDynContext _dynDb;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _sysDb = new DbSysContext(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
            _dynDb = new DbDynContext(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
