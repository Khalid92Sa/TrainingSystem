using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Users
{
   public class ForgotPasswordDTo
    {
       
        [Required]
        public string UserName { get; set; }
        
       
    }
}
