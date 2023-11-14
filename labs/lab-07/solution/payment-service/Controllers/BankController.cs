using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using IBankActorInterface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DaprBankActor;

[Route("[controller]")]
public class BankActorController : ControllerBase
{
    private readonly ILogger<BankActorController> _logger;

    public BankActorController(ILogger<BankActorController> logger)
    {
        _logger = logger;
    }

    [HttpPost("deposit")]
    public async Task<ActionResult> Deposit([FromBody] DepositRequest request)
    {
        var actor = ActorProxy.Create<IBankActor>(new ActorId(request.AccountId), "BankActor");
        await actor.SetupNewAccount(request.Amount);        
        return Ok();
    }
}