using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using System.Text;


namespace EnvanterYonetimPaneli.Controllers;

public class DashboardController : Controller
{
    private readonly string? _connectionString;
    private readonly EnvanterRepo _envanterRepo;
    private readonly KomutRepo _komutRepo;
    public DashboardController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
        _komutRepo = new KomutRepo(configuration);
    }
    [HttpGet]
    public IActionResult DashboardMain(int page = 1, string sortColumn = "ID", string sortOrder = "asc", string? searchedColumn = null, string? searchedValue1 = null, string? searchedValue2 = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }

        List<EnvanterModel>? comps;

        if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
        {
            comps = _envanterRepo.GetSearchedTable("ENVANTER_TABLE", searchedColumn, searchedValue1, searchedValue2);
        }
        else
        {
            comps = _envanterRepo.GetOrderedList("ENVANTER_TABLE", sortColumn, sortOrder);
        }

        comps ??= new List<EnvanterModel>();

        int pageSize = 10;
        page = Math.Max(page, 1);

        IPagedList<EnvanterModel> pagedList = comps.ToPagedList(page, pageSize);

        ViewBag.SortColumn = sortColumn;
        ViewBag.SortOrder = sortOrder;
        ViewBag.SearchedColumn = searchedColumn;
        ViewBag.SearchedValue1 = searchedValue1;
        ViewBag.SearchedValue2 = searchedValue2;

        return View(pagedList);
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
            envanterModel.Log = "Asset atama islemi.";
            envanterModel.DateChanged = DateTime.Now.ToString();
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

            var selectedComp = _envanterRepo.GetModelFromList(comps);
            var selectedDisks = selectedComp != null ? _envanterRepo.GetDiskListById("1") : null;

            var viewModel = new EnvanterViewModel
            {
                SelectedComputer = selectedComp,
                SelectedDisks = selectedDisks,
                EnvanterList = pagedList
            };

            return View(viewModel);
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
                List<EnvanterModel>? compList = _envanterRepo.GetRowById(id, "ENVANTER_TABLE");
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
            envanterModel.Log = "Duzenleme islemi.";
            TempData["Info"] = _envanterRepo.EditSql(envanterModel);
            return Edit(envanterModel.Id);

        }
    }

    [HttpGet("Dashboard/Commands/{compName}")]
    public IActionResult Commands(string compName)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            var model = new KomutModel();
            model.CompName = compName;

            return View(model);

        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }
    public IActionResult Commands()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            var model = new KomutModel();

            return View(model);

        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Command(string command, string compName)
    {
        if (UserModel.DefaultUser.IsLoggedIn)
        {
            if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(compName))
            {
                var model = new KomutModel { CompName = compName, Command = command, DateSent = DateTime.Now.ToString() };
                string jsonString = model.ToJson();
                System.Console.WriteLine(jsonString);
                await SendCommand(jsonString);
                TempData["Info"] = "Komut gönderildi!";
                return RedirectToAction("DashboardMain", "Dashboard");
            }
            else
            {
                TempData["Alert"] = "Komut ya da Bilgisayar adı hatalı!";
                return RedirectToAction("Commands", "Dashboard");
            }
        }
        else
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
    }
    private async Task SendCommand(string model)
    {
        var content = new StringContent(model, Encoding.UTF8, "application/json");

        string _serverUrl = "http://localhost:5105/api/KomutApi/SendCommand";

        HttpClient _httpClient = new HttpClient();

        try
        {
            var response = await _httpClient.PostAsync(_serverUrl, content);

            if (response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("Komut gönderildi");

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"HATA: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }

    [HttpGet]
    public IActionResult CommandList(int page = 1, string sortColumn = "ID", string sortOrder = "asc", string? searchedColumn = null, string? searchedValue1 = null, string? searchedValue2 = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }

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

        int pageSize = 10;
        page = Math.Max(page, 1);

        IPagedList<KomutModel> pagedList = commands.ToPagedList(page, pageSize);

        ViewBag.SortColumn = sortColumn;
        ViewBag.SortOrder = sortOrder;
        ViewBag.SearchedColumn = searchedColumn;
        ViewBag.SearchedValue1 = searchedValue1;
        ViewBag.SearchedValue2 = searchedValue2;


        return View(pagedList);
    }



}
