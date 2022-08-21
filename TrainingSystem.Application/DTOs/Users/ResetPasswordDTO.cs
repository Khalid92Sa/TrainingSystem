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
        [Required]
        public string code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

    }
}
