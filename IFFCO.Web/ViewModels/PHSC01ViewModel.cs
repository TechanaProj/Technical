using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class PHSC01ViewModel : BaseModel
    {
        public string UCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Shift { get; set; }
        public string alert { get; set; }


    }
}
