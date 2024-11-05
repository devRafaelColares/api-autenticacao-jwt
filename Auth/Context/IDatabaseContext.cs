namespace Auth.Context;

using Auth.Models;
using Microsoft.EntityFrameworkCore;

public interface IDatabaseContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    public int SaveChanges();
}