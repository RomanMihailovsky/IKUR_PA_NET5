using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IKUR_PA_NET5.ViewModels
{
    public class RegisterViewModel
    {

        private string phonenumber;

        [Required(ErrorMessage = "не указан") ]
        //[StringLength(10, ErrorMessage = "указан не полностью", MinimumLength = 10)]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber
        {
            get 
            { 
                return phonenumber; 
            }
            set 
            {
                if (value != null)
                {
                    phonenumber = value.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }
            }
        }

        [Required(ErrorMessage = "не заполнено")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "не заполнено")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "не заполнено")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        //[Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        [Display(Name = "Дата рождения")]
        public int BirthDay { get; set; }


        [Required(ErrorMessage = "не указан")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "не указан")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]

        public string PasswordConfirm { get; set; }

    }
}
