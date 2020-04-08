using System;
using Windows.UI.Xaml.Controls;

namespace PizzaPalatsetSelfService.Views
{
    public sealed partial class LoadExternalDataView : Page
    {
        public LoadExternalDataView()
        {
            this.InitializeComponent();
            LoadExternalData();
        }
        /// <summary>
        /// Begins the process of loading all external data
        /// </summary>
        private async void LoadExternalData()
        {
            //Factory Pattern starting method 
            await FoodMenuView.CreateAsync();
            //Go forward when all external data is loaded
            Frame.Navigate(typeof(WelcomePageView));
        }
    }
}
