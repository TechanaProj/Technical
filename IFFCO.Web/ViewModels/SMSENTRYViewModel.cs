using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class SMSENTRYViewModel : BaseModel
    {
        public string UCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Shift { get; set; }
        public string alert { get; set; }
        public string errorMessage { get; set; }
        public Employee employee { get; set; }
    }
}
