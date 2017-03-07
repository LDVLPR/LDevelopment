using System;
using System.Web.Mvc;
using LDevelopment.Models;
using LDevelopment.Repositories;
using Microsoft.ApplicationInsights;

namespace LDevelopment.Controllers
{
    [RequireHttps]
    public class BaseController : Controller
    {
        protected Repository Repository;

        public BaseController()
        {
            Repository = new Repository();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.HttpContext != null && filterContext.Exception != null)
            {
                //If customError is Off, then AI HTTPModule will report the exception
                if (filterContext.HttpContext.IsCustomErrorEnabled)
                {
                    var ai = new TelemetryClient();
                    ai.TrackException(filterContext.Exception);
                }

                filterContext.ExceptionHandled = true;
                filterContext.Result = View("Error");

                var logModel = new LogModel
                {
                    Message = filterContext.Exception.InnerException?.Message ?? filterContext.Exception.Message,
                    StackTrace = filterContext.Exception.StackTrace.Trim(),
                    ControllerName = filterContext.RouteData.Values["controller"]?.ToString() ?? "",
                    ActionName = filterContext.RouteData.Values["action"]?.ToString() ?? "",
                    Parameters = filterContext.RouteData.Values["id"]?.ToString() ?? "",
                    CreatedDate = DateTime.UtcNow
                };

                Repository.Add(logModel);
                Repository.Save();
            }

            base.OnException(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Repository.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}