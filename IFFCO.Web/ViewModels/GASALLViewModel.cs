using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class GASALLViewModel:BaseModel
    {
        public string UCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Shift { get; set; }
        public string alert { get; set; }
    }
}
