﻿
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
    public class TECHSCR17EController : BaseController<TECHSCR17EViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR17AViewModel> commonException = null;

        public TECHSCR17EController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR17AViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate, string ForReport, string Gas)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FromDate, ToDate, ForReport,Gas);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(DateTime FromDate, DateTime ToDate, string ForReport, string Gas)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";

            ReportData.Query = "I_DT1=" + FromDate.Date() + "+" + "I_DT2=" + ToDate.Date();
            ReportData.ReportName = "Norms_CHK_FF.rep";


            return ReportData;
        }


    }
}