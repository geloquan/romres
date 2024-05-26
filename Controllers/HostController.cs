using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace WebApplication2.Controllers {
    public class HostController : Controller {
        [HttpGet("user/{user_id}/host/{host_id}")]
        public IActionResult Index(int user_id, int host_id) {
            Console.WriteLine("HostController - Index()");
            UserEntityLogin userEntityLogin = new UserEntityLogin();
            bool success = userEntityLogin.DirectLogin(user_id);
            UserEntityData userEntityData = new UserEntityData();
            userEntityData.HostedSlots(user_id);
            if (userEntityData.hostedSlots.SlotTrees[0].RootId != null && success) {
                ViewData["UserId"] = userEntityLogin.Id;
                ViewData["UserName"] = userEntityLogin.Name;
                ViewData["UserId"] = user_id;
                ViewData["HostId"] = host_id;
                var slotTreeJson = JsonConvert.SerializeObject(userEntityData.hostedSlots.SlotTrees);
                ViewBag.SlotTreeJson = slotTreeJson;
                JsonResult jsonResult =  new JsonResult(userEntityData.hostedSlots);
                return View("Index"); 
            } else {
                return View("Error");
            }
        }
        public IActionResult Id(int host_id) {
            return View();
        }
        public IActionResult Slot(int host_id, int slot_id) {
            return View();
        }
        [HttpPut("/host/newhost")]
        public IActionResult NewHost([FromBody] HttpPutNewHost newHostRequest) {
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
        [HttpPost("/host/add/child")]
        public IActionResult AddChild([FromBody] HttpPutAddChild deletionRequest) {
            if (deletionRequest != null) {
                bool processingResult = deletionRequest.Process();
                if (processingResult) {
                    return Ok("success AddChild");
                }
                else {
                    return StatusCode(500, "Failed to process AddChild.");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
        [HttpPatch("/host/edit/slot")]
        public IActionResult EditSlot([FromBody] HttpPatchEditSlot deletionRequest) {
            if (deletionRequest != null) {
                bool processingResult = deletionRequest.Process();
                if (processingResult) {
                    return Ok("success EditSlot");
                }
                else {
                    return StatusCode(500, "Failed to process EditSlot.");  // Return appropriate error status
                }
            } else {
                Console.WriteLine("Invalid request data()");
                return StatusCode(500, "Invalid request data.");
            }
        }
    }
}
