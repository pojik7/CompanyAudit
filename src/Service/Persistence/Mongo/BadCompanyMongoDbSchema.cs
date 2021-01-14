using MongoDB.Bson.Serialization.Attributes;
using PipServices3.Commons.Data;

namespace Service.Persistence
{
    [BsonIgnoreExtraElements]
    [BsonNoId]
    public class BadCompanyMongoDbSchema : IStringIdentifiable
    {
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("company_id")]
        public string CompanyId { get; set; }

        [BsonElement("note")]
        public string Note { get; set; }        
    }
}
