using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;
using Newtonsoft.Json;

namespace WebApplication2.Controllers {
    public partial class SlotController : Controller {
        public IActionResult Calendar(int user_id, int slot_id) {
            Console.WriteLine("Calendar()");
            HttpGetCalendar httpGetCalendar = new HttpGetCalendar();
            httpGetCalendar.slot_id = slot_id;
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
        [HttpPatch("calendar/edit")]
        public IActionResult CalendarEdit([FromBody] HttpPatchCalendar request) {
            Console.WriteLine("CalendarEdit()");
            if (request != null) {
                bool processingResult = request.Process();
                if (processingResult) {
                    return Ok("errmm");
                }
                else {
                    return StatusCode(500, "Failed Edit().");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
    }
}