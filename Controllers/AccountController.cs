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
        [ValidateAntiForgeryToken]
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

                    // ============================================================================================================
                    //return RedirectToAction("Index", "Home");
                    // ============================================================================================================

                    //ConfirmPhoneNumberViewModel confirmphone_model = new ConfirmPhoneNumberViewModel()
                    //{
                    //    PhoneNumber = model.PhoneNumber
                    //};

                    return RedirectToAction("ConfirmPhoneNumber", "Account", new { model.PhoneNumber });

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

        [HttpGet]
        public IActionResult LoginSms(string returnUrl = null)
        {
            return View(new LoginSmsViewModel { ReturnUrl = returnUrl });
        }


        [HttpGet]
        public IActionResult ConfirmPhoneNumber(string phonenumber, string returnUrl = null)
        {
            return View(new ConfirmPhoneNumberViewModel { ReturnUrl = returnUrl, PhoneNumber = phonenumber });            
        }


        // ----------------------------- ConfirmPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberViewModel model)
        {

            // @@@@@@@@@ ПРОВЕРКА СОВПАДЕНИЯ ОТПРАВЛЕННОГО КОДА с model.CodeSMS;
            // @@@@@@@@@ ПОСМОТРЕТь DNS как себя ведет при неверно указанном Коде SMS

            if (ModelState.IsValid)
            {
                string phonenumber = model.PhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                User user = new User
                {
                    UserName = phonenumber,  // Номер телефона Вместо UserName 
                    PhoneNumber = phonenumber,
                    PhoneNumberConfirmed = true // @@@@@@@@@@  PhoneNumberConfirmed = true
                };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user);  // @@@@@@@@@@ USER БЕЗ ПАРОЛЯ
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);

                    // @@@@@@@@@@@ ЕСЛИ ПУСТЫЕ ЗНАЧЕНИЯ ФИО
                    // @@@@@@@@@@@ ПЕРЕХОД К ЗАПОЛНЕНИЮ ФИО

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

        // ----------------------------- LoginSms
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginSms(LoginSmsViewModel model)
        {

            if (ModelState.IsValid)
            {
                // проверяем, принадлежит ли URL приложению
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    //// установка куки
                    //await _signInManager.SignInAsync(user, false);

                    //ConfirmPhoneNumberViewModel confirmphone_model   = new ConfirmPhoneNumberViewModel() 
                    //{ 
                    //    PhoneNumber = model.PhoneNumber  
                    //};

                    return RedirectToAction("ConfirmPhoneNumber", "Account", new { model.PhoneNumber });
                }
               
            }

            return View(model);

        }


        // ----------------------------- Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                string phonenumber = model.PhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

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
                    ModelState.AddModelError("", "Телефон или пароль указаны неверно");
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
