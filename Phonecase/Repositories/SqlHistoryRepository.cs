using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public class SqlHistoryRepository:IHistoryRepository
    {
        private readonly PhoneCaseDbContext _context;
        public SqlHistoryRepository(
            PhoneCaseDbContext context)
        {
            _context = context;
            
        }

        public async Task<Purchase?> GetPurchaseByIdAsync(int id)
        {
            return await _context.Purchases
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
        }

        public async Task<IEnumerable<Purchase>> GetPurchaseHistoryAsync(int vendorId, DateTime startdate)
        {
            return await _context.Purchases
                .Where(p => p.VendorId == vendorId && p.PurchaseDate >= startdate)
                .Include(p => p.Product)
                .ThenInclude(m => m.Model)
                .Include(p => p.Product)
                .ThenInclude(cm => cm.CaseManufacturer)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();
        }

        public async Task<Purchase?> EditPurchaseAsync(int id, int quantity, decimal unitPrice)
        {
            var Purchase = await _context.Purchases.FirstOrDefaultAsync(v => v.PurchaseId == id);
            if (Purchase == null)
            {
                return null;
            }
            // Update purchase details
            Purchase.Quantity = quantity;
            Purchase.UnitPrice = unitPrice;

            await _context.SaveChangesAsync();
            return Purchase;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Vendor)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistoryAsync(int vendorId, DateTime startdate)
        {
            return await _context.Payments
                .Where(p => p.VendorId == vendorId && p.PaymentDate >= startdate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> EditPaymentAsync(int id, decimal amount)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.PaymentId == id);

            if (payment == null) return null;

            // Update payment details
            payment.Amount = amount;
            await _context.SaveChangesAsync();
            return payment;
        }

    }
}
