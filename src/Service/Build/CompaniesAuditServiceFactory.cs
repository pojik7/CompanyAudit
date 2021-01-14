using Client.Clients.Version1;
using PipServices3.Commons.Refer;
using PipServices3.Components.Build;
using Service.Logic;
using Service.Persistence;

namespace Service.Build
{
    public class CompaniesAuditServiceFactory : Factory
    {
        public static Descriptor MongoDbPersistenceDescriptor = new Descriptor("companies-audit", "persistence", "mongodb", "*", "1.0");
        public static Descriptor ControllerDescriptor = new Descriptor("companies-audit", "controller", "default", "*", "1.0");        
        public static Descriptor MemoryPersistenceDescriptor = new Descriptor("companies-audit", "persistence", "memory", "*", "1.0");
        public static Descriptor CompaniesHttpClientDescriptor = new Descriptor("companies-service", "client", "http", "*", "1.0");

        public CompaniesAuditServiceFactory()
        {
            RegisterAsType(MemoryPersistenceDescriptor, typeof(BadCompaniesMemoryPersistence));
            RegisterAsType(MongoDbPersistenceDescriptor, typeof(BadCompaniesMongoDbPersistence));
            RegisterAsType(ControllerDescriptor, typeof(CompaniesAuditController));            
            RegisterAsType(CompaniesHttpClientDescriptor, typeof(CompaniesHttpClientV1));
        }
    }
}
