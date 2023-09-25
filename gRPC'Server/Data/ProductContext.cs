using gRPCServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations.Schema;

namespace gRPCServer.Data
{
    public class ProductContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public ProductContext(DbContextOptions<ProductContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure().TrustServerCertificate(true));
        }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<LoginDetails> LoginDetail { get; set; }
    }
    
}
