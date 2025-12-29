using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace EnvanterYonetimPaneli.Filters
{
    /// <summary>
    /// API anahtarı doğrulaması için özel yetkilendirme filtresi.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "EYP_API_KEY";
        private const string ConfigApiKeyName = "EYP_API_KEY";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var envApiKey = configuration[ConfigApiKeyName];

            if (string.IsNullOrEmpty(envApiKey))
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) || !extractedApiKey.Equals(envApiKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}