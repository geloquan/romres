using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;
using Newtonsoft.Json;

namespace WebApplication2.Controllers {
    public partial class SlotController : Controller {
        public IActionResult Calendar() {
            return View("Calendar");
        }
    }
}