using BIT.EfCore.Sync;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafEfCoreSync.Module.BusinessObjects;

namespace MauiSync
{
    public class MauiSyncFrameworkDbContext : SyncFrameworkDbContext
    {
        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="TestSyncFrameworkDbContext" /> class. The
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />
        /// method will be called to configure the database (and other options) to be used for this context.
        /// </para>
        /// </summary>
        protected MauiSyncFrameworkDbContext()
        {

        }

        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="TestSyncFrameworkDbContext" /> class using the specified options.
        /// The <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" /> method will still be called to allow further
        /// configuration of the options.
        /// </para>
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public MauiSyncFrameworkDbContext(DbContextOptions options, IServiceProvider SyncFrameworkServiceCollection) : base(options, SyncFrameworkServiceCollection)
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Blog> Blogs { get; set; }
    }
}
