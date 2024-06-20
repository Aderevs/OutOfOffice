﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly LeaveRequestsRepository _repository;
        private readonly IMapper _mapper;

        public LeaveRequestsController(LeaveRequestsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<LeaveRequestView> leaveRequests;
            if (User.IsInRole("Employee"))
            {
                var leaveRequestsDb = await _repository
                    .GetAllByEmployeeId(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                leaveRequests = _mapper.Map<List<LeaveRequestView>>(leaveRequestsDb);
                return View(leaveRequests);
            }
            return View();
        }
    }
}