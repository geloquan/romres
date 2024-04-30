using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    public class UserController : Controller {
        public IActionResult SayHello() {
            return Content("hello");
        }
        public IActionResult Id(int HostId, string UserName) {
            Console.WriteLine("3");
            ViewData["UserName"] = UserName;
            HostEntityData databaseConnect = new HostEntityData();
            databaseConnect.InitRootSlots(HostId);
            return View("Home", databaseConnect);
        }
        public IActionResult Login(LoginModel Host) {
            Console.WriteLine("2");
            HostEntityLogin hostEntityLogin = new HostEntityLogin();
            LoginSuccessModel loginSuccessModel = hostEntityLogin.Verify(Host, "host");

            if (loginSuccessModel.Valid) {
                int? nullableInt = loginSuccessModel.loginModel.Id; 
                int UserId = nullableInt ?? 0; 
                return RedirectToAction("Id", new { HostId = UserId, UserName =  loginSuccessModel.loginModel.UserName});
            } else if (loginSuccessModel.Valid == false){
                return View("Error");
            } else {
                return View("Error");
            }
        }
        public IActionResult Index() {
            Console.WriteLine("0");
            return View("Index");
        }
        public IActionResult Home() {
            Console.WriteLine("1");
            return View("Home");
        }
    }
}