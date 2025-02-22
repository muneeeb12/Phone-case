﻿using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories {
    public class SqlVendorRepository : IVendorRepository {
        private readonly PhoneCaseDbContext _context;

        public SqlVendorRepository(PhoneCaseDbContext dbContext) {
            _context = dbContext;
        }

        public async Task<Vendor> CreateVendorAsync(Vendor vendor) {
            await _context.Vendors.AddAsync(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor?> DeleteVendorAsync(int id) {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == id);
            if (vendor == null) {
                return null;
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistoryByIdAsync(int id) {
            var paymentHistory = await _context.Payments
                .Where(p => p.VendorId == id)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return paymentHistory;
        }

        public async Task<IEnumerable<Purchase>> GetPurchaseHistoryByIdAsync(int id) {
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

        public async Task<IEnumerable<Vendor>> GetVendorAsync() {
            return await _context.Vendors.ToListAsync();
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id) {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.VendorId == id);
        }

        public async Task<Vendor?> UpdateVendorAsync(Vendor vendor) {
            var vendorFind = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorId == vendor.VendorId);
            if (vendorFind == null) {
                return null;
            }

            vendorFind.Name = vendor.Name;
            vendorFind.TotalCredit = vendor.TotalCredit; // Fixed typo in property name
            await _context.SaveChangesAsync();
            return vendorFind;
        }
    }
}