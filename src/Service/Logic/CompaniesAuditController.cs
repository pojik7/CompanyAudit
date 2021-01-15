using Client.Clients.Version1;
using Companies.Data.Version1;
using PipServices3.Commons.Config;
using PipServices3.Commons.Refer;
using PipServices3.Commons.Run;
using PipServices3.Components.Log;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class CompaniesAuditController : IOpenable, IClosable, IConfigurable, IReferenceable
    {                
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
            Timer.Task = new Action (() => Execute(correlationId, Parameters));
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

        public void Execute(string correlationId, Parameters parameters)
        {
            _logger.Info(correlationId, "Run Task correlationId = {0}", correlationId);
            AuditTask(correlationId);
        }
        
        private void AuditTask(string correlationId)
        {
            var list = GetCompaniesForCheck(correlationId);
            foreach (var company in list)
            {
                if (IsBadCompany(correlationId, company))
                {
                    DoTheBadThings(correlationId, company, "The company classified as BAD company");
                }
            }
        }

        public List<CompanyV1> GetCompaniesForCheck(string correlationId)
        {
            var task = _companiesClient.GetCompaniesAsync(correlationId, new PipServices3.Commons.Data.FilterParams(), new PipServices3.Commons.Data.PagingParams(), new PipServices3.Commons.Data.SortParams());
            task.Wait();
            return task.Result.Data;
        }

        public bool IsBadCompany(string correlationId, CompanyV1 company)
        {
            return company.Name.ToLower().ToLower().Contains("bad");
        }

        private void DoTheBadThings(string correlationId, CompanyV1 company, string Note)
        {
            SendEmail(company, Note);
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

        private void ShowMessage(CompanyV1 Company, string Note)
        {
            body = ProccessTags(body, Company, Note);
            Console.WriteLine(body);
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
    }
}
