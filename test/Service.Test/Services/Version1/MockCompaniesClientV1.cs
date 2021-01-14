using Client.Clients.Version1;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PipServices3.Commons.Data;
using System;
using Companies.Data.Version1;

namespace Service.Test.Services.Version1
{
    public class MockCompaniesClientV1: ICompaniesClientV1
    {
        private int _idIdentity = 0;
        private Dictionary<string, CompanyV1> _companiesDict = new Dictionary<string, CompanyV1>();

        public async Task<CompanyV1> CreateCompanyAsync(string correlationId, CompanyV1 company)
        {
            _idIdentity++;

            company.Id = _idIdentity.ToString();
            _companiesDict.Add(company.Id, company);

            return await Task.FromResult(company);
        }

        public async Task<CompanyV1> DeleteCompanyByIdAsync(string correlationId, string id)
        {
            CompanyV1 company = null;

            if (_companiesDict.TryGetValue(id, out CompanyV1 companyV1))
            {
                _companiesDict.Remove(id);
                company = companyV1;
            }

            return await Task.FromResult(company);
        }

        public async Task<DataPage<CompanyV1>> GetCompaniesAsync(string correlationId, FilterParams filter, PagingParams paging, SortParams sort)
        {
            var total = Convert.ToInt64(_companiesDict.Count);
            var skip = Convert.ToInt32(paging.Skip ?? 0);
            var take = Convert.ToInt32(paging.Take ?? total);

            var data = _companiesDict.Select(x => x.Value).Skip(skip).Take(take).ToList();


            DataPage<CompanyV1> dataPage = new DataPage<CompanyV1>(data, total);

            return await Task.FromResult(dataPage);
        }

        public async Task<CompanyV1> GetCompanyByIdAsync(string correlationId, string id)
        {
            CompanyV1 customer = _companiesDict.TryGetValue(id, out CompanyV1 customerV1) ? customerV1 : null;
            return await Task.FromResult(customer);
        }

        public async Task<CompanyV1> UpdateCompanyAsync(string correlationId, CompanyV1 company)
        {
            _companiesDict[company.Id] = company;
            return await Task.FromResult(company);
        }
    }
}
