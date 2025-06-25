using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TripController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);

    [HttpPost]
    public Trip CreateTrip(TripCreateDto newTrip)
    {
        try
        {
            string userId = User.FindFirst("userId")?.Value + "";

            var requestParameters = new
            {
                @documentId = 1, 
                @originatorId = userId
            };
            var request = _dapper.QuerySingle<Request>("Request_upsert", requestParameters) ?? throw new Exception("An error was success");

            var tripParameters = new
            {
                @requestId = request.Id,
                @departureDate = newTrip.DepartureDate,
                @returnedDate = newTrip.ReturnedDate,
                @origin = newTrip.Origin,
                @destiny = newTrip.Destiny,
                @purpose = newTrip.Purpose,
            };

            var trip = _dapper.QuerySingle<Trip>("Trip_upsert", tripParameters) ?? throw new Exception("An error was success");
            return trip;
        }
        catch (Exception)
        {
            throw;
        }
        
    }
}