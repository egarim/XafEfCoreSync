﻿using BIT.Data.Sync;
using BIT.Data.Sync.Client;
using BIT.Data.Sync.EfCore.SQLite;
using BIT.Data.Sync.EfCore.SqlServer;
using BIT.Data.Sync.Imp;
using BIT.EfCore.Sync;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XafEfCoreSync.Module.BusinessObjects;
using static DevExpress.Office.Drawing.LazyGroupBrush;

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

            
        }
        protected async override void OnAppearing()
        {
            await CreateInstanceAsync();
            base.OnAppearing();
        }
        public Task Initialize { get; }
        private async Task CreateInstanceAsync()
        {
            await CheckWriteStoragePermission();

            Debug.WriteLine($"DeltasPath:{DeltasPath}");
            Debug.WriteLine($"DataPath:{DataPath}");

           
            using (var context = GetContext())
            {
                
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

        public async Task<PermissionStatus> CheckWriteStoragePermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            return status;
        }
     
        private MauiSyncFrameworkDbContext GetContext()
        {

            var AndroidPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            DbContextOptionsBuilder OptionsBuilder = new DbContextOptionsBuilder();
            //const string ConnectionString = $"Data Source=MauiData.db;";

            string ConnectionString;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var AndroidDataPath = Path.Combine(AndroidPath, "MauiData.db");
                ConnectionString = $"Data Source={AndroidDataPath};";
            }
            else
            {
                ConnectionString = $"Data Source={DataPath};";
            }


            
           

            OptionsBuilder.UseSqlite(ConnectionString);

            HttpClient Client = new HttpClient();

            //Local Computer
            Client.BaseAddress = new Uri("https://localhost:44343");

            //Joche Dev Tunnel
            //Client.BaseAddress = new Uri("https://hj6z9022-44343.use.devtunnels.ms/");
            
            List<DeltaGeneratorBase> DeltaGenerators = new List<DeltaGeneratorBase>();
            DeltaGenerators.Add(new SqliteDeltaGenerator());
            DeltaGenerators.Add(new SqlServerDeltaGenerator());
            DeltaGeneratorBase[] additionalDeltaGenerators = DeltaGenerators.ToArray();

            ServiceCollection ServiceCollection = new ServiceCollection();

            YearSequencePrefixStrategy implementationInstance = new YearSequencePrefixStrategy();
            ServiceCollection.AddSingleton(typeof(ISequencePrefixStrategy), implementationInstance);
            ServiceCollection.AddSingleton(typeof(ISequenceService), typeof(EfSequenceService));

            ServiceCollection.AddEfSynchronization((options) =>
            {
                string ConnectionString;
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    var AndroidDataDelta = Path.Combine(AndroidPath, "MauiDelta.db");
                    ConnectionString = $"Data Source={AndroidDataDelta};";
                }
                else
                {
                    ConnectionString = $"Data Source={DeltasPath};";
                }

                options.UseSqlite(ConnectionString);
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