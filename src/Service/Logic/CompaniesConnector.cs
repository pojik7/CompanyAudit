using Client.Clients.Version1;
using Companies.Data.Version1;
using PipServices3.Commons.Data;
using System.Threading.Tasks;

namespace CompanyAudit.Logic
{
    public class CompaniesConnector
    {
        private ICompaniesClientV1 _client;

        public CompaniesConnector(ICompaniesClientV1 client)
        {
            _client = client;
        }

        public async Task<DataPage<CompanyV1>> GetCompaniesAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort)
        {
            return await _client.GetCompaniesAsync(correlationId, filter, paging, sort);
        }


    }
}
