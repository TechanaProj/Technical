using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class ADMM01ViewModel:BaseModel
    {
        public List<AdmProjmodMaster> List { get; set; }

        public AdmProjmodMaster Msts { get; set; }

        public AdmPrgMaster MstsItem { get; set; }

        public List<AdmPrgMaster> ListItem { get; set; }

        public string SelectedMod { get; set; }

        public AdmSubMenuMsts SubMsts { get; set; }

        public List<AdmSubMenuMsts> SubList { get; set; }

        public List<SelectListItem> SubLOV { get; set; }

        public string SelectedSubModule { get; set; }

        public string PrevSubMenuID { get; set; }



    }

    
}
