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
    public class TECHSCR10Controller : BaseController<TECHSCR10ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR10Controller(ModelContext context)
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
            CommonViewModel.FromDate = new DateTime(1990, 04, 01).ToString("yyyy-MM-dd");
            ViewBag.ReportLOVList = ReportLOVBind();
            return View(CommonViewModel);
        }

        public SelectList ReportLOVBind()
        {
            var dictionary = new Dictionary<string, string>
            {
               {"Records", "RECORDS.rep"},
               {"Milestones", "MILESTONES.rep"},
            };

            var selectList = new SelectList(dictionary, "Value", "Key");

            return selectList;
        }

        public ActionResult GenerateReport(TECHSCR10ViewModel tECHSCR10ViewModel)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(tECHSCR10ViewModel);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR10ViewModel.SelectedReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }

       
        public Report GenerateReportData(TECHSCR10ViewModel tECHSCR10ViewModel)
        {
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = tECHSCR10ViewModel.CallingReport;
            tECHSCR10ViewModel.SelectedReportFormat = "PDF";
            if (tECHSCR10ViewModel.CallingReport == "RECORDS.rep" || tECHSCR10ViewModel.CallingReport == "MILESTONES.rep")
            {
                ReportData.Query = "FR_DATE=" + Convert.ToDateTime(tECHSCR10ViewModel.FromDate).ToString("dd/MMM/yyyy") + "+" +
                "T_DATE=" + Convert.ToDateTime(tECHSCR10ViewModel.ToDate).ToString("dd/MMM/yyyy") + "+" +
                 "D=" + Convert.ToDateTime(tECHSCR10ViewModel.ToDate).ToString("dd/MMM/yyyy");
            }
            return ReportData;
           
        }
    }
}
