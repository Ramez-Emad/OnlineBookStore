namespace Bulky.DataAccess.Repository.Users
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
    }
}