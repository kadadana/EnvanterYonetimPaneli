using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using System.Text;

namespace EnvanterYonetimPaneli.Controllers;

public class CommandsController : Controller
{
    private readonly string? _connectionString;
    private readonly HttpClient _http;
    private readonly KomutRepo _komutRepo;
    private string url;

    public CommandsController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _http = factory.CreateClient();
        _komutRepo = new KomutRepo(configuration);
        url = "http://localhost:5105/api/KomutApi";
    }

    public async Task<IActionResult> CommandList(int page = 1,
                                                 string sortColumn = "Id",
                                                 string sortOrder = "desc",
                                                 string? searchedColumn = null,
                                                 string? searchedValue1 = null,
                                                 string? searchedValue2 = null)
    {
        if (!UserModel.User.IsLoggedIn)
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
            return View(new List<KomutModel>().ToPagedList(page, 10));
        }

        var json = await response.Content.ReadAsStringAsync();
        var cmds = System.Text.Json.JsonSerializer.Deserialize<List<KomutModel>>(json);

        int pageSize = 10;
        page = Math.Max(page, 1);

        if (cmds != null)
        {
            IPagedList<KomutModel> pagedList = cmds.ToPagedList(page, pageSize);
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
            return View(new List<KomutModel>().ToPagedList(page, 10));
        }


    }

    public IActionResult CommandPage(string? id = null)
    {
        if (!UserModel.User.IsLoggedIn)
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
        var model = new KomutModel();
        model.CompName = id;

        return View(model);

    }
    public async Task<IActionResult> SendCommand(string command, string compName)
    {
        if (!UserModel.User.IsLoggedIn)
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }

        if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(compName))
        {
            var model = new KomutModel
            {
                Id = _komutRepo.IdDeterminer(),
                CompName = compName,
                Command = command,
                DateSent = DateTime.Now.ToString()
            };
            model.User = UserModel.User.Username;
            var jsonString = System.Text.Json.JsonSerializer.Serialize(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Komut başarıyla gönderildi.";
                return RedirectToAction("CommandList", "Commands");
            }
            else
            {
                TempData["Alert"] = "Komut gönderilemedi!";
                return RedirectToAction("Command", "Commands");
            }

        }
        else
        {
            TempData["Alert"] = "Komut ya da Bilgisayar adı hatalı!";
            return RedirectToAction("Command", "Commands");
        }

    }

}