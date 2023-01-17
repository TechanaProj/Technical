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
    public class TECHSCR19Controller : BaseController<TECHSCR01AViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR01AViewModel> commonException = null;

        public TECHSCR19Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR01AViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate, string ReportType)
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
            Report reportobj = GenerateReportData(FromDate, ToDate, ReportType, separator);
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
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string ReportType, string seprator)
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

            switch (ReportType)
            {
                case "gas":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_qty."+extension;
                    break;
                default:
                case "gasL":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_CV."+extension;
                    break;
                case "gasg":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_GCV."+extension;
                    break;
                case "gasle":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_LENG."+extension;
                    break;
                case "gasge":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_GENG."+extension;
                    break;
                case "nap":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAP_CVY."+extension;
                    break;
                case "M":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAP_CVM."+extension;
                    break;
                case "H":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPHYR."+extension;
                    break;
                case "Q":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPQUART."+extension;
                    break;
                case "S":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPSUMMARY."+extension;
                    break;
                case "Gasal":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + seprator + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "GAS_ALLOC."+extension;
                    break;
            }

            //ReportData.Query = "P_Idt=" + ReportDate.Date();
            //ReportData.ReportName = "MDtelexN.rep";
            return ReportData;
        }




    }
}