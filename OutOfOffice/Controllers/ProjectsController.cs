using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
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
            List<Project> projectsDb;
            if (User.IsInRole("Employee"))
            {
                projectsDb = (await _projectsRepository
                    .GetAllByEmployeeIdIncludePMAsync(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)))
                    .ToList();

            }
            else if (User.IsInRole("HRManager"))
            {
                var hrId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                projectsDb = (await _projectsRepository
                    .GetAllProjectsOfSubordinateEmployeesByHRIdIncludePMAsync(hrId))
                    .ToList();
            }
            else
            {
                projectsDb = new List<Project>();
            }
            projects = _mapper.Map<List<ProjectView>>(projectsDb);
            return View(projects);
        }
        public async Task<IActionResult> Certain([FromQuery] int id)
        {
            var projectDb = await _projectsRepository.GetByIdIncludeEmployeesAndPMOrDefaultAsync(id);
            if(projectDb == null)
            {
                throw new ArgumentException("No project with such id was found");
            }
            var projectView = _mapper.Map<ProjectView>(projectDb);
            return View(projectView);
        }
    }
}
