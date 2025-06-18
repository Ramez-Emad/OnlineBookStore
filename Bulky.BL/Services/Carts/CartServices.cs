using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository.Carts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Carts
{
    public class CartServices(ICartRepository _cartRepository) : ICartServices
    {
        private double GetPrice(ProductDetailsDto product, int quantity)
        {
            return quantity switch
            {
                >= 100 => product.Price100,
                >= 50 => product.Price50,
                _ => product.Price
            };
        }
        private CartItem CreateCartItem(ProductDetailsDto product, int quantity)
        {
            return new CartItem
            {
                ProductId = product.Id,
                Title = product.Title,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = quantity,
                Price = GetPrice(product, quantity)
            };
        }


        public async Task<Cart?> CreateOrUpdateUserCartAsync(ProductDetailsDto productDetailsDto, int quantity, Cart? cart = null, TimeSpan? TimeToLive = null)
        {
            cart = cart ?? new Cart();

            cart.Items ??= new List<CartItem>();

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productDetailsDto.Id);

            if (existingItem != null)
            {
                cart.TotalCost -= existingItem.Price * existingItem.Quantity;
                existingItem.Quantity += quantity;
                existingItem.Price = GetPrice(productDetailsDto, existingItem.Quantity);
                cart.TotalCost += existingItem.Price * existingItem.Quantity;
            }
            else
            {
                var item = CreateCartItem(productDetailsDto, quantity);
                cart.Items.Add(item);
                cart.TotalCost += item.Price * item.Quantity;
            }
            return await _cartRepository.CreateOrUpdateCartAsync(cart);
        }

        public async Task<bool> DeleteUserCartAsync(string key)
        {
            return await _cartRepository.DeleteCartAsync(key);
        }

        public async Task<Cart?> GetCartByUserIdAsync(string key)
        {
            return await _cartRepository.GetCartAsync(key);
        }

        public async Task DeleteProductFromUserCart(Cart? cart, int productId)
        {
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                cart.TotalCost -= (item.Quantity * item.Price);
                cart.Items.Remove(item);
                if (cart.Items.Count == 0)
                {
                    await _cartRepository.DeleteCartAsync(cart.Id);
                    return;
                }
            }
            await _cartRepository.CreateOrUpdateCartAsync(cart);
        }

        public async Task IncrementProductInUserCart(Cart? cart, ProductDetailsDto product)
        {
            if (cart == null || product == null) return ;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);

            if (item != null)
            {
                cart.TotalCost -= (item.Price * item.Quantity);

                item.Quantity++;

                item.Price = GetPrice(product, item.Quantity);

                cart.TotalCost += (item.Price * item.Quantity);

            }
            await _cartRepository.CreateOrUpdateCartAsync(cart);
        }

        public async Task DecrementProductInUserCart(Cart? cart, ProductDetailsDto product)
        {
            if (cart == null || product == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);

            if (item != null)
            {
                cart.TotalCost -= (item.Price * item.Quantity);

                if (item.Quantity > 1)
                {
                    item.Quantity--;

                    item.Price = GetPrice(product, item.Quantity);

                    cart.TotalCost += (item.Price * item.Quantity);

                }
                else
                {

                    cart.Items.Remove(item);
                    if (cart.Items.Count == 0)
                    {
                        await _cartRepository.DeleteCartAsync(cart.Id);
                        return;
                    }
                }
            }
            await _cartRepository.CreateOrUpdateCartAsync(cart);
        }
    }
}
