using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHSCR22Controller : BaseController<TECHSCR22ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR22Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            CommonViewModel.ToDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.PlantLOVList = PlantLOVBind();
            return View(CommonViewModel);
        }

        public SelectList PlantLOVBind()
        {
            var dictionary = new Dictionary<string, string>
            {
               {"Aonla1", "A1"},
               {"Aonla2", "A2"},
            };

            var selectList = new SelectList(dictionary, "Value", "Key");

            return selectList;
        }

        public ActionResult GenerateReport(TECHSCR22ViewModel tECHSCR22ViewModel)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(tECHSCR22ViewModel);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR22ViewModel.SelectedReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }

       
        public Report GenerateReportData(TECHSCR22ViewModel tECHSCR22ViewModel)
        {
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = tECHSCR22ViewModel.CallingReport;
            tECHSCR22ViewModel.SelectedReportFormat = "PDF";
            ReportData.ReportName = "DESP_TECH_ST_VAR.rep";
            ReportData.Query = "F_dt=" + Convert.ToDateTime(tECHSCR22ViewModel.FromDate).ToString("dd/MMM/yyyy") + "+" +
                               "T_dt=" + Convert.ToDateTime(tECHSCR22ViewModel.ToDate).ToString("dd/MMM/yyyy") + "+" +
                               "P_UNIT=" + tECHSCR22ViewModel.PlantType;  
            return ReportData;
        }
    }
}
