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
    public class TECHSCR13Controller : BaseController<TECHSCR01AViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR01AViewModel> commonException = null;

        public TECHSCR13Controller(ModelContext context)
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




        public ActionResult GenerateReport(DateTime FromDate,DateTime ToDate, string ReportType)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FromDate, ToDate, ReportType);
            //Report reportobj2 = GenerateReportData2(ReportDate);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string ReportType)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";

            switch (ReportType)
            {
                case "M1":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date()+"+" +"TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "monthlyreport13N.rep";
                    break;
                default:
                case "M2":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "monthlyreport2N.rep";
                    break;
                case "M4":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "monthlyreport4.rep";
                    break;
                case "M5":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "monthlyreport5.rep";
                    break;
                case "SNAP":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "MDATA.rep";
                    break;
                case "W":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "plantdata.rep";
                    break;
                case "E":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "plantdatae.rep";
                    break;
                case "R":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "plantdatar.rep";
                    break;
                case "NP":
                    ReportData.Query = "FROM_DATE=" + FromDate.Date() + "+" + "TO_DATE=" + ToDate.Date();
                    ReportData.ReportName = "MDDATA.rep";
                    break;
            }

            //ReportData.Query = "P_Idt=" + ReportDate.Date();
            //ReportData.ReportName = "MDtelexN.rep";
            return ReportData;
        }




    }
}