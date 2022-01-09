using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IKUR_PA_NET5.ViewModels
{
    public class ConfirmPhoneNumberViewModel
    {
        private string phonenumber;

        [Required(ErrorMessage = "не указан")]
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

        [Required(ErrorMessage = "не указан")]
        [Display(Name = "Код SMS")]
        public string CodeSMS { get; set; }

        public string ReturnUrl { get; set; }
    }
}
