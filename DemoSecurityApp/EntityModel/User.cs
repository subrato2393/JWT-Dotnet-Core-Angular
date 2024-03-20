using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoSecurityApp.EntityModel
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? PhoneNo { get; set; }

        [Required]
        public string? Password { get; set; }

       // [Required]
        public string? Role { get; set; }

       /// <summary>
       /// [Required]
       /// </summary>
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public  DateTime? RefreshTokenExpiryTime { get; set; }         
    } 
}
