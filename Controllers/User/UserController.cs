using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    public class UserController : Controller {

        public IActionResult Id(int user_id) {
            Console.WriteLine("3");
            Console.WriteLine("user id: " + user_id);
            UserEntityLogin userEntityLogin = new UserEntityLogin();
            userEntityLogin.DirectLogin(user_id);
            
            Console.WriteLine("user id: " + userEntityLogin.Id);
            Console.WriteLine("user name: " + userEntityLogin.Name);
            ViewData["UserId"] = userEntityLogin.Id;
            ViewData["UserName"] = userEntityLogin.Name;
            return View("Home");
        }
        [HttpGet]
        public IActionResult FavoriteSlots() {
            Console.WriteLine("FavoriteSlots()");
            if (Request.Query.ContainsKey("userId") && int.TryParse(Request.Query["userId"], out int userId)) {
                UserEntityData userEntityData = new UserEntityData();
                userEntityData.FavoriteSlots(userId);
                return new JsonResult(userEntityData.favoriteSlots);
            }
            return BadRequest("UserId is missing or invalid.");
        }
        public IActionResult Login(LoginModel User) {
            Console.WriteLine("2");
            UserEntityLogin userEntityLogin = new UserEntityLogin();
            userEntityLogin.Verify(User);
            if (userEntityLogin.Verified) {
                int? nullableInt = userEntityLogin.Id; 
                int UserId = nullableInt ?? 69; 
                return RedirectToRoute("userWithId", new { user_id = userEntityLogin.Id });
            } else if (!userEntityLogin.Verified){
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