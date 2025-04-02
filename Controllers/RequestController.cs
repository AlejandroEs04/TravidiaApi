using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RequestController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);

    [HttpPost]
    public IActionResult RequestResponse(RequestResponse response)
    {
        try
        {
            int originatorId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            int statusId = response.Response ? 4 : 3;

            var request = _dapper.QuerySingle<Request>("SELECT * FROM Request WHERE id = @id", new { @id = response.Id });

            var step = _dapper.QuerySingle<RequestStep>("SELECT * FROM RequestStep WHERE id = @id", new { @id = request!.RequestStepId });
            
            if(step!.IsFinish)
            {
                // Finish flow
                var parametersTrip = new 
                {
                    @id = request!.ItemId,
                    statusId
                };

                _dapper.Execute("Spu_TripUpsert", parametersTrip);
            }
            else
            {
                // Next Step
                NextRequest(step.NextStepId, Convert.ToInt32(request.ItemId), originatorId);
            }

            var parametersRequest = new 
            {
                @id = response.Id, 
                @response = response.Response
            };
            _dapper.Execute("Spu_ResponsedTrip", parametersRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return Ok();
    }

    protected void NextRequest(int nextStepId, int itemId, int userId)
    {
        try
        {
            var step = _dapper.QuerySingle<RequestStep>("SELECT * FROM RequestStep WHERE id = @id", new { @id = nextStepId });
            var user = _dapper.QuerySingle<User>("SELECT * FROM [User] WHERE id = @id", new { @id = userId });

            var parameters = new
            {
                itemId, 
                @requestStepId = step!.Id, 
                @departmentId = GetNotNull(step.DepartmentId, user!.DepartmentId),           
                @rolId = step.RolId,
                @userId = GetNotNull(step.UserId, user.SupervisorId),    
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