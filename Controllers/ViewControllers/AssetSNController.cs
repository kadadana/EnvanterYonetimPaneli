using System.Text;
using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnvanterYonetimPaneli.Controllers;

public class AssetSNController : Controller
{
    private readonly string? _connectionString;

    private EnvanterRepo _envanterRepo;
    private readonly HttpClient _http;
    private string _url;

    public AssetSNController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);
        _http = factory.CreateClient();

        _url = "http://localhost:5105/api/EnvanterApi";

    }

    public IActionResult AssetSNMatcher()
    {
        if (UserModel.User.IsLoggedIn)
        {
            return View();
        }
        else
        {
            return RedirectToAction("LoginIndex", "Login");
        }
    }

    public async Task<IActionResult> MatchAssetSN(EnvanterModel envanterModel)
    {
        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return RedirectToAction("AssetSNMatcher", "AssetSN");
        }
        else
        {
            envanterModel.Log = "Asset atama islemi.";
            envanterModel.DateChanged = DateTime.Now.ToString();

            var json = System.Text.Json.JsonSerializer.Serialize(envanterModel);
            var response = await _http.PostAsync(_url, new StringContent(json, Encoding.UTF8, "application/json"));
            System.Console.WriteLine(json);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Alert"] = "Asset atama sırasında bir hata oluştu!";
            }
            return RedirectToAction("AssetSNMatcher", "AssetSN");

        }
    }

}