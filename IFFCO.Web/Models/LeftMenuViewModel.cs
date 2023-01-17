using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class LeftMenuViewModel
    {
        public List<SelectListItem> Modules { get; set; }
        public List<SelectListItem> Units { get; set; }
        public List<SelectListItem> RepServ { get; set; } //RDLC Server Selection
        public List<ModulesMenu> ModulesMenu { get; set; }
        public List<ModulesMenu> FilterModulesMenu { get; set; }

        public string ModuleId { get; set; }
    }
}
