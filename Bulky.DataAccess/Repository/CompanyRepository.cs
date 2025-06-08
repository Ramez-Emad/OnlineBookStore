using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository(ApplicationDbContext _db) : Repository<Company>(_db) , ICompanyRepository
    {
    }
}
