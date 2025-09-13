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
    private readonly HttpClient _http;
    private string url;

    public DashboardController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
        _http = factory.CreateClient();
        url = "http://localhost:5105/api/EnvanterApi";
    }

    public async Task<IActionResult> DashboardMain(int page = 1,
                                                   string sortColumn = "ID",
                                                   string sortOrder = "asc",
                                                   string? searchedColumn = null,
                                                   string? searchedValue1 = null,
                                                   string? searchedValue2 = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }

        var newUrl = $"{url}/?sortColumn={sortColumn}&sortOrder={sortOrder}";

        if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
        {
            newUrl += $"&searchedColumn={searchedColumn}&searchedValue1={searchedValue1}&searchedValue2={searchedValue2}";
        }

        var response = await _http.GetAsync(newUrl);
        if (!response.IsSuccessStatusCode)
        {
            TempData["Alert"] = "Veri çekme hatası!";
            return View(new List<EnvanterModel>().ToPagedList(page, 10));
        }

        var json = await response.Content.ReadAsStringAsync();
        var comps = System.Text.Json.JsonSerializer.Deserialize<List<EnvanterModel>>(json);

        int pageSize = 10;
        page = Math.Max(page, 1);

        if (comps != null)
        {
            IPagedList<EnvanterModel> pagedList = comps.ToPagedList(page, pageSize);

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchedColumn = searchedColumn;
            ViewBag.SearchedValue1 = searchedValue1;
            ViewBag.SearchedValue2 = searchedValue2;

            return View(pagedList);
        }
        else
        {
            TempData["Alert"] = "Hiçbir veri bulunamadı!";
            return View(new List<EnvanterModel>().ToPagedList(page, 10));
        }
    }

    public async Task<IActionResult> Details(string id, int page)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            var newUrl = $"{url}/{id}";
            var response = await _http.GetAsync(newUrl);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Alert"] = "Veri çekme hatası!";
                return RedirectToAction("DashboardMain", "Dashboard");
            }
            var json = await response.Content.ReadAsStringAsync();
            var selectedComp = System.Text.Json.JsonSerializer.Deserialize<EnvanterModel>(json);

            List<EnvanterModel>? comps = _envanterRepo.GetSortedByDate(id);
            int pageSize = 10;
            page = page >= 1 ? page : 1;
            IPagedList<EnvanterModel>? pagedList = comps?.ToPagedList(page, pageSize);

            if(selectedComp == null)
            {
                TempData["Alert"] = "Böyle bir kayıt bulunamadı!";
                return RedirectToAction("DashboardMain", "Dashboard");
            }
            var selectedDisks = selectedComp.Id != null ? _envanterRepo.GetDiskListById(selectedComp.Id) : null;



            EnvanterViewModel viewModel = new EnvanterViewModel
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
    public IActionResult EditPage(string? id = null)
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

    public async Task<IActionResult> Edit(EnvanterModel envanterModel)
    {
        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return EditPage(envanterModel.Id);
        }
        else
        {
            envanterModel.DateChanged = DateTime.Now.ToString();
            envanterModel.Log = UserModel.User.Username + " tarafindan yapilan duzenleme islemi.";
            var jsonString = System.Text.Json.JsonSerializer.Serialize(envanterModel);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"{url}/{envanterModel.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Alert"] = "Güncelleme hatası!";
                return EditPage(envanterModel.Id);
            }
            TempData["Info"] = "Güncelleme başarılı.";

            return RedirectToAction("Details", new { id = envanterModel.Id, page = 1 });

        }
    }


}
