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
    public class TECHSCR28CController : BaseController<TECHSCR28ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR28ViewModel> commonException = null;

        public TECHSCR28CController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR28ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            CommonViewModel.ToDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            return View(CommonViewModel);
        }

        public ActionResult GenerateReport(TECHSCR28ViewModel tECHSCR28ViewModel)
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
            Report reportobj = GenerateReportData(tECHSCR28ViewModel);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR28ViewModel.SelectedReportFormat;

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


        public Report GenerateReportData(TECHSCR28ViewModel tECHSCR28ViewModel)
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
            ReportData.ReportName = "HINDI1delh2N." + extension;
            tECHSCR28ViewModel.SelectedReportFormat = "PDF";
            ReportData.Query = "i_dt=" + Convert.ToDateTime(tECHSCR28ViewModel.ToDate).ToString("dd/MMM/yyyy");
            return ReportData;
        }
    }
}
