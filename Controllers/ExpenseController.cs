using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ExpenseController(DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;
    private User? GetAuthenticatedUser()
    {
        string? userId = User.FindFirst("userId")?.Value;
        return int.TryParse(userId, out int id) ? new User(_dapper).GetUserById(id) : null;
    }

    private static bool IsUserAdmin(User user) => user.RolId == 1;

    [HttpGet]
    public IActionResult GetExpenses()
    {
        var expenses = _dapper.Query<Expense>("SELECT * FROM Expense WHERE active = 1");
        return Ok(expenses);
    }

    [HttpPost]
    public IActionResult CreateExpense(ExpenseCreateDto expense)
    {
        try
        {
            var user = GetAuthenticatedUser();
            if (user is null) return NotFound("User not found");
            if (!IsUserAdmin(user)) return Unauthorized("User is not authorized to create expenses");

            var parameters = new
            {
                @name = expense.Name,
                @limit = expense.Limit
            };

            _dapper.Execute("Expense_upsert", parameters);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateExpense(int id, ExpenseCreateDto expense)
    {
        try
        {
            var user = GetAuthenticatedUser();
            if (user is null) return NotFound("User not found");
            if (!IsUserAdmin(user)) return Unauthorized("User is not authorized to update expenses");

            var parameters = new
            {
                id,
                @name = expense.Name,
                @limit = expense.Limit
            };

            _dapper.Execute("Expense_upsert", parameters);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteExpense(int id)
    {
        try
        {
            var user = GetAuthenticatedUser();
            if (user is null) return NotFound("User not found");
            if (!IsUserAdmin(user)) return Unauthorized("User is not authorized to delete expenses");

            var parameters = new
            {
                id,
                @active = 0
            };

            _dapper.Execute("Expense_upsert", parameters);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}