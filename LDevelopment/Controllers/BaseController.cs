using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    [RequireHttps]
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnException(ExceptionContext exceptionContext)
        {
            exceptionContext.ExceptionHandled = true;
            exceptionContext.Result = RedirectToAction("Error", "Home");

            var logModel = new LogModel
            {
                Message = exceptionContext.Exception.InnerException?.Message ?? exceptionContext.Exception.Message,
                StackTrace = exceptionContext.Exception.StackTrace.Trim(),
                ControllerName = exceptionContext.RouteData.Values["controller"]?.ToString() ?? "",
                ActionName = exceptionContext.RouteData.Values["action"]?.ToString() ?? "",
                Parameters = exceptionContext.RouteData.Values["id"]?.ToString() ?? ""
            };

            db.Logs.Add(logModel);
            db.SaveChanges();

            base.OnException(exceptionContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}