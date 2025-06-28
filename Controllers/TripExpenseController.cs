using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TripExpenseController(DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;

    private User? GetAuthenticatedUser()
    {
        string? userId = User.FindFirst("userId")?.Value;
        return int.TryParse(userId, out int id) ? new User(_dapper).GetUserById(id) : null;
    }

    [HttpGet("{tripId}")]
    public IActionResult GetTripExpenses(int tripId)
    {
        var expenses = _dapper.Query<TripExpense>("SELECT * FROM TripExpense WHERE tripRequestId = @tripId", new { tripId });
        return Ok(expenses);
    }

    [HttpPost("{tripId}")]
    public IActionResult CreateTripExpense(int tripId, TripExpenseCreateDto expense)
    {
        try
        {
            var tripRequest = _dapper.QuerySingle<Trip>("SELECT * FROM Trip WHERE id = @tripId", new { tripId });

            if (tripRequest == null) return BadRequest("Trip Request not found");

            var request = _dapper.QuerySingle<Request>("SELECT * FROM Request WHERE id = @requestId", new { @requestId = tripRequest.RequestId });

            if (request == null) return BadRequest("Request not found");

            var user = GetAuthenticatedUser();
            if (user == null) return NotFound("User not found");
            if (!Models.User.IsUserOwner(user.Id, request.OriginatorId)) return Unauthorized("User has not permissions for add trip expense in this request");

            string queryExistsExpense = "SELECT * FROM TripExpense WHERE tripRequestId = @tripId AND reportedDate = @reportedDate AND expenseId = @expenseId";
            var existsExpenseParameters = new
            {
                tripId,
                @reportedDate = expense.ReportedDate.Date, 
                @expenseId = expense.ExpenseId
            };
            var existsExpense = _dapper.QuerySingle<TripExpense>(queryExistsExpense, existsExpenseParameters);

            if (existsExpense != null) throw new Exception("Expense's already exists");

            if (tripRequest.DepartureDate.Date > expense.ReportedDate.Date || tripRequest.ReturnedDate.Date < expense.ReportedDate.Date)
            {
                throw new Exception("Expense must be in trip duration range");
            }

            var parameters = new
            {
                @tripRequestId = tripRequest.Id,
                @requestId = request.Id,
                @expenseId = expense.ExpenseId,
                @amount = expense.Amount, 
                @reportedDate = expense.ReportedDate
            };

            var tripExpense = _dapper.QuerySingle<TripExpenseResponseDto>("TripExpense_upsert", parameters);
            return Ok(tripExpense);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}