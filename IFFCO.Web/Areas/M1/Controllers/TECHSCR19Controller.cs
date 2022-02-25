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
                case "gas":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_qty.rep";
                    break;
                default:
                case "gasL":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_CV.rep";
                    break;
                case "gasg":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_GCV.rep";
                    break;
                case "gasle":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_LENG.rep";
                    break;
                case "gasge":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NG_GENG.rep";
                    break;
                case "nap":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAP_CVY.rep";
                    break;
                case "M":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAP_CVM.rep";
                    break;
                case "H":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPHYR.rep";
                    break;
                case "Q":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPQUART.rep";
                    break;
                case "S":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "NAPSUMMARY.rep";
                    break;
                case "Gasal":
                    ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();
                    ReportData.ReportName = "GAS_ALLOC.rep";
                    break;
            }

            //ReportData.Query = "P_Idt=" + ReportDate.Date();
            //ReportData.ReportName = "MDtelexN.rep";
            return ReportData;
        }




    }
}