namespace Bulky.DataAccess.Repository.Carts
{
  
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string key);
        Task<Cart?> CreateOrUpdateCartAsync(Cart cart, TimeSpan? TimeToLive = null);
        Task<bool> DeleteCartAsync(string key);
    }
}
