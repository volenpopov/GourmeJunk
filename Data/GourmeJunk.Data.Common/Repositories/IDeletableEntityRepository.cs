using GourmeJunk.Data.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GourmeJunk.Data.Common.Repositories
{
    public interface IDeletableEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllAsNoTrackingWithDeleted();

        Task<TEntity> GetByIdWithDeletedAsync(params object[] id);

        void HardDelete(TEntity entity);

        void HardDeleteRange(IEnumerable<TEntity> entities);

        void Undelete(TEntity entity);

        Task<int> ExecuteSqlCommandAsync(string command, params object[] parameters);
    }
}
