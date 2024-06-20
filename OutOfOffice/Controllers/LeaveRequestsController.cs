using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Models;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        public IActionResult Index()
        {
            List<LeaveRequestView> leaveRequests;
            if (User.IsInRole("Employee"))
            {

            }
            return View(leaveRequests);
        }
    }
}
