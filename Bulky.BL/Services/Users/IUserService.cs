using Bulky.DataAccess.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Users
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task <ApplicationUser?> GetUserByIdAsync(string id);
        Task ToggleLockUser (string id);
        Task UpdateUser(ApplicationUser user);
    }
}
