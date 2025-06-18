using Bulky.DataAccess.Repository._Generic;
using BulkyWeb.Data;


namespace Bulky.DataAccess.Repository.Categories
{
    public class CategoryRepository(ApplicationDbContext _db) : GenericRepository<Category>(_db), ICategoryRepository
    {
         
    }
}
