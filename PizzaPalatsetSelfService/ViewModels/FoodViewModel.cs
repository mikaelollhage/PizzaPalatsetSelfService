using PizzaPalatsetSelfService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.ViewModels
{
    public class FoodViewModel
    {
        private static FoodViewModel instance = null;
        private static readonly object padlock = new object();

        public FoodViewModel()
        {
            AllPizzas = new ObservableCollection<Pizza>();
            ShoppingCart = new ObservableCollection<Food>();
            AllDrinks = new ObservableCollection<Drink>();
        }

        /// <summary>
        /// Adds or removes an item from the shopping cart
        /// </summary>
        /// <param name="selectedFood"></param>
        public void AddRemoveFromShoppingCart(Food selectedFood)
        {
            //If a pizza with the same name already exist in the collection then don't add the selectedFood
            if (ShoppingCart.Select(p => p).Where(p => p.Name == selectedFood.Name).FirstOrDefault() != null && selectedFood.Quantity != 0)
            {
                //Don't do anything
            }
            //If quantity is larger than 0
            else if (selectedFood.Quantity > 0)
            {
                ShoppingCart.Add(selectedFood);
            }
            //if shoppingcart is not empty and the selected items quantity is set to 0
            else if (ShoppingCart != null && selectedFood.Quantity == 0)
            {
                ShoppingCart.Remove(selectedFood);
            }

            //update value of shopping cart to be displayed
            UpdateShoppingCartValue();
        }

        /// <summary>
        /// Method to remove food from shopping cart, called when the shopping cart image (red X) is clicked
        /// </summary>
        /// <param name="selectedFood"></param>
        public void RemoveFoodFromShoppingCart(Food selectedFood)
        {
            //Remove item
            ShoppingCart.Remove(selectedFood);
            //update value of shopping cart to be displayed
            UpdateShoppingCartValue();
            //reset the quantity of the selected item
            ResetQuantityOfSelectedFood(selectedFood);
        }

        /// <summary>
        /// Method to update the value of the shopping cart upon changes
        /// </summary>
        private void UpdateShoppingCartValue()
        {
            //Loop through shopping cart items and summarize all Quantity * Price
            CurrentBasketValue = ShoppingCart.Sum(p => p.Quantity * p.Price);
        }

        /// <summary>
        /// Method to contact database and fill the lists that populates the two menus
        /// </summary>
        /// <returns></returns>
        public async Task PopulateFoodLists()
        {
            //Populate pizza menu from database
            AllPizzas = await APICall.Instance.GetPizzasAsync();
            //Populate drink menu from database
            AllDrinks = await APICall.Instance.GetDrinksAsync();
        }

        /// <summary>
        /// Fills shopping cart from the contents of the order that is about to be changed
        /// </summary>
        /// <param name="openOrderContents"></param>
        /// <returns></returns>
        public ObservableCollection<Food> FillShoppingCartFromOpenOrder(List<Food> openOrderContents)
        {   
            //Loop through openOrderContents (gotten from merging the two content lists earlier)
            for (int i = 0; i < openOrderContents.Count; i++)
            {
                //Loop through all original Pizza objects
                for (int j = 0; j < AllPizzas.Count; j++)
                {
                    //If an original pizza object has the same name as the currect pizza from the open order...
                    if (AllPizzas[j].Name == openOrderContents[i].Name)
                    {
                        //..add that original pizza to the Shopping cart
                        ShoppingCart.Add(AllPizzas[j]);
                        //give the original pizza the same quantity as the pizza in the order.                        
                        AllPizzas[j].Quantity = openOrderContents[i].Quantity;
                        
                        //This way the same methods used for creating an order can be used for updating an order.
                    }
                }

                //Do same thing for Drinks
                for (int k = 0; k < AllDrinks.Count; k++)
                {
                    if (AllDrinks[k].Name == openOrderContents[i].Name)
                    {
                        ShoppingCart.Add(AllDrinks[k]);
                        AllDrinks[k].Quantity = openOrderContents[i].Quantity;
                    }
                }
            }                      
            return ShoppingCart;
        }

        /// <summary>
        /// Clears the shopping cart
        /// </summary>
        public void ClearShoppingCart()
        {
            ShoppingCart.Clear();
        }

        /// <summary>
        /// Resets Food quantities in the original food objects
        /// </summary>
        public void ResetFoodQuantities()
        {
            for (int i = 0; i < AllPizzas.Count; i++)
            {
                AllPizzas[i].Quantity = 0;
            }
            for (int i = 0; i < AllDrinks.Count; i++)
            {
                AllDrinks[i].Quantity = 0;
            }
        }

        /// <summary>
        /// Resets the food quantity of the selected food
        /// </summary>
        /// <param name="selectedFood"></param>
        public void ResetQuantityOfSelectedFood(Food selectedFood)
        {
            selectedFood.Quantity = 0;
        }

        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static FoodViewModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new FoodViewModel();
                    }
                }
                return instance;
            }
        }

        public ObservableCollection<Pizza> AllPizzas { get; private set; }
        public ObservableCollection<Drink> AllDrinks { get; private set; }
        public ObservableCollection<Food> ShoppingCart { get; private set; }
        public decimal CurrentBasketValue { get; private set; }

    }
}
