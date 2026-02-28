namespace EventHub.Web.Filter
{
    using EventHub.Core.AppException;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.DependencyInjection;
    using static System.Net.WebRequestMethods;

    public class DomainExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not DomainException ex)
                return;

            var http = context.HttpContext;
            var controller = context.RouteData.Values["controller"]?.ToString();
            var returnUrl = context.HttpContext.Request.Query["returnUrl"].ToString();

            var tempDataFactory = http.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(http);
            tempData["Error"] = ex.Message;

            // Check if a URL that points to your site, not to another domain. 
            //Redirect Vulnerability
            var urlHelperFactory = http.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(context);
            
            if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
            {
                context.Result = new RedirectResult(returnUrl);
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", controller, null);
            }

            context.ExceptionHandled = true;
        }
    }
}