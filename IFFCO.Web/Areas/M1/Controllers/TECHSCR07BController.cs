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
    public class TECHSCR07BController : BaseController<TECHSCR07BViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR07BViewModel> commonException = null;

        public TECHSCR07BController(ModelContext context)
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




        public ActionResult GenerateReport(DateTime FromDate, DateTime ToDate, string ReportType)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(FromDate,ToDate,ReportType);
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
                case "0":
                    ReportData.Query = "IDT=" + FromDate.Date()+"+"+ "IDT2="+ToDate.Date();
                    ReportData.ReportName = "intrep.rep";
                    break;
                case "1":
                    ReportData.Query = "P_dt=" + ToDate.Date();
                    ReportData.ReportName = "Lab_Data_Report.rep";
                    break;
                case "2":
                    ReportData.Query = "P_dt=" + ToDate.Date();
                    ReportData.ReportName = "consum.rep";
                    break;
                
                default:
                   
                    break;

            }

            return ReportData;
        }


    }
}