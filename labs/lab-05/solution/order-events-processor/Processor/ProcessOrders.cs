using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FoodApp.Orders
{
    public static class ProcessOrders
    {
        [FunctionName("ProcessOrders")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "food-nosql-peter",
            collectionName: "order-events",
            ConnectionStringSetting = "CosmosDBConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]
            IReadOnlyList<Document> input,
            ILogger logger)
        {
            OrdersRepository repo = new OrdersRepository(logger);

            foreach (var document in input)
            {
                var evt = JsonConvert.DeserializeObject<OrderEvent>(document.ToString());
                if (evt != null)
                {
                    logger.LogInformation($"Received Order Event {evt.Id} of type {evt.EventType}", evt);
                    if (evt.EventType == OrderEventType.Created.ToString())
                    {
                        //Create the order
                        Order order = JsonConvert.DeserializeObject<Order>(evt.Data.ToString());
                        await repo.CreateOrderAsync(order);
                    }
                    else
                    {
                        //Get the order and add the current event to history
                        Order order = await repo.GetOrderAsync(evt.OrderId, evt.CustomerId);
                        if (order != null)
                        {
                            await repo.UpdateOrderAsync(order, evt);
                        }
                    }
                }
            }
        }
    }
}
