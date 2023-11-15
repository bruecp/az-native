using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FoodApp
{
    public static class DurableFunctionsCustomerAccount
    {
        [FunctionName(nameof(DurableFunctionsCustomerAccount.CustomerAccount))]
        public static void CustomerAccount([EntityTrigger] IDurableEntityContext context)
        {
            switch (context.OperationName.ToLowerInvariant())
            {
                case "deposit":
                    context.SetState(context.GetState<int>() + context.GetInput<int>());
                    break;
                case "withdraw":
                    var balance = context.GetState<int>() - context.GetInput<int>();
                    context.SetState(balance);
                    break;
            }
        }

        [FunctionName("UpdateBalance")]
        public static async Task<HttpResponseMessage> UpdateBalance(
        [HttpTrigger(AuthorizationLevel.Function, Route = "customerAccount/updateBalance/{entityKey}/{amount}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey, string amount)
        {
            var entityId = new EntityId(nameof(CustomerAccount), entityKey);

            if (req.Method == HttpMethod.Post)
            {
                await client.SignalEntityAsync(entityId, "deposit", amount);
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            else if (req.Method == HttpMethod.Delete)
            {
                await client.SignalEntityAsync(entityId, "withdraw", amount);
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [FunctionName("GetBalance")]
        public static async Task<int> GetBalance(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customerAccount/getBalance/{entityKey}")] HttpRequestMessage req,
                string entityKey,
                [DurableClient] IDurableEntityClient client,
                ILogger log)
        {
            var entityId = new EntityId(nameof(CustomerAccount), entityKey);
            EntityStateResponse<int> stateResponse = await client.ReadEntityStateAsync<int>(entityId);
            return stateResponse.EntityState;
        }

    }
}