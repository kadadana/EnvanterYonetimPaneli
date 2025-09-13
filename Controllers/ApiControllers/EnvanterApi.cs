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

    [HttpGet]
    public IActionResult GetEnvanterList(string sortColumn = "ID",
                                         string sortOrder = "asc",
                                         string? searchedColumn = null,
                                         string? searchedValue1 = null,
                                         string? searchedValue2 = null)
    {
        List<EnvanterModel>? comps;

        if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
        {
            comps = _envanterRepo.GetSearchedTable(searchedColumn, searchedValue1, searchedValue2);
        }
        else
        {
            comps = _envanterRepo.GetOrderedList(sortColumn, sortOrder);
        }

        comps ??= new List<EnvanterModel>();

        return Ok(comps);
    }

    [HttpPost]
    public IActionResult AddEnvanter([FromBody] EnvanterModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        return Ok(_envanterRepo.AddToSql(model));
    }

    [HttpGet("{id}")]
    public IActionResult GetEnvanterById(string id)
    {
        var comp = _envanterRepo.GetEnvanterModelById(id);
        if (comp == null)
            return NotFound("Kayit bulunamadi!");

        return Ok(comp);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEnvanter(string id, [FromBody] EnvanterModel model)
    {
        if (model == null)
            return BadRequest("Gecersiz veri!");

        var existingComp = _envanterRepo.GetEnvanterModelById(id);
        if (existingComp == null)
            return NotFound("Kayit bulunamadi!");

        return Ok(_envanterRepo.EditSql(model));
    }

}
