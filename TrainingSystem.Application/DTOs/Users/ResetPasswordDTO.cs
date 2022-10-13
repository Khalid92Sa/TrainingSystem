using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Users
{
    public class ResetPasswordDTO
    {
        [Required]
        public string UserName { get; set; }
        public string code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&-]).{8,}$",
         ErrorMessage = "Passwords must be at least 8 characters." +
            "\r\nPasswords must have at least one non alphanumeric character." +
            "\r\nPasswords must have at least one digit ('0'-'9')." +
            "\r\nPasswords must have at least one uppercase ('A'-'Z')." +
            "\r\nPasswords must have at least one lowercase ('a'-'z').")]
        public string Password { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

    }
}
