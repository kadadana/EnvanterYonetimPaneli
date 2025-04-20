using EnvanterApiProjesi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnvanterApiProjesi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnvanterApi : ControllerBase
{
    private readonly string? _connectionString;

    private EnvanterRepo _envanterRepo;

    public EnvanterApi(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
    }
    

    [HttpPost("AddEnvanter")]
    public IActionResult AddEnvanter([FromBody] EnvanterModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_envanterRepo.AddToSql(model));
    }
}
