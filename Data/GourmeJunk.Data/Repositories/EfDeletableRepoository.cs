using GourmeJunk.Data.Common.Models;
using GourmeJunk.Data.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GourmeJunk.Data.Repositories
{
    public class EfDeletableEntityRepository<TEntity> : EfRepository<TEntity>, IDeletableEntityRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        public EfDeletableEntityRepository(GourmeJunkDbContext context)
            : base(context)
        { }

        public override IQueryable<TEntity> All() => base.All().Where(x => !x.IsDeleted);

        public override IQueryable<TEntity> AllAsNoTracking() => base.AllAsNoTracking().Where(x => !x.IsDeleted);

        public IQueryable<TEntity> AllWithDeleted() => base.All().IgnoreQueryFilters();

        public IQueryable<TEntity> AllAsNoTrackingWithDeleted() => base.AllAsNoTracking().IgnoreQueryFilters();

        public Task<TEntity> GetByIdWithDeletedAsync(params object[] id)
        {
            var byIdPredicate = EfExpressionHelper.BuildByIdPredicate<TEntity>(this.Context, id);

            return this.AllWithDeleted().FirstOrDefaultAsync(byIdPredicate);
        }

        public void HardDelete(TEntity entity) => base.Delete(entity);

        public void HardDeleteRange(IEnumerable<TEntity> entities) => base.DeleteRange(entities);

        public void Undelete(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.DeletedOn = null;

            this.Update(entity);
        }

        public override void Delete(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;

            this.Update(entity);
        }

        public async Task<int> ExecuteSqlCommandAsync(string command, params object[] parameters)
        {            
            return await base.Context.Database.ExecuteSqlCommandAsync(command, parameters);
        }
    }
}
