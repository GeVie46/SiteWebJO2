using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteWebJO2.Models;

namespace SiteWebJO2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {
    }
        public DbSet<JoSession> JoSessions { get; set; }
        public DbSet<JoTicketPack> JoTicketPacks { get; set; }
        public DbSet<JoTicket> JoTickets { get; set; }
        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

       
}
}
