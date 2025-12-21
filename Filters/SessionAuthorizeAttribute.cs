using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EnvanterYonetimPaneli.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var isLoggedIn = session.GetString("IsLoggedIn");

            if (isLoggedIn != "true")
            {
                var tempDataFactory =
                context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>();

            var tempData =
                tempDataFactory.GetTempData(context.HttpContext);

            tempData["Alert"] = "Giriş yapmalısınız!";
                context.Result = new RedirectToActionResult(
                    "LoginIndex",
                    "Login",
                    null
                );
            }

            base.OnActionExecuting(context);
        }
    }
}