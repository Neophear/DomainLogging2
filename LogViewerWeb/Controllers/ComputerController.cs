using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogViewerWeb.Controllers
{
    public class ComputerController : Controller
    {
        // GET: Computer
        [Authorize]
        public ActionResult Index()
        {
            using (((System.Security.Principal.WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return View(client.GetComputerInfos());
        }

        // GET: Computer/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            using (((System.Security.Principal.WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return View(client.GetComputerInfoFromId(id));
        }

        [Authorize]
        public ActionResult ComputerLog(int id)
        {
            using (((System.Security.Principal.WindowsIdentity)HttpContext.User.Identity).Impersonate())
                using (var client = new AdminService.AdminServiceClient())
                    return View(client.GetComputerLog(id));
        }
    }
}