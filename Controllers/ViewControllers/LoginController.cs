using Microsoft.AspNetCore.Mvc;
using EnvanterYonetimPaneli.Models;


namespace EnvanterYonetimPaneli.Controllers;

public class LoginController : Controller
{

    private readonly IConfiguration _configuration;
    AuthController _authController;

    public LoginController(IConfiguration configuration)
    {
        _configuration = configuration;
        _authController = new AuthController(_configuration);
    }

    public IActionResult LoginIndex()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            return RedirectToAction("DashboardMain", "Dashboard");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginIndex(UserModel userInput)
    {
        bool isValid = await _authController
                .ValidateUser(userInput.Username!, userInput.Password!);

        if (isValid)
        {
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("Username", userInput.Username!);

            TempData["Info"] = "Giriş yaptınız!";
            return RedirectToAction("DashboardMain", "Dashboard");
        }

        TempData["Alert"] = "Hatalı kullanıcı adı veya şifre!";
        return View();
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("IsLoggedIn");
        TempData["Info"] = "Çıkış yaptınız!";
        return RedirectToAction("LoginIndex", "Login");
    }


}


