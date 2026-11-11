using MultasApp.Servicos;
using Microsoft.EntityFrameworkCore;

namespace MultasApp
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Multa> Multas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Multa>().HasKey(p => p.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
