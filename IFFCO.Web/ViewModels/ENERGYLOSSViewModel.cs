using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class ENERGYLOSSViewModel : BaseModel
    {
        public RefEnergy ObjRefEnergy { get; set; }
        public List<RefEnergy> ListRefEnergy { get; set; }

    }
}
