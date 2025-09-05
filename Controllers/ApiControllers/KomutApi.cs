using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnvanterYonetimPaneli.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KomutApi : ControllerBase
{
    private readonly string? _connectionString;

    private KomutRepo _komutRepo;

    public KomutApi(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _komutRepo = new KomutRepo(configuration);
    }


    [HttpPost]
    public IActionResult SendCommand([FromBody] KomutModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_komutRepo.AddToSql(model, false));
    }

    [HttpGet]
    public IActionResult GetCommand([FromQuery] string compName)
    {
        if (string.IsNullOrWhiteSpace(compName))
            return BadRequest("Gecersiz bilgisayar adi!");

        KomutModel komutModel = _komutRepo.GetUnappliedCommandsByCompName(compName);
        if (komutModel.Id == null)
            return Ok("Sirada bekleyen komut yok.");
        else
            return Ok(komutModel);
    }

    [HttpPut]
    public IActionResult UpdateCommand([FromBody] KomutModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_komutRepo.AddToSql(model, true));
    }
}
