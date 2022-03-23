
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
    public class TECHSCR24Controller : BaseController<TECHSCR24ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR25ViewModel> commonException = null;

        public TECHSCR24Controller(ModelContext context)
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




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate,string ReportName)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FromDate, ToDate, ReportName);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string ReportName)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";
            ReportData.Query = "I_DT1=" + FromDate.Date() + "+" + "I_DT2=" + ToDate.Date() + "+" + "FRM_DATE=" + FromDate.Date() + "+" + "T_DATE=" + ToDate.Date() + "+" + "FYR=" + FromDate.Year.ToString()+"-"+ToDate.Year.ToString();
            ReportData.ReportName = ReportName;
            return ReportData;
        }


    }
}