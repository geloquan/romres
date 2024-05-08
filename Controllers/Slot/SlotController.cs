using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    public class SlotController : Controller {
        [HttpPut("/slot/{id}/reserve")]
        public IActionResult Reserve(int id, [FromBody] HttpPutReserve request) {
            if (request != null && request.Reserve != null && request.UserId != null) {
                bool processingResult = request.Process();
                if (processingResult) {
                    return Ok("Reservation successful.");
                }
                else {
                    return StatusCode(500, "Failed to process reservation.");  // Return appropriate error status
                }
            }
            return StatusCode(500, "Invalid request data.");
        }
    }
}