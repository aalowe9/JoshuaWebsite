using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace joshuaSite.Models
{
    public class LoginModel
    {
        [Required (ErrorMessage = "Username is required")]
        [DisplayName("User Name")]
        
        public String UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("User Name")]
        public String Password { get; set; }
    }
}