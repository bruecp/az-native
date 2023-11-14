using System.Text.Json;
using Newtonsoft.Json;

namespace FoodApp
{
    public class PaymentResponse
    {
        [JsonProperty("orderId")]
        public string OrderId {get;set;}
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}