using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dinah.EntityFrameworkCore
{
	// from: https://www.benday.com/2017/12/19/ef-core-2-0-migrations-without-hard-coded-connection-strings/

	/// <typeparam name="TContext">
	/// Child Context:
	/// not needed : public MyTestContext() : base() { }
	/// IS NEEDED :  public MyTestContext(DbContextOptions<TContext> options) : base(options) { }
	///
	/// OnConfiguring() is the standard way to do use a hardcoded conn str. Works with migrations and works with standard code.
	/// Since we're using the factory to load the conn str from a file instead, we don't need OnConfiguring()
	///     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=sample.db");
	/// </typeparam>
	public abstract class DesignTimeDbContextFactoryBase<TContext> :
        IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        protected abstract void UseDatabaseEngine(DbContextOptionsBuilder optionsBuilder, string connectionString);

        // entry point for Add-Migration and Update-Database
        public TContext CreateDbContext(string[] args)
			=> Create(new FileInfo(Directory.GetCurrentDirectory()));

        // entry point for standard code call
        public TContext Create()
			=> Create(new FileInfo(AppContext.BaseDirectory));

		public TContext Create(FileInfo fileInfo)
        {
            var connectionStringProp = typeof(TContext).Name;
            var rawConnectionString = new ConfigurationBuilder()
                .SetBasePath(fileInfo.FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .GetConnectionString(connectionStringProp);
            var connectionString = Environment
                .ExpandEnvironmentVariables(rawConnectionString)
                .Replace("%DESKTOP%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            return Create(connectionString);
        }

        public TContext Create(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

            var builder = new DbContextOptionsBuilder<TContext>();

            UseDatabaseEngine(builder, connectionString);

            var context = CreateNewInstance(builder.Options);

            // with reflection: var context = (TContext)Activator.CreateInstance(t, builder.Options);

            #region // Migrate() explanation
            // context.Database.Migrate():
            // needed by user's for initial db creation and 1st run after migration.
            // use .Migrate() within using()
            // don't use .Migrate() within add-migration/update-database. migration does the same thing so it calling Migrate() would do so twice.
            //
            // ONLY use context.Database.Migrate(); DO NOT use context.Database.EnsureCreated();
            // .EnsureCreated() will create a database but will run migration scripts. in fact, if created this way, the db cannot ever use migrations
            #endregion
            context.Database.Migrate();

            return context;
        }
    }
}
