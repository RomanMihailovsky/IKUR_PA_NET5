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

        //============================================================================================================
        //                            Register. Регистрация.
        //============================================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { 
                    UserName = model.PhoneNumber,  // Номер телефона Вместо UserName 
                    PhoneNumber = model.PhoneNumber,
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

                    // -----------------------------------------------------------------------------------
                    //return RedirectToAction("Index", "Home");
                    // -----------------------------------------------------------------------------------

                    // -- Переход к "Подтверждению номера телефона", т.к. user новый
                    return RedirectToAction("ConfirmPhoneNumber", "Account", new { model.PhoneNumber });

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError(string.Empty, $"Пользователь с номер {model.PhoneNumber} уже существует");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        
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
            //  TODO  SMSSender.Send. Отправка SMS  @@@@@@@@@

            return View(new ConfirmPhoneNumberViewModel { ReturnUrl = returnUrl, PhoneNumber = phonenumber });
        }

        //[HttpGet]
        // В model значения null даже если передаешь модель
        //public IActionResult ConfirmPhoneNumber(ConfirmPhoneNumberViewModel model, string returnUrl = null)
        //{  
        //    model.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //============================================================================================================
        //           ConfirmPhoneNumber. Подтверждение номера тел. по SMS
        //============================================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (model.CodeSMS == "5555")
                {
                    User user = new User
                    {
                        UserName = model.PhoneNumber,  // Номер телефона Вместо UserName 
                        PhoneNumber = model.PhoneNumber,
                        PhoneNumberConfirmed = true // @@@@@@ PhoneNumberConfirmed = true
                    };

                    // TODO Проверить сущ. user в базе. Если нет, добавить.

                    // добавление user
                    // если user сущ. // Username '+79127674499' is already taken.
                    var result = await _userManager.CreateAsync(user);  // TODO USER БЕЗ ПАРОЛЯ ??? @@@@@@

                    // TODO Если user существует в базе. Проверить если Empty SurName. ПЕРЕХОД к Register

                    if (result.Succeeded)
                    {
                        // установка куки 
                        //await _signInManager.SignInAsync(user, false);

                        // TODO ЕСЛИ Empty SurName ПЕРЕХОД К Register с передачей model

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
                else
                {
                    ModelState.AddModelError("", "Введен неверный код");
                }
                 
            }

            return View(model);
        }

        //============================================================================================================
        //                    LoginSms. Вход по коду SMS
        //============================================================================================================
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
                    //ConfirmPhoneNumberViewModel model_confirm = new ConfirmPhoneNumberViewModel() { PhoneNumber = model.PhoneNumber };

                    // -- Переход к "Подтверждению номера телефона"
                    return RedirectToAction("ConfirmPhoneNumber", "Account", new { model.PhoneNumber } );
                }
               
            }

            return View(model);

        }

        //============================================================================================================
        //                    Login. Вход с паролем
        //============================================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result =
                    await _signInManager.PasswordSignInAsync(model.PhoneNumber, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        // TODO Возможно Проверка PhoneNumberConfirmed = true  @@@@@@@@

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
