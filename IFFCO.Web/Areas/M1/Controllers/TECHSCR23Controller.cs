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
    public class TECHSCR23Controller : BaseController<TECHSCR23ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR23Controller(ModelContext context)
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

        public ActionResult GenerateReport(TECHSCR23ViewModel tECHSCR23ViewModel)
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
            Report reportobj = GenerateReportData(tECHSCR23ViewModel, separator);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR23ViewModel.SelectedReportFormat;
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

                Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");

            }
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);

        }


        public Report GenerateReportData(TECHSCR23ViewModel tECHSCR23ViewModel,string seprator)
        {
            bool rdlc = false;
            string extension = "rep";
            if (HttpContext.Session.GetString("ReportServer").ToLower().Contains("tech"))
            {
                rdlc = true;
                extension = "aspx";
            }

            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = tECHSCR23ViewModel.CallingReport;
            tECHSCR23ViewModel.SelectedReportFormat = "PDF";
            if (tECHSCR23ViewModel.PlantType == "A1")
            {
                ReportData.ReportName = "INVENTORY_REP1."+extension;
                ReportData.Query = "P_FR_DT=" + Convert.ToDateTime(tECHSCR23ViewModel.ToDate).ToString("dd/MMM/yyyy") + seprator +
                                   "P_UNIT=" + tECHSCR23ViewModel.PlantType;
            }
            else
            {
                ReportData.ReportName = "INVENTORY_REP2."+extension;
                ReportData.Query = "P_FR_DT=" + Convert.ToDateTime(tECHSCR23ViewModel.ToDate).ToString("dd/MMM/yyyy") + seprator +
                                   "P_UNIT=" + tECHSCR23ViewModel.PlantType;
            }
            return ReportData;
        }
    }
}
