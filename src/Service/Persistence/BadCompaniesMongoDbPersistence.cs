using MongoDB.Driver;
using PipServices3.Commons.Data;
using PipServices3.Commons.Data.Mapper;
using PipServices3.MongoDb.Persistence;
using System;
using System.Threading.Tasks;

namespace Service.Persistence
{
    public class BadCompaniesMongoDbPersistence : IdentifiableMongoDbPersistence<BadCompanyMongoDbSchema, string>, IBadCompaniesPersistence
    {
        public BadCompaniesMongoDbPersistence() :
            base("badcompanies")
        {
        }

        private new FilterDefinition<BadCompanyMongoDbSchema> ComposeFilter(FilterParams filterParams)
        {
            filterParams = filterParams ?? new FilterParams();

            var builder = Builders<BadCompanyMongoDbSchema>.Filter;
            var filter = builder.Empty;

            var id = filterParams.GetAsNullableString("id");
            var company_id = filterParams.GetAsNullableString("company_id");
            var note = filterParams.GetAsNullableString("note");
            

            if (!string.IsNullOrWhiteSpace(id)) filter &= builder.Eq(b => b.Id, id);
            if (!string.IsNullOrWhiteSpace(company_id)) filter &= builder.Eq(b => b.CompanyId, company_id);
            if (!string.IsNullOrWhiteSpace(note)) filter &= builder.Eq(b => b.Note, note);            

            return filter;
        }
        public async Task<DataPage<BadCompanyEntity>> GetRecordsAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort)
        {
            var result = await base.GetPageByFilterAsync(correlationId, ComposeFilter(filter), paging);
            var data = result.Data.ConvertAll(ToPublic);

            return new DataPage<BadCompanyEntity>
            {
                Data = data,
                Total = result.Total
            };
        }

        public async Task<BadCompanyEntity> CreateRecordAsync(string correlationId, BadCompanyEntity record)
        {
            var result = await CreateAsync(correlationId, FromPublic(record));

            return ToPublic(result);
        }

        private static BadCompanyEntity ToPublic(BadCompanyMongoDbSchema value)
        {
            return value == null ? null : ObjectMapper.MapTo<BadCompanyEntity>(value);
        }

        private static BadCompanyMongoDbSchema FromPublic(BadCompanyEntity value)
        {
            return ObjectMapper.MapTo<BadCompanyMongoDbSchema>(value);
        }
    }
}
