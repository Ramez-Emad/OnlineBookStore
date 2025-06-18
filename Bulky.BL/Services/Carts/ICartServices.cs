using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Carts
{
    public interface ICartServices
    {
        Task<Cart?> GetCartByUserIdAsync(string key);
        Task<Cart?> CreateOrUpdateUserCartAsync(ProductDetailsDto productDetailsDto , int quantity , Cart? cart = null , TimeSpan? TimeToLive = null);
        Task<bool> DeleteUserCartAsync(string key);

        Task DeleteProductFromUserCart(Cart? cart , int productId);
        Task IncrementProductInUserCart(Cart? cart, ProductDetailsDto product);
        Task DecrementProductInUserCart(Cart? cart, ProductDetailsDto product);

    }
}
