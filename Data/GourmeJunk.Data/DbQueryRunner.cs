using System;
using System.Threading.Tasks;
using GourmeJunk.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace GourmeJunk.Data
{
    public class DbQueryRunner : IDbQueryRunner
    {
        public DbQueryRunner(GourmeJunkDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public GourmeJunkDbContext Context { get; set; }

        public Task RunQueryAsync(string query, params object[] parameters)
        {
            return this.Context.Database.ExecuteSqlCommandAsync(query, parameters);
        }

        public void Dispose()
        {
            this.Context?.Dispose();
        }
    }
}
