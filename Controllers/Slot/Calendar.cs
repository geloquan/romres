using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;
using Newtonsoft.Json;

namespace WebApplication2.Controllers {
    public partial class SlotController : Controller {
        public IActionResult Calendar(int user_id, int slot_id) {
            HttpGetCalendar httpGetCalendar = new HttpGetCalendar();
            bool success = httpGetCalendar.Process();
            if (success) {
                var calendarModel = JsonConvert.SerializeObject(httpGetCalendar.calendarModel);
                ViewBag.CalendarModel = calendarModel;
                return View("Calendar");
            } else {
                return View("Error");
            }

        }
        [HttpPost("User/{user_id}/Slot/{slot_id}/calendar")]
        public IActionResult SaveChanges([FromBody] HttpPutReserve request) {
            return Ok();
        }
    }
}