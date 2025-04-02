using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TripExpenseController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);

    [HttpPost]
    public IActionResult RegisterExpense(CreateTripExpenseDto expense)
    {
        try
        {
            var trip = _dapper.QuerySingle<Trip>("SELECT * FROM Trip WHERE id = @id", new { @id = expense.TripId });

            if(trip!.StatusId != 3) return StatusCode(401, new { message = "The trip is not approved" });

            var parameters = new 
            {
                @tripId = expense.TripId,
                @expenseTypeId = expense.ExpenseTypeId, 
                @amount = expense.Amount
            };
            _dapper.Execute("Spu_TripExpenseUpsert", parameters);
        }
        catch (Exception)
        {
            throw;
        }
        return Ok();
    }
}