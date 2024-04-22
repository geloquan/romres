using Microsoft.AspNetCore.Mvc;
using qweqwe.Models;
using System.Diagnostics;

namespace romres.Controllers {
    public class HostController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult Id() {
            return View();
        }
    }
}
