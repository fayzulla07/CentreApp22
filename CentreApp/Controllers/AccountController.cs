using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CentreApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;

namespace CentreApp.Controllers
{
    public class AccountController : Controller
    {
        ISqlData data;
        public AccountController(ISqlData data)
        {
            this.data = data;
        }
        public IActionResult Login()
        {
            
            return View(new Users());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Users model)
        {
            if(model.LoginName == null || model.Password == null)
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                return View("Login", model);
            }
            if (ModelState.IsValid)
            {
                Users user = data.SqlQuery<Users>("select * from [Users] where [LoginName] = @lname and [Password] = @pname", new { lname = model.LoginName, pname = model.Password  }).FirstOrDefault();
                if (user != null)
                {
                    user.roles = data.GetById<Roles>((int)user.RoleId);
                    await Authenticate(user); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View("Login", model);

        }
        private async Task Authenticate(Users user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.roles.Name),
                new Claim("LoginName", user.LoginName),
                new Claim("UserId", user.Id.ToString()),
                new Claim("RoleDesc", user.Description == null ? "" : user.Description)

            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}