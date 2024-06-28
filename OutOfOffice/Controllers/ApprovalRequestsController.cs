using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;
using System.Transactions;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class ApprovalRequestsController : Controller
    {
        private readonly ILogger<ApprovalRequestsController> _logger;
        private readonly IMapper _mapper;
        private readonly ApprovalRequestsRepository _approvalRequestsRepository;
        private readonly EmployeesRepository _employeesRepository;

        public ApprovalRequestsController(
            ILogger<ApprovalRequestsController> logger,
            IMapper mapper,
            ApprovalRequestsRepository approvalRequestsRepository,
            EmployeesRepository employeesRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _approvalRequestsRepository = approvalRequestsRepository;
            _employeesRepository = employeesRepository;
        }

        public async Task<IActionResult> Index()
        {
            var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var requestsDb = await _approvalRequestsRepository.GetAllRequestsOfApproverByIdAsync(approverId);
            var requestsView = _mapper.Map<List<ApprovalRequestView>>(requestsDb);
            return View(requestsView);
        }

        public async Task<IActionResult> Certain([FromQuery] int id)
        {
            var requestDb = await _approvalRequestsRepository.GetByIdIncludeLeaveAndEmployeeAsync(id);
            if (requestDb is not null)
            {
                var requestView = _mapper.Map<ApprovalRequestView>(requestDb);
                return View(requestView);
            }
            throw new ArgumentException("No approval request with such id was found");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(ApprovalRequestView model)
        {
            var requestDb = await _approvalRequestsRepository.GetByIdIncludeLeaveOrDefaultAsync(model.ID);
            if (requestDb is null)
            {
                throw new ArgumentException("No approval request with such id was found");
            }
            var employeeDb = await _employeesRepository.GetByIdOrDefaultAsync(requestDb.LeaveRequest.EmployeeId);
            if (employeeDb is null)
            {
                throw new InvalidOperationException("No employee that has leave request with id such as in approval request was found");
            }
           
            using (var transaction = new TransactionScope())
            {
                try
                {
                    if (!model.Comment.IsNullOrEmpty())
                    {
                        requestDb.Comment = model.Comment;
                    }
                    requestDb.Status = DbLogic.Status.Submit;
                    var leaveDuration = (requestDb.LeaveRequest.EndDate.ToDateTime(new TimeOnly()) - requestDb.LeaveRequest.StartDate.ToDateTime(new TimeOnly())).Days;
                    employeeDb.OutOfOfficeBalance -= leaveDuration;
                   
                    await _approvalRequestsRepository.UpdateAsync(requestDb);
                    await _employeesRepository.UpdateAsync(employeeDb);
                    transaction.Complete();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    var problemDetails = new ProblemDetails
                    {
                        Status = 500,
                        Title = "Internal Server Error",
                        Detail = "Error while approving request. " + ex.Message
                    };
                    return StatusCode(500, problemDetails);
                }
            }
        }
        [HttpPatch]
        public async Task<IActionResult> Approve([FromQuery]int id)
        {
            var requestDb = await _approvalRequestsRepository.GetByIdIncludeLeaveOrDefaultAsync(id);
            if (requestDb is null)
            {
                throw new ArgumentException("No approval request with such id was found");
            }
            var employeeDb = await _employeesRepository.GetByIdOrDefaultAsync(requestDb.LeaveRequest.EmployeeId);
            if (employeeDb is null)
            {
                throw new InvalidOperationException("No employee that has leave request with id such as in approval request was found");
            }

            using (var transaction = new TransactionScope())
            {
                try
                {
                    requestDb.Status = DbLogic.Status.Submit;
                    var leaveDuration = (requestDb.LeaveRequest.EndDate.ToDateTime(new TimeOnly()) - requestDb.LeaveRequest.StartDate.ToDateTime(new TimeOnly())).Days;
                    employeeDb.OutOfOfficeBalance -= leaveDuration;

                    await _approvalRequestsRepository.UpdateAsync(requestDb);
                    await _employeesRepository.UpdateAsync(employeeDb);
                    transaction.Complete();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    var problemDetails = new ProblemDetails
                    {
                        Status = 500,
                        Title = "Internal Server Error",
                        Detail = "Error while approving request. " + ex.Message
                    };
                    return StatusCode(500, problemDetails);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Refuse(ApprovalRequestView model)
        {
            var requestDb = await _approvalRequestsRepository.GetByIdOrDefaultAsync(model.ID);
            if (requestDb is null)
            {
                throw new ArgumentException("No approval request with such id was found");
            }
            if (model.Comment.IsNullOrEmpty())
            {
                ModelState.AddModelError("Comment", "If you refuse request explain the reason in the comment");
                return View("Certain", model);
            }
            requestDb.Status = DbLogic.Status.Cancel;
            requestDb.Comment = model.Comment;
            await _approvalRequestsRepository.UpdateAsync(requestDb);
            return RedirectToAction("Index");
        }
    }
}
