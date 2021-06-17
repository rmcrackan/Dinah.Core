using Microsoft.EntityFrameworkCore;

namespace Dinah.EntityFrameworkCore
{
    public interface IDbInterceptor
    {
        /// <summary>CHANGES ARE STILL VISIBLE HERE. Runs after SaveChanges is called, before execution occurs</summary>
        void Executing(DbContext context);

        /// <summary>Runs after SaveChanges is called, after execution occurs, before SaveChanges returns</summary>
        void Executed(DbContext context);
    }
}
