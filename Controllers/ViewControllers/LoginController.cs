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
        if (!UserModel.User.IsLoggedIn)
        {
            return View();
        }
        else
        {
            return RedirectToAction("DashboardMain", "Dashboard");
        }
    }

    [HttpPost]
    public async Task<IActionResult> LoginIndex(UserModel userInput)
    {
        if (!UserModel.User.IsLoggedIn)
        {
            await _authController.ValidateUser(userInput.Username, userInput.Password);

            if (UserModel.User.IsLoggedIn)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                TempData["Info"] = "Giriş yaptınız!";
                return RedirectToAction("DashboardMain", "Dashboard");
            }
            else
            {
                TempData["Alert"] = "Hatalı kullanıcı adı veya şifre!";
                return View("LoginIndex");
            }
            /*
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
            */
        }
        else
        {
            TempData["Info"] = "Zaten giriş Yapılmış!";
            return RedirectToAction("DashboardMain", "Dashboard");
        }
    }
    public IActionResult Logout()
    {
        UserModel.User.IsLoggedIn = false;
        HttpContext.Session.Remove("IsLoggedIn");
        TempData["Info"] = "Çıkış yaptınız!";
        return RedirectToAction("LoginIndex", "Login");
    }


}


