using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;
using Newtonsoft.Json;

namespace WebApplication2.Controllers {
    public class SlotController : Controller {
        [HttpPost("/slot/delete")]
        public IActionResult Delete([FromBody] HttpPostDelete request) {
            Console.WriteLine("slot/Delete()");
            if (request != null) {
                bool processingResult = request.Process();
                if (processingResult) {
                    return Ok("errmm");
                }
                else {
                    return StatusCode(500, "Failed Delete().");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
        
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
                    return Ok(httpGetSlotSearch.slotTree); 
                }
                else {
                    return StatusCode(500, "Failed to process reservation."); 
                }
            }
            return StatusCode(500, "Invalid request data.");
        }
        public IActionResult InvitationCode(string invitation_code) {
            HttpGetSlotSearch httpGetSlotSearch = new HttpGetSlotSearch();
            httpGetSlotSearch.InvitationCode = invitation_code;
            Console.WriteLine("Slot/InvitationCode()");
            if (invitation_code != null) {
                bool processingResult = httpGetSlotSearch.Process();
                if (processingResult) {
                    var slotTreeJson = JsonConvert.SerializeObject(httpGetSlotSearch.slotTree);
                    ViewBag.SlotTreeJson = slotTreeJson;
                    return View("InvitationCode"); 
                }
                else {
                    return View("Error");
                }
            }
            return View("Error");
        }
    }
}