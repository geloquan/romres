using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    [Route("/")]
    public class HomeController : Controller {
        //private readonly ILogger<HomeController> _logger;
        //public HomeController(ILogger<HomeController> logger) {
        //    _logger = logger;
        //}
        public IActionResult Index() {
            return View();
        }
        public IActionResult Branch() {
            DatabaseConnect databaseConnect = new DatabaseConnect();
            databaseConnect.OnGete();
            return View(databaseConnect);
        }
        [Route("/HostDashboard/{HostId}")]
        public IActionResult HostDashboard(int HostId) {
            HostEntityData databaseConnect = new HostEntityData();
            databaseConnect.InitRootSlots(HostId);
            return View(databaseConnect);
        }
        [Route("/HostDashboard/{HostId}/Slot/{SlotId}")]
        public IActionResult SlotPrimary(int SlotId) {
            HostEntityData databaseConnect = new HostEntityData();
            databaseConnect.InitRootSlots(SlotId);
            return View(databaseConnect);
        }
        public IActionResult Privacy() {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error() {
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}