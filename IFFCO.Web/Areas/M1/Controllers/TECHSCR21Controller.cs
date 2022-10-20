
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
    public class TECHSCR21Controller : BaseController<TECHSCR21ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR21ViewModel> commonException = null;

        public TECHSCR21Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR21ViewModel>();
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




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate, string Plant)
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
            Report reportobj = GenerateReportData(FromDate, ToDate, Plant,separator);
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
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string Plant, string seprator)
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
            ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();

            switch (Plant)
            {
                case "A1":
                    ReportData.ReportName = "12AReport1."+extension;
                    break;
                case "A2":
                    ReportData.ReportName = "12AReport2."+extension;
                    break;
                case "A1R":
                    ReportData.ReportName = "12AReport1r2."+extension;
                    break;
                case "A2R":
                    ReportData.ReportName = "12AReport2r2."+extension;
                    break;
                case "PP":
                    ReportData.ReportName = "12AReport1r2_N."+extension;
                    break;
               
                case "A3":
                    ReportData.ReportName = "12AReportrec."+extension;
                    break;
                case "AB":
                    ReportData.ReportName = "Amm_balance."+extension;
                    break;
                case "SH23":
                    ReportData.ReportName = "12Acheck23_A1."+extension;
                    break;
                case "SH232":
                    ReportData.ReportName = "12Acheck23_A2."+extension;
                    break;
                case "A1N":
                    ReportData.ReportName = "12AReport1_N."+extension;
                    break;
                case "A2N":
                    ReportData.ReportName = "12AReport2_N."+extension;
                    break;
                case "A3N":
                    ReportData.ReportName = "12AReportrec_N."+extension;
                    break;
                case "A1RN":
                    ReportData.ReportName = "12AReport1r2_N."+extension;
                    break;
                case "A2RN":
                    ReportData.ReportName = "12AReport2r2_N."+extension;
                    break;
                default:

                    break;

            }

            return ReportData;
        }


    }
}