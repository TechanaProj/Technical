using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHSCR14Controller : BaseController<TECHSCR14ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR14ViewModel> commonException = null;

        public TECHSCR14Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR14ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            CommonViewModel.ToDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            return View(CommonViewModel);
        }

        public ActionResult GenerateReport(TECHSCR14ViewModel tECHSCR14ViewModel)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(tECHSCR14ViewModel);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR14ViewModel.SelectedReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }


        public Report GenerateReportData(TECHSCR14ViewModel tECHSCR14ViewModel)
        {
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = "NGANALYSISN.rep";
            tECHSCR14ViewModel.SelectedReportFormat = "PDF";
            ReportData.Query = "I_DT1=" + Convert.ToDateTime(tECHSCR14ViewModel.FromDate).ToString("dd/MMM/yyyy") + "+" + "I_DT2=" + Convert.ToDateTime(tECHSCR14ViewModel.ToDate).ToString("dd/MMM/yyyy");
            return ReportData;
        }
    }
}
