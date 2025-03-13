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
            List<EnvanterModel> comps = GetEnvanterList();
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
    private List<EnvanterModel> GetEnvanterList()
    {
        List<EnvanterModel> envanterList = new List<EnvanterModel>();



        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = "SELECT Asset, [Seri No], [Bilgisayar Modeli], [Bilgisayar Adı], RAM, [Disk Boyutu], [MAC Adresi], [İşlemci Modeli], Kullanıcı, [Değişiklik Tarihi] FROM \"000.000.00000\"";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    envanterList.Add(new EnvanterModel
                    {
                        Asset = reader.GetString(0),
                        SeriNo = reader.GetString(1),
                        CompModel = reader.GetString(2),
                        CompName = reader.GetString(3),
                        RAM = reader.GetString(4),
                        DiskBoyutu = reader.GetString(5),
                        MacAddress = reader.GetString(6),
                        ProcModel = reader.GetString(7),
                        User = reader.GetString(8),
                        DegisiklikTarihi = reader.GetString(9)
                    });
                }
            }

        }
        return envanterList;
    }

    [HttpPost]
    public IActionResult AssetSNMatcher(string asset, string seriNo)
    {
        if (string.IsNullOrEmpty(seriNo) || string.IsNullOrEmpty(asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return RedirectToAction("DashboardAssetSNMatcher", "Dashboard");
        }
        else
        {
            TempData["Info"] = _envanterRepo.AssetSNMatcher(asset, seriNo);
            return RedirectToAction("DashboardAssetSNMatcher", "Dashboard");

        }
    }
}
