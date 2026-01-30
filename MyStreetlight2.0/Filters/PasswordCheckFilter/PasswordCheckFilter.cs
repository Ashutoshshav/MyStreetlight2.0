using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Streetlight2._0.Filters.PasswordCheckFilter
{
    public class PasswordCheckFilter : IAsyncActionFilter
    {
        private readonly string _expectedPassword = "12321";
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly IConfiguration _configuration;

        public PasswordCheckFilter(ITempDataDictionaryFactory tempDataFactory, IConfiguration configuration)
        {
            _tempDataFactory = tempDataFactory;
            _configuration = configuration;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var startDateString = _configuration["LightActionPassDuration:StartDate"];
            var endDateString = _configuration["LightActionPassDuration:EndDate"];

            var startDate = DateTime.ParseExact(startDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            //Console.WriteLine(startDate);
            //Console.WriteLine(endDate);

            string password = null;

            if (httpContext.Request.HasFormContentType)
            {
                // Read password from form
                password = httpContext.Request.Form["password"];
            }
            else
            {
                // Allow body to be read multiple times
                httpContext.Request.EnableBuffering();
                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0; // reset so controller can read again

                if (!string.IsNullOrEmpty(body))
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("password", out var pwdElement))
                    {
                        password = pwdElement.GetString();
                    }
                }
            }

            if (password != _expectedPassword && (DateTime.Today < startDate || DateTime.Today > endDate))
            {
                //context.Result = new UnauthorizedResult();
                //return;
                var tempData = _tempDataFactory.GetTempData(httpContext);
                tempData["ErrorFeedback"] = "Wrong Password, Please try again.";
                context.Result = new RedirectResult(httpContext.Request.Headers["Referer"].ToString());
                return;
            }

            await next();
        }
    }
}
