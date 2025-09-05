using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnvanterYonetimPaneli.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnvanterApi : ControllerBase
{
    private readonly string? _connectionString;

    private EnvanterRepo _envanterRepo;

    public EnvanterApi(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
    }
    

    [HttpPost]
    public IActionResult AddEnvanter([FromBody] EnvanterModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        System.Console.WriteLine("Bilgisayardan veri geldi.");
        model.Log = "Bilgisayardan gelen veri.";
        return Ok(_envanterRepo.AddToSql(model));
    }
}
