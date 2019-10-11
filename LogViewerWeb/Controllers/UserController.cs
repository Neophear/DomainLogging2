using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogViewerWeb.Controllers
{
    public class UserController : Controller
    {
        // GET: User/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            using (((System.Security.Principal.WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return View(client.GetUser(id));
        }

        [Authorize]
        public ActionResult UserLog(string id)
        {
            using (((System.Security.Principal.WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return PartialView(client.GetUserLog(id));
        }
    }
}
