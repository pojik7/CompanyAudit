using Client.Clients.Version1;
using Companies.Data.Version1;
using CompanyAudit.Logic;
using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipServices3.Commons.Run;
using PipServices3.Components.Config;
using PipServices3.Components.Lock;
using PipServices3.Components.Log;
using Service.Persistence;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class CompaniesAuditController : IOpenable, IClosable, IConfigurable, IReferenceable
    {
        private IBadCompaniesPersistence _persistence;
        private CompaniesConnector _companiesConnector;
        private ICompaniesClientV1 _companiesClient;
        private FixedRateTimer Timer { get; set; } = new FixedRateTimer();
        private Parameters Parameters { get; set; } = new Parameters();
        private readonly CompositeLogger _logger = new CompositeLogger();

        private string host;
        private string fromAddress;
        private string toAddress;
        private string subj;
        private string body;
        int interval;
        int delay;

        public void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
            _companiesClient = references.GetOneRequired<ICompaniesClientV1>(
                new Descriptor("companies-service", "client", "*", "*", "1.0"));
            _companiesConnector = new CompaniesConnector(_companiesClient);
            _persistence = references.GetOneRequired<IBadCompaniesPersistence>(
                new Descriptor("companies-audit", "persistence", "*", "*", "1.0")
            );
        }

        public void Configure(ConfigParams config)
        {
            host = config.GetAsStringWithDefault("host", string.Empty);
            fromAddress = config.GetAsStringWithDefault("mailsettings.from_address", string.Empty);
            toAddress = config.GetAsStringWithDefault("mailsettings.to_address", string.Empty);
            subj = config.GetAsStringWithDefault("mailsettings.subj", string.Empty);
            body = config.GetAsStringWithDefault("mailsettings.body", string.Empty);
            interval = config.GetAsIntegerWithDefault("intervals.interval", 10000);
            delay = config.GetAsIntegerWithDefault("intervals.delay", 10000);
        }

        public Task OpenAsync(string correlationId)
        {
            Timer.Task = new Action(async () => await ExecuteAsync(correlationId, Parameters));
            Timer.Interval = interval;
            Timer.Delay = delay;
            Timer.Start();
            _logger.Trace(correlationId, "Companies Audit Controller opened");
            return Task.CompletedTask;
        }
        public bool IsOpen()
        {
            return Timer.IsStarted;
        }

        public Task CloseAsync(string correlationId)
        {
            Timer.Stop();
            _logger.Trace(correlationId, "Company Audit Controller closed");
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(string correlationId, Parameters parameters)
        {
            _logger.Info(correlationId, "Run Task correlationId = {0}", correlationId);
            await AuditTaskAsync(correlationId);
        }

        protected async Task<DataPage<BadCompanyEntity>> GetRecordsAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort)
        {
            return await _persistence.GetRecordsAsync(correlationId, filter, paging, sort);
        }

        public async Task<BadCompanyEntity> CreateRecordAsync(string correlationId, BadCompanyEntity record)
        {
            record.Id = record.Id ?? IdGenerator.NextLong();

            return await _persistence.CreateRecordAsync(correlationId, record);
        }

        private async Task AuditTaskAsync(string correlationId)
        {
            var task = _companiesConnector.GetCompaniesAsync(correlationId, new PipServices3.Commons.Data.FilterParams(), new PipServices3.Commons.Data.PagingParams(), new PipServices3.Commons.Data.SortParams());
            task.Wait();
            foreach (var company in task.Result.Data)
            {
                if (IsBadCompany(company))
                {
                    await DoTheBadThings(correlationId, company, "The company classified as BAD company");
                }
            }
        }

        public bool IsBadCompany(CompanyV1 company)
        {
            return company.Name.ToLower().ToLower().Contains("bad");
        }

        private async Task DoTheBadThings(string correlationId, CompanyV1 company, string Note)
        {
            SendEmail(company, Note);
            await StoreToStorage(correlationId, company, Note);
        }

        private void SendEmail(CompanyV1 Company, string Note)
        {
            body = ProccessTags(body, Company, Note);
            SmtpClient client = new SmtpClient(host);
            MailAddress from = new MailAddress(fromAddress, fromAddress, System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(toAddress);
            MailMessage message = new MailMessage(from, to);
            message.Body = body;
            message.Subject = subj;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            string token = "token";
            client.SendAsync(message, token);
            message.Dispose();

        }

        public string ProccessTags(string body,CompanyV1 BadCompany, string Note)
        {
            return body.Replace("[BANKCODE]", BadCompany.BankCode).
                Replace("[ACCCODE]", BadCompany.AccCode).
                Replace("[STATECODE]", BadCompany.StateCode).
                Replace("[IBAN]", BadCompany.IBAN).
                Replace("[NAME]", BadCompany.Name).
                Replace("[CONTRACTDATE]", BadCompany.ContractDate.ToString("dd.MM.yyyy")).
                Replace("[NOTE]", Note).
                Replace("[EMPLOYEE_ID]", BadCompany.EmployeeId.ToString());
        }

        public async Task<BadCompanyEntity> StoreToStorage(string correlationId, CompanyV1 company, string Note)
        {
            var record = new BadCompanyEntity();
            record.CompanyId = company.Id;
            record.Note = Note;
            return await CreateRecordAsync(correlationId, record);
        }
    }
}
