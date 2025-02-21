using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public class SqlManagementRepository: IManagementRepository
    {
        private readonly PhoneCaseDbContext context;

        public SqlManagementRepository(PhoneCaseDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CaseManufacturer>> GetAllCompanyAsync()
        {
            return await context.CaseManufacturers.ToListAsync();
        }

        public async Task<CaseManufacturer> CreateCompanyAsync(CaseManufacturer caseManufacturer)
        {
            await context.CaseManufacturers.AddAsync(caseManufacturer);
            await context.SaveChangesAsync();
            return caseManufacturer;
        }

        public async Task<CaseManufacturer?> DeleteCompanyAsync(int id)
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

        public async Task<PhoneModel> CreateModelAsync(PhoneModel model)
        {
            await context.PhoneModels.AddAsync(model);
            await context.SaveChangesAsync();
            return model;
        }

        public async Task<IEnumerable<PhoneModel>> GetAllModelAsync()
        {
            return await context.PhoneModels.ToListAsync();
        }
    }
}
