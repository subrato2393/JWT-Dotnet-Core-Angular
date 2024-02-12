using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoSecurityApp.EntityModel
{
    public class LoginViewModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
