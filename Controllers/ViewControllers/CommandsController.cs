using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using System.Text;

namespace EnvanterYonetimPaneli.Controllers;

public class CommandsController : Controller
{
    private readonly string? _connectionString;
    private readonly KomutRepo _komutRepo;


    public CommandsController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _komutRepo = new KomutRepo(configuration);
    }

    public IActionResult CommandList(int page = 1,
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

        if (commands != null)
        {
            IPagedList<KomutModel> pagedList = commands.ToPagedList(page, pageSize);
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
    public IActionResult SendCommand(string command, string compName)
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
                DateSent = DateTime.Now.ToString(),
                User = UserModel.User.Username
            };

            try
            {
                if(model == null)
                    throw new Exception("Model null geldi.");

                _komutRepo.AddToSql(model, false);

                TempData["Success"] = "Komut başarıyla gönderildi.";
                return RedirectToAction("CommandList", "Commands");
            }
            catch (Exception)
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