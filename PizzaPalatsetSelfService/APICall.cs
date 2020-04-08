using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.ObjectModel;
using PizzaPalatsetSelfService.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace PizzaPalatsetSelfService
{
    public class APICall
    {
        private static APICall instance = null;
        private static readonly object padlock = new object();
        private HttpClient httpClient;
        private string url;
        string jsonString;

        public APICall()
        {
            httpClient = new HttpClient();
        }

        public async Task<ObservableCollection<Pizza>> GetPizzasAsync()
        {
            url = "https://localhost:44357/api/Pizzas";
            jsonString = await httpClient.GetStringAsync(url);
            var Pizzas = JsonConvert.DeserializeObject<ObservableCollection<Pizza>>(jsonString);
            return Pizzas;
        }
        public async Task<ObservableCollection<Drink>> GetDrinksAsync()
        {
            url = "https://localhost:44357/api/Drinks";
            jsonString = await httpClient.GetStringAsync(url);
            var Drink = JsonConvert.DeserializeObject<ObservableCollection<Drink>>(jsonString);
            return Drink;
        }
        public async Task<int> PostOrderAsync(Order placedOrder)
        {
            url = "https://localhost:44357/api/Orders/";
            jsonString = JsonConvert.SerializeObject(placedOrder);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage responseOrder = await httpClient.PostAsync(url, httpContent);
            string orderString = await responseOrder.Content.ReadAsStringAsync();
            int orderId = JsonConvert.DeserializeObject<Order>(orderString).OrderNumber;

            return orderId;
        }
        public async Task UpdateOrderAsync(Order orderToChange)
        {
            url = $"https://localhost:44357/api/Orders/{orderToChange.OrderNumber}";
            jsonString = JsonConvert.SerializeObject(orderToChange);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await httpClient.PutAsync(url, httpContent);
        }
        public async Task DeleteOrderAsync(Order orderToDelete)
        {
            url = $"https://localhost:44357/api/Orders/{orderToDelete.OrderNumber}";
            await httpClient.DeleteAsync(url);
        }

        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static APICall Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new APICall();
                    }
                }
                return instance;
            }
        }
    }
}
