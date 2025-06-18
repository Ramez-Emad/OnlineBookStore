using Bulky.DataAccess.Exceptions;
using StackExchange.Redis;
using System.Text.Json;

namespace Bulky.DataAccess.Repository.Carts
{
    
    public class CartRepository(IConnectionMultiplexer connection) : ICartRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();

        public async Task<Cart?> CreateOrUpdateCartAsync(Cart Cart, TimeSpan? TimeToLive = null)
        {
            var JsonCart = JsonSerializer.Serialize(Cart);

            var IsCreatedOrUpdated = await _database.StringSetAsync(Cart.Id, JsonCart, TimeToLive ?? TimeSpan.FromDays(30));

            if (IsCreatedOrUpdated)
                return await GetCartAsync(Cart.Id);

            return null;

        }

        public async Task<bool> DeleteCartAsync(string key) => await _database.KeyDeleteAsync(key);

        public async Task<Cart?> GetCartAsync(string key)
        {
            var Cart = await _database.StringGetAsync(key);
            if (Cart.IsNullOrEmpty)
                return null;
            return JsonSerializer.Deserialize<Cart>(Cart!);
        }

    }
}
