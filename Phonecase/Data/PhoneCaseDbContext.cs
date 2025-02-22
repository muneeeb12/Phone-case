using Microsoft.EntityFrameworkCore;
using Phonecase.Models;

namespace Phonecase.Data
{
    public class PhoneCaseDbContext : DbContext
    {
        public PhoneCaseDbContext(DbContextOptions<PhoneCaseDbContext> options) : base(options) { }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<CaseManufacturer> CaseManufacturers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PhoneModel> PhoneModels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Payment> Payments { get; set; }

    }
}
