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


    [HttpGet]
    public IActionResult GetAllCommands(string sortColumn = "ID",
                                        string sortOrder = "asc",
                                        string? searchedColumn = null,
                                        string? searchedValue1 = null,
                                        string? searchedValue2 = null)
    {
        List<KomutModel>? commands;

        if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
        {
            commands = _komutRepo.GetSearchedTable("KOMUT_TABLE", searchedColumn, searchedValue1, searchedValue2);
        }
        else
        {
            commands = _komutRepo.GetOrderedList("KOMUT_TABLE", sortColumn, sortOrder);
        }

        commands ??= new List<KomutModel>();

        return Ok(commands);
    }

    [HttpGet("{compName}")]
    public IActionResult GetCommand(string compName)
    {
        if (string.IsNullOrWhiteSpace(compName))
            return BadRequest("Gecersiz bilgisayar adi!");

        KomutModel komutModel = _komutRepo.GetUnappliedCommandsByCompName(compName);
        if (komutModel.Id == null)
            return Ok("Sirada bekleyen komut yok.");
        else
            return Ok(komutModel);
    }

    [HttpPost]
    public IActionResult SendCommand([FromBody] KomutModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_komutRepo.AddToSql(model, false));
    }



    [HttpPut]
    public IActionResult UpdateCommand([FromBody] KomutModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_komutRepo.AddToSql(model, true));
    }

}