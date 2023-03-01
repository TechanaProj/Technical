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
    public class TECHSCR20AController : BaseController<TECHSCR20AViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR20AController(ModelContext context)
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
            //ViewBag.PlantLOVList = PlantLOVBind();
            return View(CommonViewModel);
        }

        //public SelectList PlantLOVBind()
        //{
        //    var dictionary = new Dictionary<string, string>
        //    {
        //       {"Aonla1", "A1"},
        //       {"Aonla2", "A2"},
        //    };

        //    var selectList = new SelectList(dictionary, "Value", "Key");

        //    return selectList;
        //}

        public ActionResult GenerateReport(TECHSCR20AViewModel TECHSCR20AViewModel)
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
            Report reportobj = GenerateReportData(TECHSCR20AViewModel, separator);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + TECHSCR20AViewModel.SelectedReportFormat;
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


        public Report GenerateReportData(TECHSCR20AViewModel TECHSCR20AViewModel, string seprator)
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
            ReportData.ReportName = TECHSCR20AViewModel.CallingReport;
            TECHSCR20AViewModel.SelectedReportFormat = "PDF";
            ReportData.ReportName = "Mnpwr."+extension;
            ReportData.Query = "FR_DATE=" + Convert.ToDateTime(TECHSCR20AViewModel.FromDate).ToString("dd/MMM/yyyy") + seprator +
                               "TO_DATE=" + Convert.ToDateTime(TECHSCR20AViewModel.ToDate).ToString("dd/MMM/yyyy");
                               //"P_UNIT=" + TECHSCR20AViewModel.PlantType;
            return ReportData;
        }
    }
}
