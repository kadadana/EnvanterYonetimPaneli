using Microsoft.AspNetCore.Mvc;
using EnvanterYonetimPaneli.Models;

namespace EnvanterYonetimPaneli.Controllers;

public class LoginController : Controller
{

    public IActionResult LoginIndex()
    {
        if (!UserModel.DefaultUser.IsLoggedIn)
        {
            return View();
        }
        else
        {
            return RedirectToAction("DashboardMain", "Dashboard");
        }
    }

    [HttpPost]
    public IActionResult LoginIndex(UserModel userInput)
    {
        if (!UserModel.DefaultUser.IsLoggedIn)
        {

            if (userInput.Username == UserModel.DefaultUser.Username && userInput.Password == UserModel.DefaultUser.Password)
            {
                UserModel.DefaultUser.IsLoggedIn = true;
                HttpContext.Session.SetString("IsLoggedIn", "true");
                TempData["Info"] = "Giriş yaptınız!";
                return RedirectToAction("DashboardMain", "Dashboard");
            }
            else
            {

                TempData["Alert"] = "Hatalı kullanıcı adı veya şifre!";
                return View("LoginIndex");
            }
        }
        else
        {
            TempData["Info"] = "Zaten giriş Yapılmış!";
            return RedirectToAction("DashboardMain", "Dashboard");
        }
    }
    public IActionResult Logout()
    {
        UserModel.DefaultUser.IsLoggedIn = false;
        HttpContext.Session.Remove("IsLoggedIn");
        TempData["Info"] = "Çıkış yaptınız!";
        return RedirectToAction("LoginIndex", "Login");
    }




}
