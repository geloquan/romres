using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Diagnostics;

namespace WebApplication2.Controllers {
    public class ReserveeController : Controller {
        public IActionResult Index() {
            return View();
        }
        public IActionResult Id(int reservee_id) {
            return View();
        }
        public IActionResult Slot(int reservee_id, int slot_id) {
            return View();
        }
    }
}