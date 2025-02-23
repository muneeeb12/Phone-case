using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phonecase.Repositories
{
    public class SqlManagementRepository : IManagementRepository
    {
        private readonly PhoneCaseDbContext _context;

        public SqlManagementRepository(PhoneCaseDbContext context)
        {
            _context = context;
        }

        // Get all case manufacturers
        public async Task<IEnumerable<CaseManufacturer>> GetAllCompanyAsync()
        {
            return await _context.CaseManufacturers.ToListAsync();
        }

        // Create a new case manufacturer
        public async Task<CaseManufacturer> CreateCompanyAsync(CaseManufacturer caseManufacturer)
        {
            if (caseManufacturer == null)
            {
                throw new ArgumentNullException(nameof(caseManufacturer), "Case manufacturer cannot be null.");
            }

            await _context.CaseManufacturers.AddAsync(caseManufacturer);
            await _context.SaveChangesAsync();
            return caseManufacturer;
        }

        // Delete a case manufacturer by ID
        public async Task<CaseManufacturer?> DeleteCompanyAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }

            var caseManufacturer = await _context.CaseManufacturers.FindAsync(id);
            if (caseManufacturer == null)
            {
                return null; // Case manufacturer not found
            }

            _context.CaseManufacturers.Remove(caseManufacturer);
            await _context.SaveChangesAsync();
            return caseManufacturer;
        }

        // Create a new phone model
        public async Task<PhoneModel> CreateModelAsync(PhoneModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Phone model cannot be null.");
            }

            await _context.PhoneModels.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        // Get all phone models
        public async Task<IEnumerable<PhoneModel>> GetAllModelAsync()
        {
            return await _context.PhoneModels.ToListAsync();
        }
    }
}