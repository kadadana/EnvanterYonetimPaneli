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

    public IActionResult DashboardMain(int page)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            List<EnvanterModel> comps = _envanterRepo.GetEnvanterList("EnvanterTablosu");
            int pageSize = 10;
            page = page >= 1 ? page : 1;
            IPagedList<EnvanterModel> pagedList = comps.ToPagedList(page, pageSize);
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
}
