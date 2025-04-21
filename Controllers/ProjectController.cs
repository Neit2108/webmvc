using BR_Architects.Models;
using BR_Architects.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BR_Architects.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectData _projectDAL;

        public ProjectController(ProjectData projectDAL)
        {
            _projectDAL = projectDAL;
        }

        public IActionResult Index()
        {
            List<Project> projects = _projectDAL.GetAllProjects();
            return View(projects);
        }

        public IActionResult Details(int id)
        {
            Project project = _projectDAL.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpGet]
        public JsonResult GetProjectJson(int id)
        {
            Project project = _projectDAL.GetProjectById(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dự án" });
            }

            return Json(new { success = true, data = project });
        }
    }
}