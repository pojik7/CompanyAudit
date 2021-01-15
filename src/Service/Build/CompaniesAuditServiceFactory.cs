using Client.Clients.Version1;
using PipServices3.Commons.Refer;
using PipServices3.Components.Build;
using Service.Logic;

namespace Service.Build
{
    public class CompaniesAuditServiceFactory : Factory
    {        
        public static Descriptor ControllerDescriptor = new Descriptor("companies-audit", "controller", "default", "*", "1.0");                
        public static Descriptor CompaniesHttpClientDescriptor = new Descriptor("companies-service", "client", "http", "*", "1.0");

        public CompaniesAuditServiceFactory()
        {            
            RegisterAsType(ControllerDescriptor, typeof(CompaniesAuditController));            
            RegisterAsType(CompaniesHttpClientDescriptor, typeof(CompaniesHttpClientV1));
        }
    }
}
