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
    public class TECHSCR15Controller : BaseController<TECHSCR15ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR15ViewModel> commonException = null;

        public TECHSCR15Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR15ViewModel>();
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

        public ActionResult GenerateReport(TECHSCR15ViewModel tECHSCR15ViewModel)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(tECHSCR15ViewModel);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR15ViewModel.SelectedReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }


        public Report GenerateReportData(TECHSCR15ViewModel tECHSCR15ViewModel)
        {
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = "NGSHFALLN.rep";
            tECHSCR15ViewModel.SelectedReportFormat = "PDF";
            ReportData.Query = "IDT1=" + Convert.ToDateTime(tECHSCR15ViewModel.FromDate).ToString("dd/MMM/yyyy") + "+" + "IDT2=" + Convert.ToDateTime(tECHSCR15ViewModel.ToDate).ToString("dd/MMM/yyyy");
            return ReportData;
        }
    }
}
