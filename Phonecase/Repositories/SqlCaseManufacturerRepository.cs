using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public class SqlCaseManufacturerRepository: ICaseManufacturerRepository
    {
        private readonly PhoneCaseDbContext context;

        public SqlCaseManufacturerRepository(PhoneCaseDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CaseManufacturer>> GetAllAsync()
        {
            return await context.CaseManufacturers.ToListAsync();
        }

        public async Task<CaseManufacturer> CreateAsync(CaseManufacturer caseManufacturer)
        {
            await context.CaseManufacturers.AddAsync(caseManufacturer);
            await context.SaveChangesAsync();
            return caseManufacturer;
        }

        public async Task<CaseManufacturer?> DeleteAsync(int id)
        {
            var caseManufacturer = await context.CaseManufacturers.FindAsync(id);
            if (caseManufacturer == null)
            {
                return null;
            }

            context.CaseManufacturers.Remove(caseManufacturer);
            await context.SaveChangesAsync();
            return caseManufacturer;

        }
    }
}
