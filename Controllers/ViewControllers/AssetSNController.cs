using System.Text;
using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnvanterYonetimPaneli.Controllers;

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
        if (!UserModel.User.IsLoggedIn)
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
        return View();

    }

    public IActionResult MatchAssetSN(EnvanterModel envanterModel)
    {
        if (!UserModel.User.IsLoggedIn)
        {
            TempData["Alert"] = "Giriş Yapmalısınız!";
            return RedirectToAction("LoginIndex", "Login");
        }
        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return RedirectToAction("AssetSNMatcher", "AssetSN");
        }
        else
        {
            envanterModel.Log = UserModel.User + " tarafindan yapilan asset atama islemi.";
            envanterModel.DateChanged = DateTime.Now.ToString();
            try
            {
                if (envanterModel == null)
                    throw new Exception("Model null geldi.");

                _envanterRepo.AddToSql(envanterModel);
                return RedirectToAction("Details", new { id = envanterModel.Id, page = 1 });
            }
            catch (System.Exception)
            {
                TempData["Alert"] = "Asset atama sırasında bir hata oluştu!";
                return RedirectToAction("AssetSNMatcher", "AssetSN");
            }



        }


    }

}