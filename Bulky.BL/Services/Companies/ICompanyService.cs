
using Bulky.BL.Models.Companies;

namespace Bulky.BL.Services.Companies
{
    public interface ICompanyService
    {
        public Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        public Task<CompanyDto?> GetCompanyByIdAsync(int? id);
        public Task<int> CreateCompanyAsync(CompanyDto company);
        public Task<int> UpdateCompanyAsync(CompanyDto company);
        public Task<bool> DeleteCompanyAsync(int? id);
    }
}
