using BR_Architects.Models;
using BR_Architects.Models.Data;
using BR_Architects.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BR_Architects.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserData _userDAL;

        public AccountController(UserData userDAL)
        {
            _userDAL = userDAL;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userDAL.GetUserByEmail(model.Email); // -> Lấy người dùng có email tương ứng

                if (user != null && user.Pass == model.Password)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.HoTen),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("Email", user.Email)
                    }; // -> Thêm các claim vào danh sách

                    if (user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    }; // -> Thiết lập thuộc tính xác thực

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties); // -> Đăng nhập

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
            }

            // Trả về JSON nếu là AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Email hoặc mật khẩu không đúng" });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_userDAL.UserExistsByEmail(model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");

                    // Trả về JSON nếu là AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Email này đã được sử dụng" });
                    }

                    return View(model);
                } // -> Kiểm tra xem email đã tồn tại chưa

                var user = new User
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    Pass = model.Password,
                    SecretKey = Guid.NewGuid().ToString().Substring(0, 20)
                };

                int userId = _userDAL.AddUser(user);
                user.Id = userId;

                // Tự động đăng nhập sau khi đăng ký
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.HoTen),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Email", user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // Trả về JSON nếu là AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("Index", "Home");
            }

            // Trả về JSON nếu là AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return Json(new { success = false, errors = errors });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CheckEmailExists(string email)
        {
            var exists = _userDAL.UserExistsByEmail(email);
            return Json(!exists);
        }
    }
}