using PipServices3.Commons.Data;
using System.Threading.Tasks;

namespace Service.Persistence
{
    public interface IBadCompaniesPersistence
    {
        Task<DataPage<BadCompanyEntity>> GetRecordsAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort);
        Task<BadCompanyEntity> CreateRecordAsync(string correlationId, BadCompanyEntity record);
    }
}
