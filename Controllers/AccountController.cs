using IKUR_PA_NET5.Models;
using IKUR_PA_NET5.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IKUR_PA_NET5.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        // ----------------------------- Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string phonenumber = model.PhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                User user = new User { 
                    UserName = phonenumber,  // Номер телефона Вместо UserName 
                    PhoneNumber = phonenumber,
                    Surname = model.Surname,
                    Name = model.Name,
                    MiddleName = model.MiddleName,
                    //Email = model.Email, 
                    //BirthDay = model.BirthDay 
                };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        // ----------------------------- Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                string phonenumber = model.PhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ","");


                //var user = await UserManager<User>.ChangePhoneNumberTokenPurpose FindAsync(phonenumber, model.Password);
                //if (user != null)
                //{
                //    if (user. == true)   // (if user.ConfirmedCodeSMS = true)
                //    {
                //        await SignInAsync(user, model.RememberMe);
                //        return RedirectToLocal(returnUrl);
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", "Не подтвержден email.");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Неверный Логин или Пароль.");
                //}


                var result =
                    await _signInManager.PasswordSignInAsync(phonenumber, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
