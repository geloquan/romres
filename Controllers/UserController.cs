using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    public class UserController : Controller {
        public IActionResult Id(int HostId) {
            Console.WriteLine("3");
            HostEntityData databaseConnect = new HostEntityData();
            databaseConnect.InitRootSlots(HostId);
            return View(databaseConnect);
        }
        public IActionResult Login(LoginModel Host) {
            Console.WriteLine("2");
            HostEntityLogin hostEntityLogin = new HostEntityLogin();
            LoginSuccessModel loginSuccessModel = hostEntityLogin.Verify(Host, "host");

            if (loginSuccessModel.Valid) {
                int? nullableInt = loginSuccessModel.loginModel.Id; 
                int UserId = nullableInt ?? 0; 
                return Id(UserId);
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