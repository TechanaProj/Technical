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
    public class TECHSCR26Controller : BaseController<TECHSCR01AViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR01AViewModel> commonException = null;

        public TECHSCR26Controller(ModelContext context)
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




        public ActionResult GenerateReport(string FinYear, string ReportType)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FinYear, ReportType);
            //Report reportobj2 = GenerateReportData2(ReportDate);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(string FinYear,  string ReportType)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";

            switch (ReportType)
            {
                case "A1":
                    ReportData.Query = "FIN=" + FinYear;
                    ReportData.ReportName = "TOP3_A1.rep";
                    break;
                default:
                case "A2":
                    ReportData.Query = "FIN=" + FinYear;
                    ReportData.ReportName = "TOP3_A2.rep";
                    break;
            }

            //ReportData.Query = "P_Idt=" + ReportDate.Date();
            //ReportData.ReportName = "MDtelexN.rep";
            return ReportData;
        }




    }
}