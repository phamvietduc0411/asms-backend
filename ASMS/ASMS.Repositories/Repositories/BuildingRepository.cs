using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class BuildingRepository : GenericRepository<Building>, IBuildingRepository
    {
        public BuildingRepository(
           VstorageContext context,ILogger logger) : base(context,logger)
        {
        }

        public async Task<Building> GetLastRecord()
        {
            if (_context == null || _context.Buildings == null)
                throw new InvalidOperationException("Database context or Building DbSet is not initialized.");

            var lastBuilding = await _context.Buildings
    .Where(b => b.BuildingCode != null && b.BuildingCode.StartsWith("BLD"))
    .OrderByDescending(b => b.BuildingCode)
    .FirstOrDefaultAsync();

            return lastBuilding;
        }
        public async Task<Building?> GetByCodeAsync(string buildingCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(b => b.BuildingCode == buildingCode);
        }
    }
}
