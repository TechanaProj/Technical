﻿using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class TECHSCR01AViewModel: BaseModel
    {
        public string UCode { get; set; }

        public DateTime ReportDate { get; set; }
        public DateTime? FromDate { get; set; }


        public DateTime? ToDate { get; set; }
        public List<SelectListItem> UnitLOV { get; set; }

        public string ReportType { get; set; }

        public string CallingReport { get; set; }
        public Report ReportObj { get; set; }
        public string SelectedReportFormat { get; set; }
        public string Report { get; set; }

        public string SelectedSuggestionNo { get; set; }
    }
}
