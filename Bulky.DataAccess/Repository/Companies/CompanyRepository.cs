
using Bulky.DataAccess.Repository._Generic;
using BulkyWeb.Data;

namespace Bulky.DataAccess.Repository.Companies
{
    public class CompanyRepository(ApplicationDbContext _db) : GenericRepository<Company>(_db) , ICompanyRepository
    {
    }
}
