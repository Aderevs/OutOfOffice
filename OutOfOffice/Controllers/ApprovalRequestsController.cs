using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.DbLogic.Repositories.Interfaces;
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
        private readonly IApprovalRequestsRepository _approvalRequestsRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public ApprovalRequestsController(
            ILogger<ApprovalRequestsController> logger,
            IMapper mapper,
            IApprovalRequestsRepository approvalRequestsRepository,
            IEmployeesRepository employeesRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _approvalRequestsRepository = approvalRequestsRepository;
            _employeesRepository = employeesRepository;
        }

        [Authorize(Roles = "HRManager,ProjectManager")]
        public async Task<IActionResult> Index()
        {
            var approverId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var requestsDb = await _approvalRequestsRepository.GetAllOfApproverByIdAsync(approverId);
            var requestsView = _mapper.Map<List<ApprovalRequestView>>(requestsDb);
            return View(requestsView);
        }

        public async Task<IActionResult> MyLeaveRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var requestsDb = await _approvalRequestsRepository.GetAllForLeaveRequestsOfEmployeeByIdIncludeApproverAsync(userId);
            var requestsView = _mapper.Map<List<ApprovalRequestView>>(requestsDb);
            return View(requestsView);
        }
        public async Task<IActionResult> ApprovalToMyLeaveRequest([FromQuery] int id)
        {
            var requestDb = await _approvalRequestsRepository.GetByIdIncludeLeaveAndApproverAsync(id);
            if (requestDb is not null)
            {
                var requestView = _mapper.Map<ApprovalRequestView>(requestDb);
                return View(requestView);
            }
            throw new ArgumentException("No approval request with such id was found");
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
        [Authorize(Roles = "HRManager,ProjectManager")]
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

            var otherApprovals = await _approvalRequestsRepository.GetAllByLeaveIdIncludeLeaveAsync(requestDb.LeaveRequestId);
            otherApprovals = otherApprovals.Where(request => request.ID != model.ID).ToList();
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!model.Comment.IsNullOrEmpty())
                    {
                        requestDb.Comment = model.Comment;
                    }
                    requestDb.Status = DbLogic.ApprovalRequestStatus.Approved;
                    if (!otherApprovals.Any(request => request.Status != DbLogic.ApprovalRequestStatus.Approved))
                    {
                        var leaveDuration = (requestDb.LeaveRequest.EndDate.ToDateTime(new TimeOnly()) - requestDb.LeaveRequest.StartDate.ToDateTime(new TimeOnly())).Days;
                        employeeDb.OutOfOfficeBalance -= leaveDuration;
                        await _employeesRepository.UpdateAsync(employeeDb);
                    }

                    await _approvalRequestsRepository.UpdateAsync(requestDb);
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
        [Authorize(Roles = "HRManager,ProjectManager")]
        public async Task<IActionResult> Approve([FromQuery] int id)
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

            var otherApprovals = await _approvalRequestsRepository.GetAllByLeaveIdIncludeLeaveAsync(requestDb.LeaveRequestId);
            otherApprovals = otherApprovals.Where(request => request.ID != id).ToList();
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    requestDb.Status = DbLogic.ApprovalRequestStatus.Approved;
                    await _approvalRequestsRepository.UpdateAsync(requestDb);
                    if (!otherApprovals.Any(request => request.Status != DbLogic.ApprovalRequestStatus.Approved))
                    {
                        var leaveDuration = (requestDb.LeaveRequest.EndDate.ToDateTime(new TimeOnly()) - requestDb.LeaveRequest.StartDate.ToDateTime(new TimeOnly())).Days;
                        employeeDb.OutOfOfficeBalance -= leaveDuration;
                        await _employeesRepository.UpdateAsync(employeeDb);
                    }
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
        [Authorize(Roles = "HRManager,ProjectManager")]
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
            requestDb.Status = DbLogic.ApprovalRequestStatus.Rejected;
            requestDb.Comment = model.Comment;
            await _approvalRequestsRepository.UpdateAsync(requestDb);
            return RedirectToAction("Index");
        }
    }
}
