using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Infrastructures
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public Task AddEntities(ICollection<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public TEntity AddEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<TEntity>> GetAllEntitiesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetEntityByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
