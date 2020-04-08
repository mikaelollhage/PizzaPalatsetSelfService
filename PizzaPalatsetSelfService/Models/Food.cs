using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.Models
{
    public class Food : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int quantity;

        public Food(string name, decimal price, string image)
        {
            Name = name;
            Price = price;
            Image = image;
        }

        private void NotifyPropertyChanged(string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
                NotifyPropertyChanged("Quantity");
                this.TotalItemCost = $"{this.Quantity * this.Price}";
            }
        }
        public string PriceAndCurrency
        {
            get { return $"Pris: {Price} SEK"; }
        }
        public string TotalItemCost
        {
            get
            {
                return $"{Quantity * Price} SEK";
            }
            set
            {
                NotifyPropertyChanged("TotalItemCost");
            }
        }
    }
}
