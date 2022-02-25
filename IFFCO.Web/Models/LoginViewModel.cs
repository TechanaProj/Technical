using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class LoginViewModel
    {
        //public LoginModel loginModel { get; set; }
        public int PersonalNo { get; set; }
        public string Password { get; set; }        
        public string ModuleId { get; set; }
    }
}
