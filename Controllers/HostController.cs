using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Diagnostics;

namespace WebApplication2.Controllers {
    public class HostController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult Id(int host_id) {
            //show slots where host_id and parent_slot == null;
            return View();
        }
        public IActionResult Slot(int host_id, int slot_id) {
            //show slots where host_id and parent_slot == null;
            return View();
        }
    }
}
