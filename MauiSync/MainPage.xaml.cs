using BIT.Data.Sync.Client;
using BIT.Data.Sync.EfCore.Sqlite;
using BIT.Data.Sync.EfCore.SqlServer;
using BIT.EfCore.Sync;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
            //this.dataContext=GetContext();
            using (var context= GetContext())
            {
                //context.Database.EnsureCreated();
                if (context.Blogs.Count() == 0)
                {
                    context.Add(new Blog() { Name = "Maui" });
                    context.SaveChanges();
                }



                context.Blogs.ToList().ForEach((item) =>
                {
                    blogs.Add(item);
                });
            }
           

            this.BlogsList.ItemsSource = blogs;
        }

        private MauiSyncFrameworkDbContext GetContext()
        {
            DbContextOptionsBuilder OptionsBuilder = new DbContextOptionsBuilder();
            const string ConnectionString = "Data Source=MauiData.db;";
            OptionsBuilder.UseSqlite(ConnectionString);

            HttpClient Client = new HttpClient();
            //Local Computer
            //Client.BaseAddress = new Uri("https://localhost:44343

            //Joche Dev Tunnel
            Client.BaseAddress = new Uri("https://hj6z9022-44343.use.devtunnels.ms/");
            
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

            MauiSyncFrameworkDbContext mauiSyncFrameworkDbContext = new MauiSyncFrameworkDbContext(OptionsBuilder.Options, ServiceProvider);
            mauiSyncFrameworkDbContext.Database.EnsureCreated();
            return mauiSyncFrameworkDbContext;
        }

        private async void PullClicked(object sender, EventArgs e)
        {
            try
            {
                using (var Context = GetContext())
                { 
                    await Context.PullAsync();
                }
                UpdateData();
            }
            catch (Exception ex)
            {

                throw;
            }
            await ShowMessage("Pull....Done!!");

        }

        private void UpdateData()
        {
            blogs.Clear();
            using (var Context = GetContext())
            {
                var Count = Context.Blogs.Count();
                Context.Blogs.ToList().ForEach((item) =>
                {

                    blogs.Add(item);

                });
            }
                
        }

        private async void PushClicked(object sender, EventArgs e)
        {
            using (var Context = GetContext())
            {
                await Context.PushAsync();
              
            }
            await ShowMessage("Push....Done!!");
        }
        async Task ShowMessage(string Message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            string text = Message;
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(text, duration, fontSize);

            await toast.Show(cancellationTokenSource.Token);
        }
        private async void SaveClicked(object sender, EventArgs e)
        {
            using (var Context = GetContext())
            {
                Context.Add(new Blog() { Name = this.BlogName.Text });
                await Context.SaveChangesAsync();
               
            }
            UpdateData();
            this.BlogName.Text = null;
            await ShowMessage("Save....Done!!");
        }
   
   
    }
}