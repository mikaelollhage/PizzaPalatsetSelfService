using PizzaPalatsetSelfService.Models;
using PizzaPalatsetSelfService.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace PizzaPalatsetSelfService.Views
{
    public sealed partial class FoodMenuView : Page
    {
        private ObservableCollection<Pizza> allPizzas;
        private ObservableCollection<Drink> allDrinks;
        private ObservableCollection<Food> shoppingCart;
        private Food selectedFood;
        private ContentDialog foodDetails;
        private ContentDialog cashier;
        private Order orderToChange;

        //used to pass on either Pizza or Drink GridView to deselect the item if FoodDetails Back-button is hit. 
        //iF this is not done the item will be added to ShoppingCart even when it shouldn't
        private GridView foodMenu;
        private bool deleteXClicked = false;

        public FoodMenuView()
        {
            this.InitializeComponent();
            allPizzas = FoodViewModel.Instance.AllPizzas;
            allDrinks = FoodViewModel.Instance.AllDrinks;
            shoppingCart = FoodViewModel.Instance.ShoppingCart;
            foodMenu = new GridView();
            //Resets the quantities of all original food objects
            FoodViewModel.Instance.ResetFoodQuantities();
        }
        
        /// <summary>
        /// Factory Pattern creation method
        /// </summary>
        /// <returns></returns>
        public static async Task CreateAsync()
        {
            //Create instance of this class to be able to call next method
            FoodMenuView foodMenuView = new FoodMenuView();
            await foodMenuView.InitializeAsync();
        }
        
        /// <summary>
        /// Factory Pattern Initialize async method
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            //Get external data and return an instance of this calss
            await FoodViewModel.Instance.PopulateFoodLists();
        }

        /// <summary>
        /// Method to run when this frame is navigated to. Calls two methods to prepare the page if there is an active order to be changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            orderToChange = (Order)e.Parameter;
           
            //Only step in if the frame was navigated to together with an open order.
            if (orderToChange != null)
            {
                List<Food> openOrderContents = new List<Food>();
                MergeCurrentOrderLists(openOrderContents);
                SetPageContents(openOrderContents);  
            }
        }

        /// <summary>
        /// Merges the order's two lists into one, to fit in the shoppingcart
        /// </summary>
        /// <param name="openOrderContents"></param>
        private void MergeCurrentOrderLists(List<Food> openOrderContents)
        {
            //Merges the orders two lists to one list
            foreach (Pizza pizza in orderToChange.PizzaOrderContents)
            {
                openOrderContents.Add(pizza);
            }

            foreach (Drink drink in orderToChange.DrinkOrderContents)
            {
                openOrderContents.Add(drink);
            }
        }

        /// <summary>
        /// Sets curerent page's contents based on the order to be changed
        /// </summary>
        /// <param name="openOrderContents"></param>
        private void SetPageContents(List<Food> openOrderContents)
        {
            shoppingCart = FoodViewModel.Instance.FillShoppingCartFromOpenOrder(openOrderContents);
            Txt_CurrentPizzaBasketValue.Text = orderToChange.TotalOrderCost.ToString() + " SEK";
            Bttn_GoToBasket.Content = "Till 'Ändra Beställning'";
            Bttn_DeleteCurrentOrder.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event for when a Food is selected in either the Pizza or Drink GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoodSelecter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Convert SelectedItem to a Food
            selectedFood = ((Food)(((GridView)sender).SelectedItem));
            //Save which gridView was clicked
            foodMenu = (GridView)sender;

            //Avoids a crash if the user clicks between the cells of the pizza or drink grid
            if (selectedFood != null)
            {
                //Calls method to call ContentDialog-Page
                ShowFoodDetails();
            }

            //Reset GridView selection
            ((GridView)sender).SelectedItem = null;
        }

        /// <summary>
        /// Event for when an item in the shoppingCart is tapped 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lv_ShoppingCart_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Only step in if anything BUT the x-button in the listview was tapped
            if (!deleteXClicked)
            {
                //Convert selected item to Food
                selectedFood = ((Food)(((ListView)sender).SelectedItem));
                //Calls method to call ContentDialog-Page
                ShowFoodDetails();
            }
            deleteXClicked = false;
        }

        /// <summary>
        /// Open ContentDialog page with more food details
        /// </summary>
        private async void ShowFoodDetails()
        {
            //Create instance to call constructor and pass on the selected food and the correct GridView
            foodDetails = new FoodDetailsView(selectedFood, foodMenu);
            //Open ContentDialog page
            await foodDetails.ShowAsync();

            //Step in if the GridView has a selection OR if selectedFood has a Food (if the shopping cart has been tapped)
            if (foodMenu.SelectedItem != null || selectedFood != null)
            {
                //Call method to Add or Remove food from the shoppingCart
                FoodViewModel.Instance.AddRemoveFromShoppingCart(selectedFood);
                //Call method to update the pages shopping cart value
                UpdateBasketValue();
            }
        }

        /// <summary>
        /// Calls method to update the current shopping cart value
        /// </summary>
        private void UpdateBasketValue()
        {
            //Update textblock property  with the current shopping cart value
            Txt_CurrentPizzaBasketValue.Text = FoodViewModel.Instance.CurrentBasketValue.ToString() + " SEK";
        }

        /// <summary>
        /// Event to go to the cashier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Bttn_GoToBasket_Click(object sender, RoutedEventArgs e)
        {
            //Variables used in printing of a ContentDialog below
            string orderManipulationString;
            int orderNumberToPrint;

            //Step in if shoppin cart ha sitems in it
            if (shoppingCart.Count!=0)
            {
                //Create instance if CashierView to call constructor
                cashier = new CashierView(shoppingCart, orderToChange);
                //Open Cashier ContentDialog page
                await cashier.ShowAsync();

                //If the order was created Go back to the first page
                //Otherwise stay here
                if (OrderViewModel.Instance.OrderCreatedOrModified)
                {
                    //New order: Get ordernumber stored in OrderViewModel. Current order: Get ordernumber of the order object that was just changed
                    orderNumberToPrint = orderToChange is null ? OrderViewModel.Instance.OrderNumber : orderToChange.OrderNumber;
                    orderManipulationString = orderToChange is null ? "skapats" : "ändrats";

                    //Create anonymous ContentDialog
                    _ = new ContentDialog() 
                    {   
                        Content= $"Din order med Ordernummer {orderNumberToPrint} har nu {orderManipulationString}. \nHåll koll på status för din order på hemskärmen.",
                        CloseButtonText = "OK"
                    }.ShowAsync();

                    ClearShoppingCartAndGoToWelcomePage();
                }
            }
            else
            {
                _ = await new ContentDialog() 
                {   Content = "Din varukorg är tom." ,
                    CloseButtonText="OK"     
                }.ShowAsync();
            }                 
        }

        /// <summary>
        /// Event to go back to WelcomePage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_GoBack_Click(object sender, RoutedEventArgs e)
        {
            ClearShoppingCartAndGoToWelcomePage();
        }

        /// <summary>
        /// Event to display ContentDialog. Delete current order or not?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Bttn_DeleteCurrentOrder_Click(object sender, RoutedEventArgs e)
        {
            //Variable to be used as content in ContentDialog below
            string dialogContent = $"\nDu håller på att ta bort order {orderToChange.OrderNumber}.\n\nVill du gå vidare?";
            
            ContentDialog deleteOrder = new ContentDialog()
            {
                Title = $"Ta bort order",
                Content = dialogContent,
                PrimaryButtonText="Ta bort",
                CloseButtonText="Tillbaka",
            };

            //Set what till happen if PrimaryButton (Delete) is clicked
            deleteOrder.PrimaryButtonClick += DeleteOrder;

            //Open ContentDialog
            await deleteOrder.ShowAsync();
        }

        /// <summary>
        /// Method to start the process of deleting the current order
        /// </summary>
        /// <param name="dialogSender"></param>
        /// <param name="args"></param>
        private void DeleteOrder(ContentDialog dialogSender, ContentDialogButtonClickEventArgs args)
        {
            //Call method to Delete the order locally and in the database
            OrderViewModel.Instance.DeleteCurrentOrder(orderToChange);
            ClearShoppingCartAndGoToWelcomePage();
        }

        /// <summary>
        /// Clear shopping cart and go back to the Welcome Page
        /// </summary>
        private void ClearShoppingCartAndGoToWelcomePage()
        {
            FoodViewModel.Instance.ClearShoppingCart();
            Frame.GoBack();
        }

        /// <summary>
        /// Event for when the shoppingCart-listview imgae is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_DeleteX_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Cast selected item and store in local variable
            selectedFood = Lv_ShoppingCart.SelectedItem as Food;
            //Set variable to true to prevent ListView-tapped event from running
            deleteXClicked = true;
            
            FoodViewModel.Instance.RemoveFoodFromShoppingCart(selectedFood);
            UpdateBasketValue();
        }
    }
}
