using BR_Architects.Models;
using BR_Architects.Models;
using BR_Architects.Models.Data;
using BR_Architects.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using BR_Architects.Services;

namespace BR_Architects.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectData _projectDAL;
        private readonly TeamMemberData _teamMemberDAL;
        private readonly ContactData _contactDAL;
        private readonly OtpService _otpService;
        public HomeController(ProjectData projectDAL, TeamMemberData teamMemberDAL, ContactData contactDAL, OtpService otpService)
        {
            _projectDAL = projectDAL;
            _teamMemberDAL = teamMemberDAL;
            _contactDAL = contactDAL;
            _otpService = otpService;
        }

        public IActionResult Index()
        {
            var projects = _projectDAL.GetAllProjects();
            var teamMembers = _teamMemberDAL.GetAllTeamMembers();

            ViewBag.Projects = projects;
            ViewBag.TeamMembers = teamMembers;

            return View();
        }

        public IActionResult Contact()
        {
            // Kiểm tra nếu người dùng đã đăng nhập, tự động điền email
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                var contactViewModel = new ContactViewModel { Email = email };
                return View(contactViewModel);
            }

            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    NguoiGui = model.NguoiGui,
                    Email = model.Email,
                    TieuDe = model.TieuDe,
                    NoiDung = model.NoiDung,
                    DaDoc = false
                };

                _contactDAL.AddContact(contact);

                TempData["SuccessMessage"] = "Tin nhắn của bạn đã được gửi thành công!";
                return RedirectToAction("Index", "Home");
                
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult GetProjectDetails(int id)
        {
            var project = _projectDAL.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return Json(project);
        }

        [HttpGet]
        public IActionResult GetTeamMemberDetails(int id)
        {
            var teamMember = _teamMemberDAL.GetTeamMemberById(id);
            if (teamMember == null)
            {
                return NotFound();
            }

            var result = new
            {
                id = teamMember.Id,
                name = teamMember.User.HoTen,
                position = teamMember.ChucVu,
                bio = teamMember.GioiThieu,
                image = teamMember.LinkAnh,
                email = teamMember.User.Email
            };

            return Json(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    
}