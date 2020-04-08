using PizzaPalatsetSelfService.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.ViewModels
{
    public class OrderViewModel
    {
        private static OrderViewModel instance = null;
        private static readonly object padlock = new object();  
        private ObservableCollection<Pizza> allOrderedPizzas;
        private ObservableCollection<Drink> allOrderedDrinks;
        //field created to hold all items the user wants to order.
        //is used to reset the original food objects quantities after the order has been placed. 
        private ObservableCollection<Food> allOrderedItems;

        public OrderViewModel()
        {
            allOrderedPizzas = new ObservableCollection<Pizza>();
            allOrderedDrinks = new ObservableCollection<Drink>();
        }

        /// <summary>
        /// Splits the shopping cart from one list of Food into Pizza and Drink
        /// </summary>
        /// <param name="allOrderedItems"></param>
        /// <returns></returns>
        public (ObservableCollection<Pizza>, ObservableCollection<Drink>) SplitShoppingCart(ObservableCollection<Food> allOrderedItems)
        {
            Pizza tempPizza;
            Drink tempDrink;            
            Pizza newPizza;
            Drink newDrink;

            //Set local field to all selected Food items so they later can be reset
            this.allOrderedItems = allOrderedItems;
            
            //Reset TotalOrderCost from last order
            TotalOrderCost = 0;

            //Loop through all ordered food items
            for (int i = 0; i < allOrderedItems.Count; i++)
            {
                //Creation of two lists, populated with brand new food objects so they can stora their own Quantity-information

                //if pizza, add new pizza to pizzas-list
                if (allOrderedItems[i] is Pizza)
                {
                    tempPizza = allOrderedItems[i] as Pizza;
                    allOrderedPizzas.Add(newPizza = new Pizza((tempPizza).PizzaId, (tempPizza).Name, (tempPizza).Ingredients, (tempPizza).Price, (tempPizza).Image));
                    newPizza.Quantity = tempPizza.Quantity;
                    AddToOrderCost(tempPizza);
                }
                //if drink, add new drink to drinks-list
                else
                {
                    tempDrink = allOrderedItems[i] as Drink;
                    allOrderedDrinks.Add((newDrink = new Drink((tempDrink).DrinkId, (tempDrink).Name, (tempDrink).Price, (tempDrink).Image)));
                    newDrink.Quantity = tempDrink.Quantity;
                    AddToOrderCost(tempDrink);
                }
            }
            //return tuple of both lists
            return (allOrderedPizzas, allOrderedDrinks);
        }

        /// <summary>
        /// Update value of TotalOrderCost 
        /// </summary>
        /// <param name="food"></param>
        private void AddToOrderCost(Food food)
        {
            TotalOrderCost += food.Quantity * food.Price;
        }

        /// <summary>
        /// Creates the confirmed order, saves it locally and in the database
        /// </summary>
        /// <returns></returns>
        public async Task CreateOrder()
        {
            Order placedOrder;
            //Save order locally
            AllActiveOrders.Add(placedOrder = new Order(OrderNumber, allOrderedPizzas.ToList<Pizza>(), allOrderedDrinks.ToList<Drink>(), TotalOrderCost));
            
            //Write order to database and save the received ordernumber to the object in the order-list
            placedOrder.OrderNumber = await APICall.Instance.PostOrderAsync(placedOrder);

            //Used to print the correct orderNumber in the ContentDialog after the order has been created
            OrderNumber = placedOrder.OrderNumber;

            //Set bool to true to be able to navigate all the way back to the welcome page
            OrderCreatedOrModified = true;

            //Clear all lists used for populating controls in various lists
            ClearOrderLists();            
        }

        /// <summary>
        /// Method to give an existing order new content
        /// </summary>
        /// <param name="orderToChange"></param>
        /// <returns></returns>
        public async Task ModifySelectedOrder(Order orderToChange)
        {
            //Give selected order it's new Pizza contents
            orderToChange.PizzaOrderContents = allOrderedPizzas.ToList();
            //Give selected order it's new Drink contents
            orderToChange.DrinkOrderContents = allOrderedDrinks.ToList();
            //Give selected order it's new Total cost
            orderToChange.TotalOrderCost = TotalOrderCost;
            //Write update to database
            await APICall.Instance.UpdateOrderAsync(orderToChange);

            //Set bool to true to be able to navigate all the way back to the welcome page
            OrderCreatedOrModified = true;

            //Clear all lists used for populating controls in various lists
            ClearOrderLists();
        }

        /// <summary>
        /// Method to remove selected order locally and from the database
        /// </summary>
        /// <param name="orderToDelete"></param>
        public async void DeleteCurrentOrder(Order orderToDelete)
        {
            //remove locally
            AllActiveOrders.Remove(orderToDelete);
            //remove from database
            await APICall.Instance.DeleteOrderAsync(orderToDelete);
        }

        /// <summary>
        /// Method to clear all lists pertaining to the specific order
        /// </summary>
        public void ClearOrderLists()
        {
            //This must always happen regardless if an order is created or if user "goes back". Used in Cashier
            allOrderedPizzas.Clear();
            allOrderedDrinks.Clear();

            //This must only happen if an order has been placed. Otherwise the shopping cart would be cleared.
            if (OrderCreatedOrModified)
            {
                for (int i = 0; i < allOrderedItems.Count; i++)
                {
                    allOrderedItems[i].Quantity = 0;
                }
                allOrderedItems.Clear();
            }            
        }


        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static OrderViewModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if(instance==null)
                    {
                        instance = new OrderViewModel();
                    }
                }
                return instance;
            }
        }

        public int OrderNumber { get; set; }        
        public ObservableCollection<Order> AllActiveOrders { get; set; } = new ObservableCollection<Order>();
        public decimal TotalOrderCost { get; private set; }
        public bool OrderCreatedOrModified { get; set; }
    }
}
