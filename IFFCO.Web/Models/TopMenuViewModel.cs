using IFFCO.HRMS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class TopMenuViewModel
    {
        public string UserName { get; set; }
        public string Designation { get; set; }
        public string Image { get; set; }
        public string PersonalNo { get; set; }
        public int UnitCode { get; set; }
        public string Unit { get; set; }
        public int WorkUnit { get; set; }
        public string WorkDepartment { get; set; }
        public string WorkSection { get; set; }
    }
}
