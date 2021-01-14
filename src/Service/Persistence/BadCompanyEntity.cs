using PipServices3.Commons.Data;
using System.Runtime.Serialization;

namespace Service.Persistence
{
    [DataContract]
    public class BadCompanyEntity : IStringIdentifiable
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        
        [DataMember(Name = "company_id")]
        public string CompanyId { get; set; }
        
        [DataMember(Name = "note")]
        public string Note { get; set; }
    }
}
