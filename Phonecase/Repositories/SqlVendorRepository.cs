using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public class SqlVendorRepository: IVendorRepository
    {
        private readonly PhoneCaseDbContext _context;
        public SqlVendorRepository(PhoneCaseDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Vendor> CreateVendorAsync(Vendor vendor)
        {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor?> DeleteVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == id);
            if (vendor == null)
            {
                return null;
            }
            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return vendor;

        }

        public async Task<IEnumerable<Payment?>> GetPaymentHistoryByIdAsync(int id)
        {
            var vendor = await _context.Payments
                        .FirstOrDefaultAsync(v => v.VendorId == id);
            if (vendor == null)
            {
                return new List<Payment>();
            }

            var paymentHistory = await _context.Payments
                .Where(p => p.VendorId == id)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return paymentHistory;

           
        }

        public async Task<IEnumerable<Purchase>> GetPurchaseHistoryByIdAsync(int id)
        {
            var vendor = await _context.Purchases
                .FirstOrDefaultAsync(v => v.VendorId == id);

            if (vendor == null)
            {
                return new List<Purchase>(); // Return an empty list instead of null
            }

            var purchaseHistory = await _context.Purchases
                .Where(p => p.VendorId == id)
                .Include(p => p.Product)
                    .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                    .ThenInclude(cm => cm.CaseManufacturer)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return purchaseHistory;
        }

        public async Task<IEnumerable<Vendor>> GetVendorAsync()
        {
            var vendors = await _context.Vendors.ToListAsync();
            return vendors;
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id)
        {
            var vendor = await _context.Vendors
                        .FirstOrDefaultAsync(v => v.VendorId == id);
            if (vendor == null)
            {
                return null;
            }
            return vendor;
        }
    }
}
