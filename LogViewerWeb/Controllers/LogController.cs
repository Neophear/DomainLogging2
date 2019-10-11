using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace LogViewerWeb.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        [Authorize]
        public ActionResult Index()
        {
            using (((WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return View(client.GetLog(500));
        }
    }
}
