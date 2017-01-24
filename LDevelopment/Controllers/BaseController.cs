using System.Web.Mvc;
using LDevelopment.Models;

namespace LDevelopment.Controllers
{
    [RequireHttps]
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();

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