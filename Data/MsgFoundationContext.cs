using Microsoft.EntityFrameworkCore;
using MsgFoundation.Models;

namespace MsgFoundation.Data
{
    public class MsgFoundationContext : DbContext
    {
        public MsgFoundationContext(DbContextOptions<MsgFoundationContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Observation> Observations => Set<Observation>();
        public DbSet<Credit> Credits => Set<Credit>();
    }   
}
