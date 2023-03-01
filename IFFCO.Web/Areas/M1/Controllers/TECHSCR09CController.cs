
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
    public class TECHSCR09CController : BaseController<TECHSCR07BViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR07BViewModel> commonException = null;

        public TECHSCR09CController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR07BViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }




        public ActionResult GenerateReport(DateTime Date, string ReportType, string Operation)
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


            if (Operation.ToLower() == "send-data")
            {

            }
            else
            {

                Report reportobj = GenerateReportData(Date, ReportType, separator);
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
            }
            return Json(CommonViewModel);
        }

        public Report GenerateReportData(DateTime dt, string ReportType, string seprator)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";
            bool rdlc = false;
            string extension = "rep";
            if (HttpContext.Session.GetString("ReportServer").ToLower().Contains("tech"))
            {
                rdlc = true;
                extension = "aspx";
            }

            ReportData.Query = "I_DT=" + dt.Date();
            switch (ReportType)
            {
                case "TP6A1":

                    ReportData.ReportName = "pmis_down."+extension;
                    break;
                case "TP6A2":

                    ReportData.ReportName = "pmis_prod."+extension;
                    break;
                case "TP6A3":

                    ReportData.ReportName = "pmis_desp."+extension;
                    break;
                case "TP6A4":

                    ReportData.ReportName = "pmis_stock."+extension;
                    break;
                default:

                    break;
            }

            return ReportData;
        }


    }
}