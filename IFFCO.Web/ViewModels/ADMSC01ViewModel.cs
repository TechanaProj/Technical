using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class ADMSC01ViewModel:BaseModel
    {
        public List<AdmEmpprgAccess> ListObj { get; set; }

        public List<SelectListItem> UnitLOV { get; set; }

        public List<SelectListItem> EmpLOV { get; set; }

        public List<SelectListItem> ModuleLOV { get; set; }

        public string EmployeeName { get; set; }

        public string UnitCD { get; set; }

        public string Designation { get; set; }

        public string Grade { get; set; }

        public string Unit { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public int PersonnelNumber { get; set; }

        public string ModuleID { get; set; }

        public string ProgramType { get; set; }

        public string QueryType { get; set; }

    }

    
}
