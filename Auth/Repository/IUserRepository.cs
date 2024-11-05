namespace Auth.Repository;

using Auth.Models;

public interface IUserRepository
{
    User Add(User user);
    User? GetUserByEmail(string email);
    List<User> GetAll();
    User? GetById(int id);
    void Update(User user);
    void Delete(int id);
}