using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IKUR_PA_NET5.Models
{
    // Класс проверки пароля
    public class CustomPasswordValidator : IPasswordValidator<User>
    {

        public int RequiredLength { get; set; }

        public CustomPasswordValidator(int length)
        {
            RequiredLength = length;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (String.IsNullOrEmpty(password) || password.Length < RequiredLength)
            {
                errors.Add(new IdentityError
                {
                    Description = $"Минимальная длина пароля равна {RequiredLength}"
                });
            }
            //string pattern = "^[0-9]+$";
            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$"; // Строчные и прописные латинские буквы, цифры
            //string pattern = @"^(?=.*\d)(?=.*[a-z])(?!.*\s).*$"; // Строчные латинские буквы, цифры

            //string pattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*0-9]{10,}$"; // Строчные и прописные латинские буквы, цифры и специальные символы 

            if (!Regex.IsMatch(password, pattern))
            {
                errors.Add(new IdentityError
                {
                    Description = "В пароле должны содержаться строчные и прописные латинские буквы, цифры"
                });
            }
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }


    }
}
