using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ProjectsRepository _projectsRepository;

        public ProjectsController(IMapper mapper, ProjectsRepository projectsRepository)
        {
            _mapper = mapper;
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<ProjectView> projects;
            if (User.IsInRole("Employee"))
            {
                var projectsDb = await _projectsRepository.
                    GetAllByEmployeeIdIncludePMAsync(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                projects = _mapper.Map<List<ProjectView>>(projectsDb);
                return View(projects);
            }
            return View();
        }
    }
}
