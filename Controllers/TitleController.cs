using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travidia.Models;

namespace Travidia.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TitleController(DataContextDapper dapper) : ControllerBase
{
    private readonly DataContextDapper _dapper = dapper;

    [HttpGet]
    public IActionResult GetTitles()
    {
        var titles = _dapper.Query<Title>("SELECT * FROM Title");
        return Ok(titles);
    }
}