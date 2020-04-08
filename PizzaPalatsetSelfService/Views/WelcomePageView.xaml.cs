using PizzaPalatsetSelfService.Models;
using PizzaPalatsetSelfService.ViewModels;
using PizzaPalatsetSelfService.Views;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace PizzaPalatsetSelfService
{
    public sealed partial class WelcomePageView : Page
    {
        private ObservableCollection<Order> allOrders;
        public WelcomePageView()
        {
            allOrders = OrderViewModel.Instance.AllActiveOrders;
            this.InitializeComponent();

            //Sets the order status to false. It is set to true when the order is created and then navigation will take us back to this page
            OrderViewModel.Instance.OrderCreatedOrModified = false;
        }

        /// <summary>
        /// Navigates to the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FoodMenuView));
        }

        /// <summary>
        /// Navigates to the menu with an already placed order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grd_AllOpenOrders_Tapped(object sender, TappedRoutedEventArgs e)
        {            
            //Catches if the user clicks around the grid view cell rectangles
            //Do this if a rectangle was clicked
            try
            {
                if ((Order)(((GridView)sender).SelectedItem)!= null)
                {
                    Order orderToChange = (Order)(((GridView)sender).SelectedItem);
                    Frame.Navigate(typeof(FoodMenuView), orderToChange);
                }                
            }
            catch
            {
                //Do nothing if a rectangle wasn't clicked
            }
        }
    }
}
