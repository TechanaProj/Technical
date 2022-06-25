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
            bool rdlc = false;
            string separator = "+";
            string extension = "rep";
            var fullClientIp = HttpContext.Session.GetString("fullClientIp");
            var clientIp = HttpContext.Session.GetString("clientIp");
            if (HttpContext.Session.GetString("ReportServer").ToLower().Contains("tech"))
            {
                
                rdlc = true;
                separator = "&";
                extension = "aspx";
               
            }

            string Report = "";
            string QueryString = String.Empty;
            tECHSCR10ViewModel.CallingReport = tECHSCR10ViewModel.CallingReport.Split(".")[0] + "." + extension;
            tECHSCR10ViewModel.SelectedReportFormat = "PDF";
            if (tECHSCR10ViewModel.CallingReport.Contains("RECORDS") || tECHSCR10ViewModel.CallingReport.Contains("MILESTONES"))
            {
                Report reportobj = GenerateReportData(tECHSCR10ViewModel, separator);
                string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR10ViewModel.SelectedReportFormat;


                if (rdlc)
                {
                    Report = reportRepository.GenerateReportRdlc(HttpContext.Session.GetString("ReportServer"),
                          reportobj.Query,
                          reportobj.ReportName,
                          this.ControllerContext.RouteData.Values["area"].ToString(),
                          this.ControllerContext.RouteData.Values["controller"].ToString(),
                          HttpContext.Session.GetInt32("EmpID").ToString(), fullClientIp, clientIp);
                }
                else
                {

                    Report = reportRepository.GenerateReport(reportobj.Query, data);

                }
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                CommonViewModel.Report = Report;
            }
            return Json(CommonViewModel);
        }

       
        public Report GenerateReportData(TECHSCR10ViewModel tECHSCR10ViewModel, string seprator)
        {
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = tECHSCR10ViewModel.CallingReport;
            ReportData.Query = "FR_DATE=" + Convert.ToDateTime(tECHSCR10ViewModel.FromDate).ToString("dd/MMM/yyyy") + seprator +
                "T_DATE=" + Convert.ToDateTime(tECHSCR10ViewModel.ToDate).ToString("dd/MMM/yyyy") + seprator +
                 "D=" + Convert.ToDateTime(tECHSCR10ViewModel.ToDate).ToString("dd/MMM/yyyy");
            
            return ReportData;
           
        }
    }
}
