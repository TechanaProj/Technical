
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
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FromDate, ToDate, Plant);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string Plant)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";
            ReportData.Query = "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date();

            switch (Plant)
            {
                case "A1":
                    ReportData.ReportName = "12AReport1.rep";
                    break;
                case "A2":
                    ReportData.ReportName = "12AReport2.rep";
                    break;
                case "A1R":
                    ReportData.ReportName = "12AReport1r2.rep";
                    break;
                case "A2R":
                    ReportData.ReportName = "12AReport2r2.rep";
                    break;
                case "PP":
                    ReportData.ReportName = "12AReport1r2_N.rep";
                    break;
               
                case "A3":
                    ReportData.ReportName = "12AReportrec.rep";
                    break;
                case "AB":
                    ReportData.ReportName = "Amm_balance.rep";
                    break;
                case "SH23":
                    ReportData.ReportName = "12Acheck23_A1.rep";
                    break;
                case "SH232":
                    ReportData.ReportName = "12Acheck23_A2.rep";
                    break;
                case "A1N":
                    ReportData.ReportName = "12AReport1_N.rep";
                    break;
                case "A2N":
                    ReportData.ReportName = "12AReport2_N.rep";
                    break;
                case "A3N":
                    ReportData.ReportName = "12AReportrec_N.rep";
                    break;
                case "A1RN":
                    ReportData.ReportName = "12AReport1r2_N.rep";
                    break;
                case "A2RN":
                    ReportData.ReportName = "12AReport2r2_N.rep";
                    break;
                default:

                    break;

            }

            return ReportData;
        }


    }
}