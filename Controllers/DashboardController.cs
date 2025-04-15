using EnvanterApiProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;


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
    public IActionResult DashboardMain(int page = 1, string sortColumn = "Id", string sortOrder = "asc", string? searchedColumn = null, string? searchedValue1 = null, string? searchedValue2 = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            List<EnvanterModel>? comps = _envanterRepo.GetOrderedList("EnvanterTablosu", sortColumn, sortOrder);
            if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
            {
                comps = _envanterRepo.GetSearchedTable("EnvanterTablosu", searchedColumn, searchedValue1, searchedValue2);
            }
            else if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1) && !string.IsNullOrEmpty(searchedValue2))
            {
                comps = _envanterRepo.GetSearchedTable("EnvanterTablosu", searchedColumn, searchedValue1, searchedValue2);
            }
            else if (ViewBag.SearchedTable != null)
            {
                comps = (List<EnvanterModel>)ViewBag.SearchedTable;
            }
            else
            {
                comps = _envanterRepo.GetOrderedList("EnvanterTablosu", sortColumn, sortOrder);
            }

            int pageSize = 10;
            page = page >= 1 ? page : 1;
            IPagedList<EnvanterModel>? pagedList = comps?.ToPagedList(page, pageSize);

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchedColumn = searchedColumn;
            ViewBag.SearchedValue1 = searchedValue1;
            ViewBag.SearchedValue2 = searchedValue2;

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

            TempData["Info"] = _envanterRepo.AddToSql(envanterModel);
            return RedirectToAction("DashboardAssetSNMatcher", "Dashboard");

        }
    }

    [HttpGet("Details/{id}/{page?}")]
    public IActionResult Details(string id, int page)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            List<EnvanterModel>? comps = _envanterRepo.GetSortedByDate(id);
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
    public IActionResult Edit(string? id = null)
    {

        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            if (!string.IsNullOrEmpty(id))
            {
                List<EnvanterModel>? compList = _envanterRepo.GetRowById(id, "EnvanterTablosu");
                EnvanterModel? comp = _envanterRepo.GetModelFromList(compList);
                return View(comp);

            }
            else
            {
                TempData["Alert"] = "Id belirtilmeli.";
                return RedirectToAction("DashboardMain", "Dashboard");
            }

        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }

    [HttpPost]
    public IActionResult Edit(EnvanterModel envanterModel)
    {
        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return Edit(envanterModel.Id);
        }
        else
        {
            envanterModel.DateChanged = DateTime.Now.ToString();
            TempData["Info"] = _envanterRepo.EditSql(envanterModel);
            return Edit(envanterModel.Id);

        }
    }

}
