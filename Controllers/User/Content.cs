using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
public partial class UserController : Controller {
    public IActionResult SayHello() {
        return Content("hello");
    }
    public IActionResult FavoriteSlots(int UserId) {
        //JSON return
        return Content("FavoriteSlots");
    }
    public IActionResult ReservedSlots(int UserId) {
        //JSON return
        return Content("ReservedSlots");
    }
    public IActionResult AdministeredSlots(int UserId) {
        //JSON return
        return Content("AdministeredSlots");
    }
}