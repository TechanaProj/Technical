using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class ADMSC03ViewModel:BaseModel
    {
        public List<EmpProgramAccess> ListObj { get; set; }

        public EmpProgramAccess Obj { get; set; }

        public List<AdmEmpUnitAccess> ObjDetail { get; set; }

        public List<PlantTableForBind> ObjPlantDetail { get; set; }

        public List<SelectListItem> UnitLOV { get; set; }

        public List<SelectListItem> ModuleLOV { get; set; }

        public string SelectedModule { get; set; }

        public string SelectedProgram { get; set; }

        public bool  AN0Access { get; set; }

        public bool PH0Access { get; set; }

        public bool KD0Access { get; set; }

        public bool KL0Access { get; set; }

        public bool PD0Access { get; set; }

        public string UnitCD { get; set; }

        public string PersonnelNumber { get; set; }



    }

    public class EmpProgramAccess
    {
        public string PersonnelNo { get; set; }

        public string Name { get; set; }

        public string ModuleID { get; set; }

        public string ModuleName { get; set; }

        public string ProgramID { get; set; }

        public string ProgramName { get; set; }

        public string SelectAccess { get; set; }

        public string InsertAccess { get; set; }

        public string UpdateAccess { get; set; }

        public string DeleteAccess { get; set; }

        public int UnitCode { get; set; }
    }
}
