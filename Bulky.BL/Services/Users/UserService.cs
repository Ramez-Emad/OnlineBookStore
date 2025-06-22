using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAllUsersAsync();
        }

        public  async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _unitOfWork.UserRepository.GetUserByIdAsync(id);
        }

        public async Task ToggleLockUser(string id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
                {
                    user.LockoutEnd = DateTime.Now;
                }
                else
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            var userfromDb = await _unitOfWork.UserRepository.GetUserByIdAsync(user.Id);

            if (userfromDb != null)
            {
                userfromDb.Role = user.Role;
                userfromDb.CompanyId = user.CompanyId;
            }
            
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
