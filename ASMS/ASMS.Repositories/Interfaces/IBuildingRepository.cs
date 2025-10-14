using ASMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetEntityByIdAsync(int id);
        Task<Building> AddAsync(Building building);
        Task<Building> UpdateAsync(Building building);
        Task<Building> GetLastRecord();

    }
}
