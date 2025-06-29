using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Dtos;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DocumentController(DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;

    private User? GetAuthenticatedUser()
    {
        string? userId = User.FindFirst("userId")?.Value;
        return int.TryParse(userId, out int id) ? new User(_dapper).GetUserById(id) : null;
    }

    private static bool IsUserAdmin(User user) => user.RolId == 1;

    [HttpGet]
    public IActionResult GetDocuments()
    {
        var documents = _dapper.Query<Document>("SELECT * FROM Document");
        return Ok(documents);
    }

    [HttpPost]
    public IActionResult CreateDocument(DocumentCreateDto document)
    {
        try
        {
            var user = GetAuthenticatedUser();
            if (user is null) return NotFound("User not found");
            if (!IsUserAdmin(user)) return Unauthorized("User is not authorized to create documents");

            var newDocument = _dapper.QuerySingle<Document>("Document_upsert", new { @name = document.Name }) ?? throw new Exception("An error have been success");

            foreach (var step in document.Steps)
            {
                var parameters = new
                {
                    @titleId = step.TitleId,
                    @departmentId = step.DepartmentId,
                    @areaId = step.AreaId,
                    @order = step.Order, 
                    @documentId = newDocument.Id
                };
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}