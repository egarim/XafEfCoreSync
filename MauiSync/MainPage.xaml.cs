using BIT.Data.Sync.Client;
using BIT.Data.Sync.EfCore.Sqlite;
using BIT.Data.Sync.EfCore.SqlServer;
using BIT.EfCore.Sync;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XafEfCoreSync.Module.BusinessObjects;

namespace MauiSync
{
    public partial class MainPage : ContentPage
    {
        public static string DeltasPath =>Path.Combine(FileSystem.AppDataDirectory, "MauiDeltas.db");
        public static string DataPath => Path.Combine(FileSystem.AppDataDirectory, "MauiData.db");
        ObservableCollection <Blog> blogs=new ObservableCollection<Blog>();
        public MainPage()
        {
            InitializeComponent();

            Debug.WriteLine($"DeltasPath:{DeltasPath}");
            Debug.WriteLine($"DataPath:{DataPath}");
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
                dataContext.SaveChanges();
            }



            dataContext.Blogs.ToList().ForEach((item) =>
            {
                blogs.Add(item);
            });

            this.BlogsList.ItemsSource = blogs;
        }
        private async void PullClicked(object sender, EventArgs e)
        {
            try 
            {
                await dataContext.PullAsync();
                dataContext.Blogs.ToList().ForEach((item) =>
                {
                    if(blogs.Contains(item)==false)
                        blogs.Add(item);
                });
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