using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Diagnostics;

namespace WebApplication2.Controllers {
    public class AdminController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult Id(int admin_id) {
            return View();
        }
        public IActionResult Slot(int admin_id, string slot_name_slot_code) {
            return View();
        }
    }
}
