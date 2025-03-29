using Microsoft.AspNetCore.Mvc;

namespace Travidia.Controllers;

[ApiController]
[Route("[controller]")]
public class TripController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);

    [HttpPost]
    public IActionResult CreateTrip()
    {
        try
        {
            
        }
        catch (Exception)
        {
            throw;
        }

        return Ok();
    }
}