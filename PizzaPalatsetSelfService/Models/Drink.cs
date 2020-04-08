using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.Models
{
    public class Drink:Food
    {
        public Drink(int drinkId, string name, decimal price, string image) : base(name, price, image)
        {
            DrinkId = drinkId;
        }
        public int DrinkId { get; set; }
    }
}
