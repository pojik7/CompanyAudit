using Client.Clients.Version1;
using Companies.Data.Version1;
using CompaniesAudit.Logic;
using PipServices3.Commons.Config;
using PipServices3.Commons.Refer;
using Service.Logic;
using System;
using System.Globalization;
using Xunit;

namespace Service.Test.Logic
{
    public class CompaniesAuditControllerTest: IDisposable
    {        
        CompanyV1 company1 = new CompanyV1()
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

        CompanyV1 company2 = new CompanyV1()
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

        private CompaniesAuditController _controller;
        private CompaniesHttpClientV1 _client;        
        private bool isIntegratedTestEnable;

        public CompaniesAuditControllerTest()
        {

            isIntegratedTestEnable = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HOST"));

            _controller = new CompaniesAuditController();

            _client = new CompaniesHttpClientV1();
            _client.Configure(ConfigParams.FromTuples(
                    "connection.host", Environment.GetEnvironmentVariable("HOST"),
                    "connection.port", Environment.GetEnvironmentVariable("PORT"),
                    "connection.protocol", "http"
                ));

            _controller.Configure(ConfigParams.FromTuples(
                    "mailsettings.body", "[BANKCODE][ACCCODE][STATECODE][IBAN][NAME][CONTRACTDATE][NOTE][EMPLOYEE_ID]"
                ));


            var references = References.FromTuples(
                new Descriptor("companies-audit", "controller", "default", "*", "1.0"), _controller,
                new Descriptor("companies-service", "client", "http", "default", "1.0"), _client                                                
            );
                        
            _controller.SetReferences(references);

            _controller._sendMessageClient = new SendMessageNullHelper();

            _client.OpenAsync(null).Wait();

        }
         
        [Fact]
        public async void GetCompaniesForCheckAsync()
        {
            if (isIntegratedTestEnable)
            {
                Console.WriteLine(Environment.GetEnvironmentVariable("HOST"));
                Console.WriteLine(Environment.GetEnvironmentVariable("PORT"));
                await _client.CreateCompanyAsync(null, company1);

                var list = await _controller.GetCompaniesForCheckAsync(null);

                Assert.True(list.Count == 1);
                AssertCompanies(company1, list[0]);

                await _client.CreateCompanyAsync(null, company2);

                list = await _controller.GetCompaniesForCheckAsync(null);
                Assert.True(list.Count == 2);
            }
            else
                Assert.True(true);

        }

        [Fact]
        public void SendMail()
        {            
            var text = _controller.DoTheBadThings(null,company2, "The company classified as BAD company");
            var etalon = "8888882600000002132456780IBANBadCompany202.05.2020The company classified as BAD company2";
            Assert.Equal(text, etalon);
        }

        [Fact]
        public void IsBadCompany()
        {
            Assert.True(_controller.IsBadCompany(null, company2));
            Assert.False(_controller.IsBadCompany(null, company1));
        }


        private static void AssertCompanies(CompanyV1 etalon, CompanyV1 company)
        {
            Assert.NotNull(company);
            Assert.Equal(etalon.Name, company.Name);
            Assert.Equal(etalon.AccCode, company.AccCode);
            Assert.Equal(etalon.BankCode, company.BankCode);
            Assert.Equal(etalon.ContractDate, company.ContractDate);
            Assert.Equal(etalon.ContractNo, company.ContractNo);
            Assert.Equal(etalon.IBAN, company.IBAN);
            Assert.Equal(etalon.Id, company.Id);
            Assert.Equal(etalon.StateCode, company.StateCode);
            Assert.Equal(etalon.EmployeeId, company.EmployeeId);
        }

        public void Dispose()
        {
            _client.CloseAsync(null).Wait();
        }
    }
}
