using PizzaPalatsetSelfService.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace PizzaPalatsetSelfService.Views
{
    public sealed partial class FoodDetailsView : ContentDialog
    {
        private Food selectedFood;
        private GridView foodMenu;

        public FoodDetailsView(Food selectedFood, GridView foodMenu)
        {
            this.InitializeComponent();
            this.selectedFood = selectedFood;
            //Need the foodMenu GridView to deselect an item if the back-button is hit here. Otherwise the item will be added to the shopping cart anyway.
            this.foodMenu = foodMenu;
            SetControls(selectedFood);
        }
        /// <summary>
        /// Sets values for the controls on this page
        /// </summary>
        /// <param name="selectedFood"></param>
        private void SetControls(Food selectedFood)
        {
            if (selectedFood is Drink && selectedFood.Quantity==0)
            {
                Bttn_FoodToOrFromBasket.Content = "Lägg till Dryck";
            }            

            //Set image source for the SelectedFood image control
            BitmapImage image = new BitmapImage();
            Uri uri = new Uri(this.BaseUri, selectedFood.Image);
            image.UriSource = uri;
            Img_SelectedFoodImage.ImageSource = image;

            Txt_FoodName.Text = selectedFood.Name;

            //Set listview of ingredients if the food is a pizza (Drink has no ingredients)
            if (selectedFood is Pizza)
            {
                List_Ingredients.ItemsSource = ((Pizza)selectedFood).Ingredients;
            }

            //Set Txt_PizzaPrice
            Txt_FoodPrice.Text = selectedFood.PriceAndCurrency;

            //if Quantity is > 0 upon construction, this is a Food item currently already in the basket. Set controls thereafter
            if (selectedFood.Quantity > 0)
            {
                Txt_CurrentQuantity.Text = selectedFood.Quantity.ToString();
                Bttn_FoodToOrFromBasket.Content = "Ändra antal";
            }
            else
                Txt_CurrentQuantity.Text = "1";
        }

        /// <summary>
        /// Event for when the button to decrease quantity is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_LessQty_Click(object sender, RoutedEventArgs e)
        {
            //Get current quantity from textblock
            int currentQuantity = int.Parse(Txt_CurrentQuantity.Text);
            
            //Only do stuff if the quantity is not now 0
            if(currentQuantity != 0) 
            {
                //if quantity=0, return same quantity, otherwise reduce by 1
                Txt_CurrentQuantity.Text = currentQuantity == 0 ? currentQuantity.ToString() : (currentQuantity-1).ToString();
                if (Txt_CurrentQuantity.Text == "0")
                {
                    //if quantity = 0 change button color and text
                    Bttn_FoodToOrFromBasket.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    if (selectedFood is Drink)
                        Bttn_FoodToOrFromBasket.Content = "Ta bort Dryck";
                    else
                        Bttn_FoodToOrFromBasket.Content = "Ta bort Pizza";
                }
            }
        }

        /// <summary>
        /// Event for when the button to increase quantity is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_MoreQty_Click(object sender, RoutedEventArgs e)
        {
            //Get current quantity from textblock
            int currentQuantity = int.Parse(Txt_CurrentQuantity.Text);
            //if quantity=99, return same quantity, otherwise increase by 1
            Txt_CurrentQuantity.Text = currentQuantity == 99 ? currentQuantity.ToString() : (currentQuantity + 1).ToString();
            //Change button color
            Bttn_FoodToOrFromBasket.Background = (SolidColorBrush)Resources["GreenButton"];

            //if pizza and the object initial quantity is 0 
            if (selectedFood is Pizza && selectedFood.Quantity==0)
                Bttn_FoodToOrFromBasket.Content = "Lägg till Pizza";
            //if drink and the object initial quantity is 0 
            else if (selectedFood is Drink && selectedFood.Quantity == 0)
                Bttn_FoodToOrFromBasket.Content = "Lägg till Dryck";
            //The object already exists in shopping cart
            else
                Bttn_FoodToOrFromBasket.Content = "Ändra antal";
        }

        /// <summary>
        /// Event for when the back button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_Back_Click(object sender, RoutedEventArgs e)
        {
            //reset food selection from the GridView in question
            foodMenu.SelectedItem = null;
            //Hide ContentDialog
            Hide();
        }

        /// <summary>
        /// Set quantity of the selected food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bttn_FoodToOrFromBasket_Click(object sender, RoutedEventArgs e)
        {
            //Set quantity of selected food
            selectedFood.Quantity = int.Parse(Txt_CurrentQuantity.Text);
            Hide();
        }
    }
}
