using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Dinah.EntityFrameworkCore
{
    /// <summary>Attempt to approximate EF 6 interceptors. NOT ATOMIC WITH TRANSACTIONS</summary>
    public class InterceptableDbContext : DbContext
    {
        public InterceptableDbContext() : base() { }
        public InterceptableDbContext(DbContextOptions options) : base(options) { }

        private List<IDbInterceptor> interceptors { get; } = new List<IDbInterceptor>();
        public void AddInterceptor(IDbInterceptor dbInterceptor) => interceptors.Add(dbInterceptor);
        public void RemoveInterceptor(IDbInterceptor dbInterceptor) => interceptors.Remove(dbInterceptor);

        // IS called by parameterless SaveChanges()
        // IS NOT called by async
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            preSave();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            postSave();

            return result;
        }

        // this is also called from within SaveChangesAsync(CancellationToken)
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            try
            {
                preSave();
				return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            finally
            {
                postSave();
            }
        }

        private void preSave()
        {
            // needed if we're possibly changing an entity between here and SaveChanges(bool)
            ChangeTracker.DetectChanges();

            // before save
            foreach (var interceptor in interceptors)
                interceptor.Executing(this);

            // disable this flag to prevent ChangeTracker from recursively tracking changes made in Executing()
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        private void postSave()
        {
            ChangeTracker.AutoDetectChangesEnabled = true;

            // after save
            foreach (var interceptor in interceptors)
                interceptor.Executed(this);
        }
    }
}
