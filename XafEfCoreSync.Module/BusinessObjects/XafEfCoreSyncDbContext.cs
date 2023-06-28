using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using BIT.EfCore.Sync;
using Microsoft.Extensions.DependencyInjection;
using BIT.Data.Sync.EfCore.SqlServer;
using Microsoft.Extensions.Options;
using BIT.Data.Sync.EfCore.Sqlite;

namespace XafEfCoreSync.Module.BusinessObjects;

// This code allows our Model Editor to get relevant EF Core metadata at design time.
// For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
public class XafEfCoreSyncContextInitializer : DbContextTypesInfoInitializerBase {
	protected override DbContext CreateDbContext() {
		var optionsBuilder = new DbContextOptionsBuilder<XafEfCoreSyncEFCoreDbContext>()
            .UseSqlServer(";")
            .UseChangeTrackingProxies()
            .UseObjectSpaceLinkProxies();
        return new XafEfCoreSyncEFCoreDbContext(optionsBuilder.Options);
	}
}
//This factory creates DbContext for design-time services. For example, it is required for database migration.
public class XafEfCoreSyncDesignTimeDbContextFactory : IDesignTimeDbContextFactory<XafEfCoreSyncEFCoreDbContext> {
	public XafEfCoreSyncEFCoreDbContext CreateDbContext(string[] args) {
		throw new InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
		//var optionsBuilder = new DbContextOptionsBuilder<XafEfCoreSyncEFCoreDbContext>();
		//optionsBuilder.UseSqlServer("Integrated Security=SSPI;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=XafEfCoreSync");
        //optionsBuilder.UseChangeTrackingProxies();
        //optionsBuilder.UseObjectSpaceLinkProxies();
		//return new XafEfCoreSyncEFCoreDbContext(optionsBuilder.Options);
	}
}
[TypesInfoInitializer(typeof(XafEfCoreSyncContextInitializer))]
public class XafEfCoreSyncEFCoreDbContext : SyncFrameworkDbContext
{
	public XafEfCoreSyncEFCoreDbContext(DbContextOptions<XafEfCoreSyncEFCoreDbContext> options) : base(options,null)
	{
        List<DeltaGeneratorBase> DeltaGenerators = new List<DeltaGeneratorBase>();
        DeltaGenerators.Add(new SqlServerDeltaGenerator());
        DeltaGenerators.Add(new SqliteDeltaGenerator());
        DeltaGeneratorBase[] additionalDeltaGenerators = DeltaGenerators.ToArray();

        HttpClient Client = new HttpClient();
        Client.BaseAddress =new Uri("https://localhost:44343");
        ServiceCollection ServiceCollectionMaster = new ServiceCollection();
        //ServiceCollectionMaster.AddEfSynchronization((options) => { options.UseInMemoryDatabase("MemoryDb2"); }, Client, "MemoryDeltaStore1", "Master", additionalDeltaGenerators);
        ServiceCollectionMaster.AddEfSynchronization((options) => { options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=XafEfMasterDeltas;Trusted_Connection=True;");  }, Client, "MemoryDeltaStore1", "Master", additionalDeltaGenerators);
        ServiceCollectionMaster.AddEntityFrameworkSqlServer();
        ServiceCollectionMaster.AddEntityFrameworkProxies();
        ServiceCollectionMaster.AddXafServiceProviderContainer();
        this.serviceProvider = ServiceCollectionMaster.BuildServiceProvider();


    }
    //public DbSet<ModuleInfo> ModulesInfo { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      



        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=XafEfMaster;Trusted_Connection=True;");
        optionsBuilder.UseChangeTrackingProxies();
        optionsBuilder.UseObjectSpaceLinkProxies();




        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
    }
    public DbSet<Blog> Blogs { get; set; }
}
