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
        [HttpPut("/slot/{id}/noteedit")]
        public IActionResult SlotNoteEdit(int id, [FromBody] HttpPutSlotNoteEdit request) {
            Console.WriteLine("[HttpPut(/slot/{id}/noteedit)]");
            if (request != null) {
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
        [HttpGet("/slot/{invitation_code}/exists")]
        public IActionResult SlotSearch(string invitation_code) {
            HttpGetSlotSearch httpGetSlotSearch = new HttpGetSlotSearch();
            httpGetSlotSearch.InvitationCode = invitation_code;
            Console.WriteLine("[HttpGet(/slot/{invitation_code}/exists)]");
            if (invitation_code != null) {
                bool processingResult = httpGetSlotSearch.Process();
                if (processingResult) {
                    return Ok(httpGetSlotSearch.favoriteSlots); 
                }
                else {
                    return StatusCode(500, "Failed to process reservation."); 
                }
            }
            return StatusCode(500, "Invalid request data.");
        }
    }
}