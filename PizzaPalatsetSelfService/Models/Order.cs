using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.Models
{
    public class Order : INotifyPropertyChanged
    {        
        public event PropertyChangedEventHandler PropertyChanged;
        private int orderNumber;

        public Order(int orderNumber, List<Pizza> pizzaOrderContents, List<Drink> drinkOrderContents, decimal totalOrderCost)
        {
            OrderNumber = orderNumber;
            PizzaOrderContents = pizzaOrderContents;
            DrinkOrderContents = drinkOrderContents;
            TotalOrderCost = totalOrderCost;
        }
        private void NotifyPropertyChanged(string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        [JsonProperty("OrderId")]
        public int OrderNumber
        {
            get
            {
                return orderNumber;
            }
            set
            {
                orderNumber = value;
                NotifyPropertyChanged("OrderNumber");
            }
        }
        public List<Pizza> PizzaOrderContents { get; set; }
        public List<Drink> DrinkOrderContents { get; set; }
        public decimal TotalOrderCost { get; set; }

    }
}
