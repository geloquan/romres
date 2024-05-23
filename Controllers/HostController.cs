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
        [HttpPut("/host/duplicate/slot")]
        public IActionResult DuplicateParentSlot([FromBody] HttpPutParentSlotDuplication duplicationRequest) {
            Console.WriteLine("DuplicateParentSlot()");
            if (duplicationRequest != null) {
                bool processingResult = duplicationRequest.Process2();
                if (processingResult) {
                    return Ok("errmm");
                }
                else {
                    return StatusCode(500, "Failed to process new host.");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
        [HttpPost("/host/delete")]
        public IActionResult DeleteHosts([FromBody] HttpPostDeleteSelectedHosts deletionRequest) {
            Console.WriteLine("DeleteHosts()");
            if (deletionRequest != null) {
                bool processingResult = deletionRequest.Process();
                if (processingResult) {
                    return Ok("success DeleteHosts");
                }
                else {
                    return StatusCode(500, "Failed to process DeleteHosts.");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
    }
}
