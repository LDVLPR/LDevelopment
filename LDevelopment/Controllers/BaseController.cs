using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    [RequireHttps]
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = RedirectToAction("Error", "Home");

            var logModel = new LogModel
            {
                Message = filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.Message : filterContext.Exception.Message,
                StackTrace = filterContext.Exception.StackTrace.Trim(),
                ControllerName = filterContext.RouteData.Values["controller"] != null ? filterContext.RouteData.Values["controller"].ToString() : "",
                ActionName = filterContext.RouteData.Values["action"] != null ? filterContext.RouteData.Values["action"].ToString() : "",
                Parameters = filterContext.RouteData.Values["id"] != null ? filterContext.RouteData.Values["id"].ToString() : ""
            };

            db.Logs.Add(logModel);
            db.SaveChanges();

            base.OnException(filterContext);
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