using PipServices3.Commons.Data;
using Service.Persistence;
using System.Threading.Tasks;
using Xunit;

namespace Service.Test.Persistence
{
    public class BadCompaniesPersistenceFixture
    {
        private BadCompanyEntity BadCompany1 = new BadCompanyEntity()
        {
            CompanyId = "1",
            Note = "The company classified as BAD company"            
        };

        private BadCompanyEntity BadCompany2 = new BadCompanyEntity()
        {
            CompanyId = "2",
            Note = "The company classified as BAD company, but..."
        };

        private BadCompanyEntity BadCompany3 = new BadCompanyEntity()
        {
            CompanyId = "3",
            Note = "The company classified as BAD company"
        };

        private IBadCompaniesPersistence _persistence;

        public BadCompaniesPersistenceFixture(IBadCompaniesPersistence persistence)
        {
            _persistence = persistence;
        }

        private async Task TestCreateBadCompaniesAsync()
        {
            // Create the first customer
            var badCompany = await _persistence.CreateRecordAsync(null, BadCompany1);

            AssertBadCompanies(BadCompany1, badCompany);

            // Create the second customer
            badCompany = await _persistence.CreateRecordAsync(null, BadCompany2);

            AssertBadCompanies(BadCompany2, badCompany);

            // Create the third customer
            badCompany = await _persistence.CreateRecordAsync(null, BadCompany3);

            AssertBadCompanies(BadCompany3, badCompany);
        }

        public async Task TestCrudOperationsAsync()
        {
            // Create items
            await TestCreateBadCompaniesAsync();

            // Get all customers
            var page = await _persistence.GetRecordsAsync(
                null,
                new FilterParams(),
                new PagingParams(),
                new SortParams()
            );

            Assert.NotNull(page);
            Assert.Equal(3, page.Data.Count);

            var badCompany1 = page.Data[0];           
        }

        public async Task TestGetWithFiltersAsync()
        {
            // Create items
            await TestCreateBadCompaniesAsync();

            // Filter by id
            var page = await _persistence.GetRecordsAsync(
                null,
                FilterParams.FromTuples(
                    "company_id", "1"
                ),
                new PagingParams(),
                new SortParams()
            );

            Assert.Single(page.Data);

            // Filter by state_code
            page = await _persistence.GetRecordsAsync(
                null,
                FilterParams.FromTuples(
                    "note", "The company classified as BAD company, but..."
                ),
                new PagingParams(),
                new SortParams()
            );

            Assert.Single(page.Data);
        }

        private static void AssertBadCompanies(BadCompanyEntity etalon, BadCompanyEntity record)
        {
            Assert.NotNull(record);
            Assert.Equal(etalon.CompanyId, record.CompanyId);
            Assert.Equal(etalon.Note, record.Note);            
        }
    }
}
