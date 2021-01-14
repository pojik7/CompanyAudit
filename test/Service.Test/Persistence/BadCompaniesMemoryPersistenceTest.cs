using PipServices3.Commons.Config;
using Service.Persistence;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Service.Test.Persistence
{
    public class BadCompaniesMemoryPersistenceTest : IDisposable
    {
        public BadCompaniesMemoryPersistence _persistence;
        public BadCompaniesPersistenceFixture _fixture;

        public BadCompaniesMemoryPersistenceTest()
        {
            _persistence = new BadCompaniesMemoryPersistence();
            _persistence.Configure(new ConfigParams());

            _fixture = new BadCompaniesPersistenceFixture(_persistence);

            _persistence.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            await _fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestGetWithFiltersAsync()
        {
            await _fixture.TestGetWithFiltersAsync();
        }
    }
}
