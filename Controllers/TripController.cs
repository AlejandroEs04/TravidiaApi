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

    [HttpGet]
    public IEnumerable<Trip> GetTrips()
    {
        try
        {
            var trips = _dapper.Query<Trip>("SELECT * FROM Trip");
            return trips;
        }
        catch (Exception)
        {
            throw;
        }
    }

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

            _dapper.Execute("Spu_TripUpsert", parameters);
        }
        catch (Exception)
        {
            throw;
        }
        return Ok();
    }

    [HttpGet("SendApproval/{id}")]
    public IActionResult SubmitTrip(int id)
    {
        try
        {
            int originatorId = Convert.ToInt32(User.FindFirst("userId")?.Value);
            CreateRequest(1, id, originatorId);

            _dapper.Execute("Spu_TripUpsert", new { id, @statusId = 2 });
        }
        catch (Exception)
        {
            throw;
        }
        return Ok();
    }

    [HttpPut]
    public IActionResult UpdateTrip(int id, Trip trip)
    {
        try
        {
            var tripDb = _dapper.QuerySingle<Trip>("SELECT * FROM Trip WHERE id = @id", new { id });

            if(tripDb!.StatusId != 1) return StatusCode(401, new { message = "Trip cannot update if it was sended to approvation" });

            var parameters = new
            {
                id,
                @departureDate = trip.DepartureDate,
                @returnDate = trip.ReturnDate,
                @destiny = trip.Destiny,
                @origin = trip.Origin,
                @purpose = trip.Purpose
            };

            _dapper.Execute("Spu_TripUpsert", parameters);
        }
        catch (Exception)
        {
            throw;
        }

        return Ok();
    }

    protected void CreateRequest(int flowId, int itemId, int userId)
    {
        try
        {
            if(_dapper == null) throw new Exception("Dapper is not inizializated");

            var steps = _dapper.Query<RequestStep>("SELECT * FROM RequestStep WHERE requestFlowId = @requestFlowId", new { @requestFlowId = flowId });

            var lastStep = steps.FirstOrDefault(s => s.IsFinish == true) ?? throw new Exception("No step found with IsFinish == true");
            RequestStep? firstStep = null;
            
            while (firstStep == null && lastStep != null)
            {
                var step = steps.FirstOrDefault(s => s.NextStepId == lastStep.Id);
                if (step == null) 
                {
                    firstStep = lastStep;
                    break;
                }
                lastStep = step;
            }
            if (firstStep == null) throw new Exception("No valid first step found");

            var user = _dapper.QuerySingle<User>("SELECT * FROM [User] WHERE id = @id", new { @id = userId });

            var parameters = new
            {
                itemId, 
                @requestStepId = lastStep!.Id, 
                @departmentId = GetNotNull(lastStep.DepartmentId, user!.DepartmentId),           
                @rolId = lastStep.RolId,    
                @userId = GetNotNull(lastStep.UserId, user.SupervisorId),    
            };

            _dapper.Execute("Spu_Request_Upsert", parameters);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    protected int GetNotNull(int? dbValue, int? value)
    {
        return (int)(dbValue ?? value)!;
    }
}