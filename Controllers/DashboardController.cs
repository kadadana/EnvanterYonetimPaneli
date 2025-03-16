using EnvanterApiProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.Data.SqlClient;


namespace EnvanterApiProjesi.Controllers;

public class DashboardController : Controller
{
    private readonly string? _connectionString;
    private readonly EnvanterRepo _envanterRepo;

    public DashboardController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
    }
    [HttpGet]
    public IActionResult DashboardMain(int page, string sortColumn = "Asset", string sortOrder = "des")
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            List<EnvanterModel>? comps = _envanterRepo.GetEnvanterList("EnvanterTablosu");



            var sortedComps = sortColumn switch
            {

                "Asset" => sortOrder == "asc" ? comps?.OrderBy(x => x.Asset) : comps?.OrderByDescending(x => x.Asset),
                "SeriNo" => sortOrder == "asc" ? comps?.OrderBy(x => x.SeriNo) : comps?.OrderByDescending(x => x.SeriNo),
                "CompModel" => sortOrder == "asc" ? comps?.OrderBy(x => x.CompModel) : comps?.OrderByDescending(x => x.CompModel),
                "CompName" => sortOrder == "asc" ? comps?.OrderBy(x => x.CompName) : comps?.OrderByDescending(x => x.CompName),
                "RAM" => sortOrder == "asc" ? comps?.OrderBy(x => float.TryParse(x.RAM?.Replace(" GB", ""), out float ram) ? ram : float.MaxValue)
                                            : comps?.OrderByDescending(x => float.TryParse(x.RAM?.Replace(" GB", ""), out float ram) ? ram : float.MinValue),
                "DiskGB" => sortOrder == "asc" ? comps?.OrderBy(x => float.TryParse(x.DiskGB?.Replace(" GB", ""), out float disk) ? disk : float.MaxValue)
                                            : comps?.OrderByDescending(x => float.TryParse(x.DiskGB?.Replace(" GB", ""), out float disk) ? disk : float.MinValue),
                "MAC" => sortOrder == "asc" ? comps?.OrderBy(x => x.MAC) : comps?.OrderByDescending(x => x.MAC),
                "ProcModel" => sortOrder == "asc" ? comps?.OrderBy(x => x.ProcModel) : comps?.OrderByDescending(x => x.ProcModel),
                "Username" => sortOrder == "asc" ? comps?.OrderBy(x => x.Username) : comps?.OrderByDescending(x => x.Username),
                "DateChanged" => sortOrder == "asc" ? comps?.OrderBy(x => DateTime.TryParse(x.DateChanged, out DateTime dt) ? dt : DateTime.MaxValue)
                                            : comps?.OrderByDescending(x => DateTime.TryParse(x.DateChanged, out DateTime dt) ? dt : DateTime.MinValue),
                _ => comps?.OrderBy(x => x.Asset)

            };
            int pageSize = 10;
            page = page >= 1 ? page : 1;
            IPagedList<EnvanterModel>? pagedList = sortedComps?.ToPagedList(page, pageSize);

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            return View(pagedList);

        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }
    public IActionResult DashboardAssetSNMatcher()
    {

        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {

            return View();
        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }


    [HttpPost]
    public IActionResult AssetSNMatcher(EnvanterModel envanterModel)
    {
        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return RedirectToAction("DashboardAssetSNMatcher", "Dashboard");
        }
        else
        {
            TempData["Info"] = _envanterRepo.AssetSNMatcher(envanterModel);
            return RedirectToAction("DashboardAssetSNMatcher", "Dashboard");

        }
    }

    [HttpGet("Details/{seriNo}/{page?}")]
    public IActionResult Details(string seriNo, int page)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            List<EnvanterModel>? comps = _envanterRepo.GetEnvanterList(seriNo);
            int pageSize = 10;
            page = page >= 1 ? page : 1;
            IPagedList<EnvanterModel>? pagedList = comps?.ToPagedList(page, pageSize);
            return View(pagedList);



        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }

}
