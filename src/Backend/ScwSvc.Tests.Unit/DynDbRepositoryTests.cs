using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.Models;

namespace ScwSvc.Tests.Unit
{
    public class DynDbRepositoryTests
    {
        private DbDynContext _dynDb;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _dynDb = new DbDynContext(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
