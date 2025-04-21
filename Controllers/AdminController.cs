// Controllers/AdminController.cs
using BR_Architects.Models;
using BR_Architects.Models.Data;
using BR_Architects.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BR_Architects.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProjectData _projectDAL;
        private readonly TeamMemberData _teamMemberDAL;
        private readonly UserData _userDAL;
        private readonly ContactData _contactDAL;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminController(
        ProjectData projectDAL,
            TeamMemberData teamMemberDAL,
        UserData userDAL,
            ContactData contactDAL,
            IWebHostEnvironment hostEnvironment)
        {
            _projectDAL = projectDAL;
            _teamMemberDAL = teamMemberDAL;
            _userDAL = userDAL;
            _contactDAL = contactDAL;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Dashboard()
        {
            ViewBag.ProjectCount = _projectDAL.GetAllProjects().Count;
            ViewBag.TeamMemberCount = _teamMemberDAL.GetAllTeamMembers().Count;
            ViewBag.UserCount = _userDAL.GetAllUsers().Count;
            ViewBag.ContactCount = _contactDAL.GetAllContacts().Count;

            return View();
        }


        //Quản lý dự án
        public IActionResult ManageProjects()
        {
            var projects = _projectDAL.GetAllProjects();
            return View(projects);
        } // Quản lý dự án

        [HttpGet]
        public IActionResult GetProjectById(int id)
        {
            var project = _projectDAL.GetProjectById(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dự án" });
            }

            return Json(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(Project project, IFormFile imageFile)
        {
                try
                {
                    // Ảnh
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(imageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            return Json(new { success = false, message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)." });
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        project.LinkAnh = "images/" + fileName;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Vui lòng chọn hình ảnh cho dự án." });
                    }

                    _projectDAL.AddProject(project);
                    return Json(new { success = true, message = "Thêm dự án thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(Project project, IFormFile imageFile)
        {
                try
                {
                    var existingProject = _projectDAL.GetProjectById(project.Id);
                    if (existingProject == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy dự án." });
                    }

                    // Lưu đường dẫn ảnh cũ
                    string oldImagePath = existingProject.LinkAnh;

                    // Cập nhật thông tin
                    existingProject.TenProject = project.TenProject;
                    existingProject.MoTaChiTiet = project.MoTaChiTiet;
                    existingProject.DiaDiem = project.DiaDiem;
                    existingProject.NamHoanThanh = project.NamHoanThanh;

                    // ảnh mới nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingProject.LinkAnh = "images/" + fileName;

                        // Xóa ảnh cũ nếu không phải ảnh mặc định
                        if (!string.IsNullOrEmpty(oldImagePath) && !oldImagePath.Contains("house"))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, oldImagePath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(project.LinkAnh))
                    {
                        // Giữ nguyên ảnh cũ
                        existingProject.LinkAnh = oldImagePath;
                    }

                    _projectDAL.UpdateProject(existingProject);
                    return Json(new { success = true, message = "Cập nhật dự án thành công." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProject(int id)
        {
            try
            {
                var project = _projectDAL.GetProjectById(id);
                if (project == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy dự án." });
                }

                // Xóa ảnh nếu không phải ảnh mặc định
                if (!string.IsNullOrEmpty(project.LinkAnh) && !project.LinkAnh.Contains("house"))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, project.LinkAnh);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _projectDAL.DeleteProject(id);
                return Json(new { success = true, message = "Xóa dự án thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        //Quản lý thành veien
        [HttpGet]
        public IActionResult ManageTeamMembers()
        {
            var teamMembers = _teamMemberDAL.GetAllTeamMembers();
            ViewBag.Users = _userDAL.GetAllUsers();
            return View(teamMembers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeamMember(TeamMember teamMember, IFormFile imageFile, string HoTen, string Email)
        {
            try
            {
                // Xử lý ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    teamMember.LinkAnh = "images/" + fileName;
                }
                else
                {
                    return Json(new { success = false, message = "Vui lòng chọn hình ảnh cho thành viên." });
                }

                // Tạo người dùng mới
                var user = new User
                {
                    HoTen = HoTen,
                    Email = Email,
                    Pass = Guid.NewGuid().ToString().Substring(0, 8), // Tạo mật khẩu mặc định
                    SecretKey = Guid.NewGuid().ToString().Substring(0, 20)
                };

                // Kiểm tra email đã tồn tại chưa
                if (_userDAL.UserExistsByEmail(user.Email))
                {
                    return Json(new { success = false, message = "Email này đã được sử dụng." });
                }

                // Thêm người dùng mới vào bảng Users
                int userId = _userDAL.AddUser(user);
                user.Id = userId;

                // Gán UserId cho TeamMember
                teamMember.UserId = userId;

                _teamMemberDAL.AddTeamMember(teamMember);
                return Json(new { success = true, message = "Thêm thành viên thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeamMember(TeamMember teamMember, IFormFile imageFile)
        {
                try
                {
                    var existingMember = _teamMemberDAL.GetTeamMemberById(teamMember.Id);
                Console.WriteLine(teamMember.User.HoTen);
                Console.WriteLine(teamMember.User.Email);
                if (existingMember == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy thành viên." });
                    }

                    // Lưu đường dẫn ảnh cũ
                    string oldImagePath = existingMember.LinkAnh;

                    // Cập nhật thông tin
                    existingMember.UserId = teamMember.UserId;
                    existingMember.ChucVu = teamMember.ChucVu;
                    existingMember.GioiThieu = teamMember.GioiThieu;
                    existingMember.User.HoTen = teamMember.User.HoTen;
                    existingMember.User.Email = teamMember.User.Email;


                // Xử lý file ảnh mới nếu có
                if (imageFile != null && imageFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingMember.LinkAnh = "images/" + fileName;

                        // Xóa ảnh cũ nếu không phải ảnh mặc định
                        if (!string.IsNullOrEmpty(oldImagePath) && !oldImagePath.Contains("avatar"))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, oldImagePath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(teamMember.LinkAnh))
                    {
                        // Giữ nguyên ảnh cũ
                        existingMember.LinkAnh = oldImagePath;
                    }
                

                _teamMemberDAL.UpdateTeamMember(existingMember);
                _userDAL.UpdateUser(existingMember.User);
                return Json(new { success = true, message = "Cập nhật thành viên thành công." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Json(new { success = false, message = "Lỗi: " + ex.Message });
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeamMember(int id)
        {
            try
            {
                var teamMember = _teamMemberDAL.GetTeamMemberById(id);
                if (teamMember == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thành viên." });
                }

                // Xóa file ảnh nếu không phải ảnh mặc định
                if (!string.IsNullOrEmpty(teamMember.LinkAnh) && !teamMember.LinkAnh.Contains("avatar"))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, teamMember.LinkAnh);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _teamMemberDAL.DeleteTeamMember(id);
                return Json(new { success = true, message = "Xóa thành viên thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetTeamMemberById(int id)
        {
            var teamMember = _teamMemberDAL.GetTeamMemberById(id);
            if (teamMember == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thành viên" });
            }

            return Json(teamMember);
        }

       //Quản lý liên hệ
        public IActionResult ManageContacts()
        {
            var contacts = _contactDAL.GetAllContacts();
            return View(contacts);
        }

        public IActionResult ContactDetails(int id)
        {
            var contact = _contactDAL.GetContactById(id);
            if (contact == null)
            {
                return NotFound();
            }
            
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteContact(int id)
        {
            _contactDAL.DeleteContact(id);
            return RedirectToAction(nameof(ManageContacts));
        }

    }
}