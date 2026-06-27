using AICodingBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace AICodingBlog.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
}
