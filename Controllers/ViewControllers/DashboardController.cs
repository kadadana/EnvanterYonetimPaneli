using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;
using EnvanterYonetimPaneli.Filters;


namespace EnvanterYonetimPaneli.Controllers;

[SessionAuthorize]

public class DashboardController : Controller
{
    private readonly string? _connectionString;
    private readonly EnvanterRepo _envanterRepo;


    public DashboardController(IConfiguration configuration, IHttpClientFactory factory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _envanterRepo = new EnvanterRepo(configuration);

    }

    public IActionResult DashboardMain(int page = 1,
                                                   string sortColumn = "ID",
                                                   string sortOrder = "desc",
                                                   string? searchedColumn = null,
                                                   string? searchedValue1 = null,
                                                   string? searchedValue2 = null)
    {
        List<EnvanterModel>? comps;


        if (!string.IsNullOrEmpty(searchedColumn) && !string.IsNullOrEmpty(searchedValue1))
        {
            comps = _envanterRepo.GetSearchedTable(searchedColumn, searchedValue1, searchedValue2);
        }
        else
        {
            comps = _envanterRepo.GetOrderedList(sortColumn, sortOrder);
        }


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

    public IActionResult Details(string id, int page)
    {
        IPagedList<EnvanterModel>? pagedList;
        var selectedComp = _envanterRepo.GetEnvanterModelById(id);
        if (selectedComp == null)
            return NotFound("Kayit bulunamadi!");

        List<EnvanterModel>? history = _envanterRepo.GetEnvanterHistoryById(id);
        int pageSize = 10;
        page = Math.Max(page, 1);
        if (history != null)
        {
            pagedList = history?.ToPagedList(page, pageSize);
        }
        else
        {
            pagedList = new List<EnvanterModel>().ToPagedList(page, pageSize);
        }

        if (selectedComp == null)
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
    public IActionResult EditPage(string? id = null)
    {

        if (!string.IsNullOrEmpty(id))
        {
            EnvanterModel? comp = _envanterRepo.GetLatestRowById(id);
            return View("EditPage", comp);
        }
        else
        {
            TempData["Alert"] = "Id belirtilmeli.";
            return RedirectToAction("DashboardMain", "Dashboard");
        }


    }

    public IActionResult Edit(EnvanterModel envanterModel)
    {

        if (string.IsNullOrEmpty(envanterModel.SeriNo) || string.IsNullOrEmpty(envanterModel.Asset))
        {
            TempData["Alert"] = "Seri numarası veya asset boş olamaz!";
            return EditPage(envanterModel.Id);
        }
        else
        {
            try
            {
                envanterModel.DateChanged = DateTime.Now;
                envanterModel.Log = HttpContext.Session.GetString("Username") + " tarafindan yapilan duzenleme islemi.";

                if (envanterModel == null)
                    throw new Exception("Model null geldi!");

                var existingComp = _envanterRepo.GetEnvanterModelById(envanterModel.Id!);
                if (existingComp == null)
                    throw new Exception("Kayıt bulunamadı!");

                _envanterRepo.AddToSql(envanterModel);
                TempData["Info"] = "Düzenleme başarılı.";
                return RedirectToAction("Details", new { id = envanterModel.Id, page = 1 });
            }
            catch (Exception)
            {
                TempData["Alert"] = "Düzenleme hatası!";
                return EditPage(envanterModel.Id);
            }

        }
    }

}
