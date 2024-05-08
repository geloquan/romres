using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApplication2.Models;

namespace WebApplication2.Controllers {
    public class SlotController : Controller {
        [HttpPut("/slot/{id}/reserve")]
        public IActionResult Reserve(int id, [FromBody] ReserveRequest request) {
            if () {

            }
            return BadRequest("UserId is missing or invalid.");
        }
    }
}