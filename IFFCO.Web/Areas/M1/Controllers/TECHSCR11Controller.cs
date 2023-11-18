
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
    public class TECHSCR11Controller : BaseController<TECHSCR11ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR11ViewModel> commonException = null;

        public TECHSCR11Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR11ViewModel>();
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
                case "0":
                    ReportData.Query = "FROM_DT=" + FromDate.Date() + seprator + "TO_DT=" + ToDate.Date();
                    ReportData.ReportName = "Energy_report." + extension;
                    break;
                case "1":
                    ReportData.Query = "P_Idt=" + ToDate.Date();
                    ReportData.ReportName = "BL_ENERGY." + extension;
                    break;
                case "2":
                    ReportData.Query = "FROM_DT=" + FromDate.Date() + seprator + "TO_DT=" + ToDate.Date();
                    ReportData.ReportName = "SGPG_EFF." + extension;
                    break;
                case "3":
                    if (rdlc)
                    {
                        ReportData.Query = "FROM_DATE=" + FromDate.Date() + seprator + "TO_DATE=" + ToDate.Date();
                        ReportData.ReportName = "Electrical_balance_daily." + extension;
                        break;
                    }
                    else
                    {
                        ReportData.Query = "FROM_DATE=" + FromDate.Date() + seprator + "TO_DATE=" + ToDate.Date();
                        ReportData.ReportName = "Electrical_balabce_daily." + extension;
                        break;
                    }


                default:

                    break;
            }

            return ReportData;
        }


    }
}