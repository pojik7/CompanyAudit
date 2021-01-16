using Companies.Data.Version1;

namespace CompaniesAudit.Logic
{
    public interface ISendMessageHelper
    {
        string SendEmail(CompanyV1 Company, string host, string fromAddress, string toAddress, string Subj, string Body, string Note);
    }
}
