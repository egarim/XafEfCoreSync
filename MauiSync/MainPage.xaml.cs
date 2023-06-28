using BIT.Data.Sync.Client;
using BIT.Data.Sync.EfCore.Sqlite;
using BIT.Data.Sync.EfCore.SqlServer;
using BIT.EfCore.Sync;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using XafEfCoreSync.Module.BusinessObjects;

namespace MauiSync
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            DbContextOptionsBuilder OptionsBuilder = new DbContextOptionsBuilder();
            const string ConnectionString = "Data Source=MauiData.db;";
            OptionsBuilder.UseSqlite(ConnectionString);

            HttpClient Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44343");

            List<DeltaGeneratorBase> DeltaGenerators = new List<DeltaGeneratorBase>();
            DeltaGenerators.Add(new SqliteDeltaGenerator());
            DeltaGenerators.Add(new SqlServerDeltaGenerator());
            DeltaGeneratorBase[] additionalDeltaGenerators = DeltaGenerators.ToArray();

            ServiceCollection ServiceCollection = new ServiceCollection();
            ServiceCollection.AddEfSynchronization((options) =>
            {
                options.UseSqlite("Data Source=MauiDeltas.db;");
            },
            Client, "MemoryDeltaStore1",
            "Maui",
            additionalDeltaGenerators);


            ServiceCollection.AddEntityFrameworkSqlite();

            var ServiceProvider = ServiceCollection.BuildServiceProvider();

            dataContext = new MauiSyncFrameworkDbContext(OptionsBuilder.Options, ServiceProvider);
            dataContext.Database.EnsureCreated();
            if (dataContext.Blogs.Count() == 0)
            {
                dataContext.Add(new Blog() { Name = "Maui" });
            }
            dataContext.SaveChanges();


            this.BlogsList.ItemsSource = dataContext.Blogs;
        }
        private async void PullClicked(object sender, EventArgs e)
        {
            try 
            {
                await dataContext.PullAsync();
                this.BlogsList.ItemsSource = dataContext.Blogs;
            }
            catch (Exception ex)
            {

                throw;
            }
          
           
        }
        private async void PushClicked(object sender, EventArgs e)
        {
            await dataContext.PushAsync();
        }
        private async void SaveClicked(object sender, EventArgs e)
        {
            dataContext.Add(new Blog() { Name = this.BlogName.Text });
            await dataContext.SaveChangesAsync();
           
        }
        MauiSyncFrameworkDbContext dataContext;
   
    }
}