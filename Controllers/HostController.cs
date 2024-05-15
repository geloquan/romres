using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Diagnostics;

namespace WebApplication2.Controllers {
    public class HostController : Controller {
        public IActionResult Index() {
            return View();
        }
        public IActionResult Id(int host_id) {
            return View();
        }
        public IActionResult Slot(int host_id, int slot_id) {
            return View();
        }
        [HttpPut("/host/newhost")]
        public IActionResult NewHost(int userId, [FromBody] HttpPutNewHost newHostRequest) {
            Console.WriteLine("NewHost()");
            if (newHostRequest != null) {
                bool processingResult = newHostRequest.Process();
                if (processingResult) {
                    var result = new {
                        response = "Reservation successful",
                        new_slot_id = newHostRequest.newSlotId
                    };
                    return new JsonResult(result);
                }
                else {
                    return StatusCode(500, "Failed to process new host.");  // Return appropriate error status
                }
            } else {
                return StatusCode(500, "Invalid request data.");
            }
        }
    }
}
