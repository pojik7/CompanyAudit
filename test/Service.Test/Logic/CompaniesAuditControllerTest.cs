using Client.Clients.Version1;
using Companies.Data.Version1;
using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using Service.Logic;
using Service.Persistence;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Service.Test.Logic
{
    public class CompaniesAuditControllerTest : IDisposable
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


        private CompanyV1 Company1 = new CompanyV1()
        {
            AccCode = "2600000001",
            BankCode = "777777",
            ContractDate = DateTime.ParseExact("01/05/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            ContractNo = "1",
            EmployeeId = 1,
            IBAN = "IBAN",
            Id = "1",
            Name = "Company1",
            StateCode = "132456789"
        };

        private CompanyV1 Company2 = new CompanyV1()
        {
            AccCode = "2600000002",
            BankCode = "888888",
            ContractDate = DateTime.ParseExact("02/05/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            ContractNo = "2",
            EmployeeId = 2,
            IBAN = "IBAN",
            Id = "2",
            Name = "BadCompany2",
            StateCode = "132456780"
        };

        string body = "[BANKCODE][ACCCODE][STATECODE][IBAN][NAME][CONTRACTDATE][NOTE][EMPLOYEE_ID]";

        private CompaniesAuditController _controller;
        private BadCompaniesMemoryPersistence _persistence;

        public CompaniesAuditControllerTest()
        {
            _persistence = new BadCompaniesMemoryPersistence();
            _persistence.Configure(new ConfigParams());

            _controller = new CompaniesAuditController();

            var references = References.FromTuples(
                new Descriptor("companies-audit", "persistence", "memory", "*", "1.0"), _persistence,
                new Descriptor("companies-audit", "controller", "default", "*", "1.0"), _controller,
                new Descriptor("companies-service", "client", "null", "default", "1.0"), new CompaniesNullClientV1()
            );

            _controller.SetReferences(references);

            _persistence.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            _persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public void ProccessTagsTest()
        {
            var text = _controller.ProccessTags(body,Company2, "The company classified as BAD company");
            var etalon = "8888882600000002132456780IBANBadCompany202.05.2020The company classified as BAD company2";
            Assert.Equal(text, etalon);
        }

        [Fact]
        public async Task StoreToStorage()
        {
            var record = await _controller.StoreToStorage(null, Company2, "The company classified as BAD company, but...");
            AssertBadCompanies(BadCompany2, record);
        }

        [Fact]
        public void IsBadCompanyTest()
        {
            Assert.True(_controller.IsBadCompany(Company2));
            Assert.False(_controller.IsBadCompany(Company1));
        }

        private static void AssertBadCompanies(BadCompanyEntity etalon, BadCompanyEntity record)
        {
            Assert.NotNull(record);
            Assert.Equal(etalon.CompanyId, record.CompanyId);
            Assert.Equal(etalon.Note, record.Note);
        }



    }
}
