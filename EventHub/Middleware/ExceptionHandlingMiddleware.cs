
namespace EventHub.Web.Middleware
{
    using EventHub.Core.AppException;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public class ExceptionHandlingMiddleware 
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
           catch(DomainException ex)
            {
                await HandleDomainExceptionAsync(context,ex);
            }
            catch (Exception ex)
            {
                await HandleGenericExceptionAsync(context);
            }
        }

        private static Task HandleDomainExceptionAsync(HttpContext context, DomainException ex)
        {
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            context.Response.Clear();

            context.Response.Cookies.Append
                  (
                "eh_error",
                Uri.EscapeDataString(ex.Message),
                new CookieOptions
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = context.Request.IsHttps,
                    MaxAge = TimeSpan.FromSeconds(10)
                });

            var isPost = HttpMethods.IsPost(context.Request.Method);
            var target =  context.Request.Headers["Referer"].ToString();
            context.Response.Redirect(string.IsNullOrEmpty(target) ? "/" : target);

            return Task.CompletedTask;
        }

        private static Task HandleGenericExceptionAsync(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Redirect($"/Home/Error");
            return Task.CompletedTask;
        }
    }
}
