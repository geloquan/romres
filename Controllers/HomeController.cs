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
        [Route("/branch")]
        public IActionResult Branch() {
            DatabaseConnect databaseConnect = new DatabaseConnect();
            databaseConnect.OnGete();
            return View(databaseConnect);
        }
        [Route("/HostDashboard/{HostId}")]
        public IActionResult HostDashboard(int HostId) {
            Console.WriteLine("3");
            HostEntityData databaseConnect = new HostEntityData();
            databaseConnect.InitRootSlots(HostId);
            return View(databaseConnect);
        }
        [Route("/HostDashboard/")]
        public IActionResult HostLoginPage() {
            Console.WriteLine("1");
            return View("HostLoginPage");
        }
        public IActionResult HostLogin(LoginModel Host) {
            Console.WriteLine("2");
            HostEntityLogin hostEntityLogin = new HostEntityLogin();
            LoginSuccessModel loginSuccessModel = hostEntityLogin.Verify(Host, "host");

            if (loginSuccessModel.Valid) {
                int? nullableInt = loginSuccessModel.loginModel.Id; 
                int qwe = nullableInt ?? 0; 
                return HostDashboard(qwe);
            } else if (loginSuccessModel.Valid == false){
                return View("Error");
            } else {
                return View("Error");
            }
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