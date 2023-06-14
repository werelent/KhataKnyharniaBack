using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PracticeWebApp.Models
{
    public class DbContext : IdentityDbContext<User>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }
    }
}
