using PizzaPalatsetSelfService.Models;
using PizzaPalatsetSelfService.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PizzaPalatsetSelfService.Views
{
    public sealed partial class CashierView : ContentDialog
    {
        private ObservableCollection<Pizza> orderedPizzas;
        private ObservableCollection<Drink> orderedDrinks;
        private Order orderToChange;

        public CashierView(ObservableCollection<Food> orderedItems, Order orderToChange)
        {
            this.orderToChange = orderToChange;
            //Calls method to split the shopping cart from Food to Pizza and Drink. Saves result in tuple of ObservableCollections
            (orderedPizzas,orderedDrinks) = OrderViewModel.Instance.SplitShoppingCart(orderedItems);
            this.InitializeComponent();
            SetControls();
        }

        /// <summary>
        /// Sets values for the page's controls
        /// </summary>
        public void SetControls()
        {
            Txt_TotalOrderCost.Text = OrderViewModel.Instance.TotalOrderCost.ToString() + " SEK";

            //set button content depending on of the current order has already been placed before or not
            Bttn_PlaceOrder.Content = orderToChange != null ? "Ändra ordern" : "Lägg beställning";
            //if there are no pizzas in the order, hide the pizza controls
            if (orderedPizzas.Count==0)
            {
                Lv_PizzasList.Visibility = Visibility.Collapsed;
                Line_UnderPizzas.Visibility = Visibility.Collapsed;
                Txt_PizzasLabel.Text = "";
            }
            //if there are no drinks in the order, hide the pizza controls
            if (orderedDrinks.Count == 0)
            {
                Lv_DrinksList.Visibility = Visibility.Collapsed;
                Line_UnderDrinks.Visibility = Visibility.Collapsed;                
                Txt_DrinksLabel.Text = "";
            }
        }

        /// <summary>
        /// Event for when the GoBack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_GoBack_Click(object sender, RoutedEventArgs e)
        {
            //Clear the lists populating this page
            OrderViewModel.Instance.ClearOrderLists();
            Hide();
        }

        /// <summary>
        /// Event for button to place the presneted order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Bttn_PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            //Step in if existing order
            if (orderToChange != null)
            {
                await OrderViewModel.Instance.ModifySelectedOrder(orderToChange);
            }
            //step in if new order
            else
            {
                await OrderViewModel.Instance.CreateOrder();
            }
            Hide();
        }
    }
}
