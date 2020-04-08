using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaPalatsetSelfService.Models
{
    public class Pizza : Food
    {
        public Pizza(int pizzaId, string name, List<Ingredient> ingredients,  decimal price,string image) : base(name, price, image)
        {
            PizzaId = pizzaId;
            Ingredients = ingredients;
        }

        public int PizzaId { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }
}
