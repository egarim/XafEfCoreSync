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
            blogs.Add(new Blog() { Name = "Test" });
            blogs.Add(new Blog() { Name = "Test 1" });
            this.BlogsList.ItemsSource = this.blogs;
        }
        List<Blog> blogs = new List<Blog>();
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}