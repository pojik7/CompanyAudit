using Companies.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompaniesAudit.Logic
{
    public class SendMessageNullHelper : SendMessageByEmailHelper
    {
        public override string SendEmail(CompanyV1 Company, string host, string fromAddress, string toAddress, string Subj, string Body, string Note)
        {
            return ProccessTags(Body, Company, Note);
        }
    }
}
