using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TripController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);

    [HttpPost]
    public IActionResult CreateTrip(CreateTripDto trip)
    {
        try
        {
            int originatorId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            var parameters = new
            {
                @departureDate = trip.DepartureDate,
                @returnDate = trip.ReturnDate,
                @destiny = trip.Destiny,
                @origin = trip.Origin,
                @purpose = trip.Purpose,
                originatorId,
            };

            _dapper.Execute("Spu_TripUpset", parameters);
        }
        catch (Exception)
        {
            throw;
        }

        return Ok();
    }
}