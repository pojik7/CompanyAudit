using PipServices3.Container;
using PipServices3.Rpc.Build;
using Service.Build;

namespace CompaniesAudit.Conteiner
{
    public class CompaniesAuditProcess : ProcessContainer
    {
        public CompaniesAuditProcess()
            : base("companies-audit", "Companies audit microservice")
        {
            _factories.Add(new DefaultRpcFactory());
            _factories.Add(new CompaniesAuditServiceFactory());
        }
    }
}
