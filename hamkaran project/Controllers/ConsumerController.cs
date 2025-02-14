using Microsoft.AspNetCore.Mvc;
using MessageBrokerAPI.Services;

namespace MessageBrokerAPI.Controllers;

[ApiController]
[Route("api/consume")]
public class ConsumerController : ControllerBase
{
    private readonly BrokerService _broker;

    public ConsumerController(BrokerService broker)
    {
        _broker = broker;
    }

    [HttpGet]
    public IActionResult Consume()
    {
        var message = _broker.Consume();
        if (message == null)
        {
            return BadRequest();
        }
        
        return Ok(message);
    }
}
