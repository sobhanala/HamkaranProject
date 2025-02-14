using Microsoft.AspNetCore.Mvc;
using MessageBrokerAPI.Services;

namespace MessageBrokerAPI.Controllers;

[ApiController]
[Route("api/produce")]
public class ProducerController : ControllerBase
{
    private readonly BrokerService _broker;

    public ProducerController(BrokerService broker)
    {
        _broker = broker;
    }

    [HttpPost]
    public IActionResult Produce([FromBody] string message)
    {
        _broker.Produce(message);
        return Ok("Message produced");
    }
}
