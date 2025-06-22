using Bulky.DataAccess.Exceptions;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _db.ApplicationUsers.Include(u => u.Company).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            var user = await _db.ApplicationUsers
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) 
                throw new UserNotFoundException(id);

            return user;
        }
    }
}
