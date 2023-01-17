
using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHSCR25Controller : BaseController<TECHSCR25ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR25ViewModel> commonException = null;

        public TECHSCR25Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR25ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            //int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            //string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            //List<SelectListItem> Units = dropDownListBindWeb.GetSuggestionUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);
            //CommonViewModel.UnitLOV = Units;
            //CommonViewModel.UnitCd = HttpContext.Session.GetString("UnitCode");
            return View(CommonViewModel);
        }




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate, string R1, string R2, string R3, string P1, string P2, string P3,string Plant)
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
            Report reportobj = GenerateReportData(FromDate, ToDate, R1,R2,R3,P1,P2,P3,Plant,separator);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

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
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string R1, string R2, string R3, string P1, string P2, string P3,string Plant,string seprator)
        {
            bool rdlc = false;
            string extension = "rep";
            if (HttpContext.Session.GetString("ReportServer").ToLower().Contains("tech"))
            {
                rdlc = true;
                extension = "aspx";
            }

            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";
            ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date() + seprator + "R1=" + R1 + seprator + "R2=" + R2 + seprator + "R3=" + R3 + seprator + "P1=" + P1 + seprator + "P2=" + P2 + seprator + "P3=" + P3;

            switch (Plant)
            {
                case "TP6A1":
                    ReportData.ReportName = "TOP6_A1."+extension;
                    break;
                case "TP6A2":
                    ReportData.ReportName = "TOP6_A2."+extension;
                    break;
               
                default:
                    break;
            }
            return ReportData;
        }
    }
}