using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly LeaveRequestsRepository _leaveRequestsRepository;
        private readonly EmployeesRepository _employeesRepository;
        private readonly ApprovalRequestsRepository _approvalRequestsRepository;
        private readonly LeaveRequestDateValidator _dateValidator;
        public LeaveRequestsController(
            IMapper mapper,
            LeaveRequestsRepository leaveRequestsRepository,
            EmployeesRepository employeesRepository,
            ApprovalRequestsRepository approvalRequestsRepository,
            LeaveRequestDateValidator dateValidator)
        {
            _leaveRequestsRepository = leaveRequestsRepository;
            _mapper = mapper;
            _dateValidator = dateValidator;
            _employeesRepository = employeesRepository;
            _approvalRequestsRepository = approvalRequestsRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<LeaveRequestView> leaveRequests;
            if (User.IsInRole("Employee"))
            {
                var leaveRequestsDb = await _leaveRequestsRepository
                    .GetAllByEmployeeId(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                leaveRequests = _mapper.Map<List<LeaveRequestView>>(leaveRequestsDb);
                return View(leaveRequests);
            }
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var requestDb = await _leaveRequestsRepository.GetByIdOrDefaultAsync(id);
            if (requestDb == null)
            {
                throw new ArgumentException("No leave request with such id was found");
            }
            var request = _mapper.Map<LeaveRequestView>(requestDb);
            return View(request);
        }

        [HttpPatch]
        public async Task<IActionResult> Edit(LeaveRequestView model)
        {
            var validResult = _dateValidator.Validate(model);
            if (ModelState.IsValid && validResult.IsValid)
            {
                if (model.AbsenceReason == AbsenceReason.OtherInComment && model.Comment.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Comment", "You have to leave a comment if you choose \"other option in comment\" as absence reason");
                    return View(model);
                }
                var leaveRequestDb = _mapper.Map<LeaveRequest>(model);
                leaveRequestDb.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _leaveRequestsRepository.UpdateAsync(leaveRequestDb);
                return View("Index");
            }
            else
            {
                foreach (var error in validResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestView model)
        {
            var validResult = _dateValidator.Validate(model);
            if (ModelState.IsValid && validResult.IsValid)
            {
                if(model.AbsenceReason == AbsenceReason.OtherInComment && model.Comment.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Comment", "You have to leave a comment if you choose \"other option in comment\" as absence reason");
                    return View(model);
                }
                var leaveRequestDb = _mapper.Map<LeaveRequest>(model);
                leaveRequestDb.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _leaveRequestsRepository.AddAsync(leaveRequestDb);
                return View("Index");
            }
            else
            {
                foreach (var error in validResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Submit(int id)
        {
            var requestDb = await _leaveRequestsRepository.GetByIdOrDefaultAsync(id);
            if (requestDb == null)
            {
                throw new ArgumentException("No leave request with such id was found");
            }
            if (requestDb.Status == Status.Submit || requestDb.Status == Status.Cancel)
            {
                return BadRequest("Status can't submit when it's already submitted or canceled");
            }

            var employee = await _employeesRepository.GetByIdIncludeHRAndProjectsWithManagersAsync(requestDb.EmployeeId);
            var projectManagers = employee.Projects
                .Select(project => project.ProjectManager)
                .Distinct()
                .ToList();
            var hrManager = employee.PeoplePartner;
            List<ApprovalRequest> approvals = new List<ApprovalRequest>();
            foreach (var projectManager in projectManagers)
            {
                approvals.Add(
                    new ApprovalRequest
                    {
                        ApproverId = projectManager.ID,
                        LeaveRequestId = id,
                        Status = Status.New

                    });
            }
            approvals.Add(
                new ApprovalRequest
                {
                    ApproverId = hrManager.ID,
                    LeaveRequestId = id,
                    Status = Status.New
                });
            await _approvalRequestsRepository.AddListAsTransactionAsync(approvals);
            requestDb.Status = Status.Submit;
            await _leaveRequestsRepository.UpdateAsync(requestDb);

            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Cancel(int id)
        {
            var requestDb = await _leaveRequestsRepository.GetByIdOrDefaultAsync(id);
            if (requestDb == null)
            {
                throw new ArgumentException("No leave request with such id was found");
            }
            if(requestDb.Status == Status.Cancel)
            {
                return BadRequest("It's already canceled");
            }
            await _approvalRequestsRepository.DeleteByLeaveRequestId(id);
            requestDb.Status = Status.Cancel;
            await _leaveRequestsRepository.UpdateAsync(requestDb);
            return Ok();
        }
    }
}
