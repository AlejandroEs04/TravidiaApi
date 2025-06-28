using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TripController(DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;

    private User? GetAuthenticatedUser()
    {
        string? userId = User.FindFirst("userId")?.Value;
        return int.TryParse(userId, out int id) ? new User(_dapper).GetUserById(id) : null;
    }

    [HttpGet]
    public IActionResult GetTrips()
    {
        string? userId = User.FindFirst("userId")?.Value;

        var tripRequests = _dapper.Query<TripResponseDto>("SELECT * FROM Vw_TripRequest WHERE originatorId = @userId", new { userId });
        return Ok(tripRequests);
    }

    [HttpPost]
    public IActionResult CreateTrip(TripCreateDto newTrip)
    {
        try
        {
            if (newTrip.ReturnedDate <= newTrip.DepartureDate) throw new Exception("Returned date must be greater than departure date");

            string userId = User.FindFirst("userId")?.Value + "";
            var tripRequests = _dapper.Query<TripResponseDto>("SELECT * FROM Vw_TripRequest WHERE originatorId = @userId", new { userId });

            var matchTripRequest = tripRequests
                .Where(t => newTrip.ReturnedDate.Date >= t.DepartureDate.Date && newTrip.DepartureDate.Date <= t.ReturnedDate.Date);

            if (matchTripRequest.Any()) throw new Exception("Cannot there an trip between an another trip");

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
            return Ok(trip);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTrip(int id, TripCreateDto updatingTrip)
    {
        try
        {
            if (updatingTrip.ReturnedDate <= updatingTrip.DepartureDate) throw new Exception("Returned date must be greater than departure date");

            string userId = User.FindFirst("userId")?.Value + "";
            var tripRequests = _dapper.Query<TripResponseDto>("SELECT * FROM Vw_TripRequest WHERE originatorId = @userId AND requestId != @id", new { userId, id });

            var matchTripRequest = tripRequests
                .Where(t => updatingTrip.ReturnedDate.Date >= t.DepartureDate.Date && updatingTrip.DepartureDate.Date <= t.ReturnedDate.Date);

            if (matchTripRequest.Any()) throw new Exception("Cannot there an trip between an another trip");

            var request = _dapper.QuerySingle<Request>("SELECT * FROM Request WHERE id = @id", new { id });

            if (request == null) return NotFound("Request not found");
            if (request.StatusId == 7) return BadRequest("Request is canceled, you cannot update it");

            var user = GetAuthenticatedUser();
            if (user == null) return NotFound("User not found");
            if (!Models.User.IsUserOwner(user.Id, request.OriginatorId)) return Unauthorized("User has not permissions for update this request");

            var tripRequets = _dapper.QuerySingle<Trip>("SELECT * FROM Trip WHERE requestId = @requestId", new { @requestId = request.Id });

            if (tripRequets == null) return NotFound("Trip Request not found");

            var tripParameters = new
            {
                @id = tripRequets.Id,
                @departureDate = updatingTrip.DepartureDate,
                @returnedDate = updatingTrip.ReturnedDate,
                @origin = updatingTrip.Origin,
                @destiny = updatingTrip.Destiny,
                @purpose = updatingTrip.Purpose,
            };

            var trip = _dapper.QuerySingle<Trip>("Trip_upsert", tripParameters) ?? throw new Exception("An error was success");
            return Ok(trip);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTrip(int id)
    {
        try
        {
            var request = _dapper.QuerySingle<Request>("SELECT * FROM Request WHERE id = @id", new { id });

            if (request == null) return NotFound("Request not found");

            var user = GetAuthenticatedUser();
            if (user == null) return NotFound("User not found");
            if (!Models.User.IsUserOwner(user.Id, request.OriginatorId)) return Unauthorized("User has not permissions for update this request");

            _dapper.Execute("Request_upsert", new { id, @statusId = 7 });

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}