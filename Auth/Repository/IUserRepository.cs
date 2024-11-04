namespace Auth.Repository;

using Auth.Models;

public interface IUserRepository
{
    User Add(User user);
    User? GetUserByEmail(string email);

    List<User> GetAll();
}