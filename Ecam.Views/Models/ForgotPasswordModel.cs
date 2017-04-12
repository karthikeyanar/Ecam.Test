using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecam.Views.Models {
    public class ForgotPasswordModel {
        [Required(ErrorMessage = "Token is required")]
        public string token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50,MinimumLength = 6,ErrorMessage = "Password Minimum 6 characters required.")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}