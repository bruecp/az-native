using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using Dapr.Client;
using IBankActorInterface;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        AILogger logger;
        IPaymentRepository payment;
        DaprClient daprClient;

        public PaymentController(IPaymentRepository repository,  DaprClient dapr, AILogger aILogger)
        {
            logger = aILogger;
            payment = repository;
            daprClient = dapr;
        }

        // http://localhost:PORT/payment/create
        [HttpPost()]
        [Route("create")]
        [Dapr.Topic("food-pubsub", "payment-requested")]
        public async Task AddPayment(OrderEvent evt)
        {
            Console.WriteLine($"Received payment request for order: {evt.OrderId}", evt);
            PaymentRequest paymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentRequest>(evt.Data.ToString());
            if (paymentRequest != null)
            {
                // When using transactional Outbox pattern, we need to add the payment to the database
                // We then could use Cosmos Change feed to execute the payment against our dapr bank service
                // PaymentTransaction payment = new PaymentTransaction()
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     CustomerId = evt.CustomerId,
                //     OrderId = paymentRequest.OrderId,
                //     PaymentInfo = paymentRequest.PaymentInfo,
                //     Amount = paymentRequest.Amount,
                //     Status = "Pending"
                // };            
                // await this.payment.AddPaymentAsync(payment);

                // To keep things simple we will just execute the payment against our dapr bank service
                // Make sure to created the bank account with the same account number in advance
                // You will find a sample in the bank client of the starter
                var usersBank = ActorProxy.Create<IBankActor>(new ActorId(paymentRequest.PaymentInfo.AccountNumber), "BankActor");
                var withdrawResp = await usersBank.Withdraw(new WithdrawRequest() { Amount = paymentRequest.Amount });
                // Now we could issue a payment response just like we did in 
                PaymentResponse paymentResponse = new PaymentResponse()
                {
                    OrderId = paymentRequest.OrderId,
                    Status = withdrawResp.Status,
                    Data = withdrawResp.Message
                };

                await daprClient.PublishEventAsync("food-pubsub", "payment-response", paymentResponse);
            }
        }

        // http://localhost:PORT/payment/getAll
        [HttpGet()]
        [Route("getAll")]
        public async Task<IEnumerable<PaymentTransaction>> GetAllPaymentsAsync()
        {
            return await payment.GetAllPaymentsAsync();
        }

        // http://localhost:PORT/orders/getById/{id}/{customerId
        [HttpGet()]
        [Route("getById/{id}/{customerId}")]
        public async Task<PaymentTransaction> GetPaymentByIdAsync(string id, string customerId)
        {
            return await payment.GetPaymentByIdAsync(id, customerId);
        }

        // http://localhost:PORT/orders/update
        [HttpPut()]
        [Route("update")]
        public async Task<IActionResult> UpdatePaymentAsync(PaymentTransaction payment)
        {
            await this.payment.UpdatePaymentAsync(payment.Id, payment);
            return Ok();
        }
    }
}