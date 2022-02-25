using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Shared.Entities;

namespace IFFCO.TECHPROD.Web.ViewModels
{
    public class TECHSCR22ViewModel : BaseModel
    {
        public string UCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<SelectListItem> UnitLOV { get; set; }
        public string ReportType { get; set; }
        public string CallingReport { get; set; }
        public Report ReportObj { get; set; }
        public string SelectedReportFormat { get; set; }
        public string Report { get; set; }
        public string Date { get; set; }
        public string PlantType { get; set; }
        public TECHSCR22ViewModel()
        {
            ToDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            FromDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
        }
    }
}
