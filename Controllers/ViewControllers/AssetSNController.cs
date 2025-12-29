using System.Text;
using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using EnvanterYonetimPaneli.Filters;

namespace EnvanterYonetimPaneli.Controllers;

[SessionAuthorize]
public class AssetSNController : Controller
{
    private readonly string? _connectionString;

    private EnvanterRepo _envanterRepo;


    public AssetSNController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);


    }

    public IActionResult AssetSNMatcher()
    {

        return View();

    }

    public IActionResult MatchAssetSN(EnvanterModel envanterModel)
    {

        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return RedirectToAction("AssetSNMatcher", "AssetSN");
        }
        else
        {
            envanterModel.Log = HttpContext.Session.GetString("Username") + " tarafindan yapilan asset atama islemi.";
            envanterModel.DateChanged = DateTime.Now;
            try
            {
                if (envanterModel == null)
                    throw new Exception("Model null geldi.");

                _envanterRepo.AddToSql(envanterModel);
                return RedirectToAction("Details", "Dashboard", new { id = envanterModel.Id, page = 1 });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                TempData["Alert"] = "Asset atama sırasında bir hata oluştu!";
                return RedirectToAction("AssetSNMatcher", "AssetSN");
            }

        }


    }

}