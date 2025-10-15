using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Infrastructures
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);

        //Task<ICollection<TEntity>> GetAllEntitiesAsync();

        //Task AddEntities(ICollection<TEntity> entities);
    }
}
