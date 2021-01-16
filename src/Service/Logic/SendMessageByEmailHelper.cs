using Companies.Data.Version1;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace CompaniesAudit.Logic
{
    public class SendMessageByEmailHelper: ISendMessageHelper
    {        
        protected string ProccessTags(string Body, CompanyV1 BadCompany, string Note)
        {
            return Body.Replace("[BANKCODE]", BadCompany.BankCode).
                Replace("[ACCCODE]", BadCompany.AccCode).
                Replace("[STATECODE]", BadCompany.StateCode).
                Replace("[IBAN]", BadCompany.IBAN).
                Replace("[NAME]", BadCompany.Name).
                Replace("[CONTRACTDATE]", BadCompany.ContractDate.ToString("dd.MM.yyyy")).
                Replace("[NOTE]", Note).
                Replace("[EMPLOYEE_ID]", BadCompany.EmployeeId.ToString());
        }

        public virtual string SendEmail(CompanyV1 Company, string host,string fromAddress, string toAddress, string Subj, string Body, string Note)
        {
            Body = ProccessTags(Body, Company, Note);
            SmtpClient client = new SmtpClient(host);
            MailAddress from = new MailAddress(fromAddress, fromAddress, System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(toAddress);
            MailMessage message = new MailMessage(from, to);
            message.Body = Body;
            message.Subject = Subj;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            string token = "token";
            client.SendAsync(message, token);
            message.Dispose();
            return Body;
        }
    }
}
