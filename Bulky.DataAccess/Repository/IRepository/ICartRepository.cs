using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
  
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string key);
        Task<Cart?> CreateOrUpdateCartAsync(Cart cart, TimeSpan? TimeToLive = null);

        Task<bool> DeleteCartAsync(string key);
    }
}
