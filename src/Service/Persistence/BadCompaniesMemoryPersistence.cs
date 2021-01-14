using MongoDB.Driver;
using PipServices3.Commons.Data;
using PipServices3.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Persistence
{
    public class BadCompaniesMemoryPersistence : IdentifiableMemoryPersistence<BadCompanyEntity, string>, IBadCompaniesPersistence
    {
        public BadCompaniesMemoryPersistence()
        {
            _maxPageSize = 1000;
        }

        public Task<DataPage<BadCompanyEntity>> GetRecordsAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort)
        {
            return base.GetPageByFilterAsync(correlationId, ComposeFilter(filter), paging);
        }

        public Task<BadCompanyEntity> CreateRecordAsync(string correlationId, BadCompanyEntity record)
        {
            return base.CreateAsync(correlationId, record);
        }

        private List<Func<BadCompanyEntity, bool>> ComposeFilter(FilterParams filterParams)
        {
            filterParams = filterParams ?? new FilterParams();

            var builder = Builders<BadCompanyEntity>.Filter;
            var filter = builder.Empty;

            var id = filterParams.GetAsNullableString("id");
            var CompanyId = filterParams.GetAsNullableString("company_id");
            var Note = filterParams.GetAsNullableString("note");
            

            return new List<Func<BadCompanyEntity, bool>>
            {
                (item) =>
                {
                    if (!string.IsNullOrWhiteSpace(id) && item.Id != id) return false;
                    if (!string.IsNullOrWhiteSpace(Note) && item.Note != Note) return false;
                    if (!string.IsNullOrWhiteSpace(CompanyId) && item.CompanyId != CompanyId) return false;                    
                    return true;
                }
            };

        }
    }
}
