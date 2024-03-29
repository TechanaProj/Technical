﻿using IFFCO.HRMS.Service;
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
    public class TECHSCR18Controller : BaseController<TECHSCR18ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR18Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            CommonViewModel.ToDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            CommonViewModel.FromDate = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.InputLOVList = InputLOVBind();
            return View(CommonViewModel);
        }

        public SelectList InputLOVBind()
        {
            var dictionary = new Dictionary<string, string>
            {
               {"Monthly Basis", "M"},
               {"Weekly Basis", "W"},
            };

            var selectList = new SelectList(dictionary, "Value", "Key");

            return selectList;
        }

        public ActionResult GenerateReport(TECHSCR18ViewModel tECHSCR18ViewModel)
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
            Report reportobj = GenerateReportData(tECHSCR18ViewModel, separator);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + tECHSCR18ViewModel.SelectedReportFormat;
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
            return Json(CommonViewModel);
        }

       
        public Report GenerateReportData(TECHSCR18ViewModel tECHSCR18ViewModel, string seprator)
        {
            bool rdlc = false;
            string extension = "rep";
            if (HttpContext.Session.GetString("ReportServer").ToLower().Contains("tech"))
            {
                rdlc = true;
                extension = "aspx";
            }
            Report ReportData = new Report();
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            ReportData.ReportName = tECHSCR18ViewModel.CallingReport;
            tECHSCR18ViewModel.SelectedReportFormat = "PDF";
            ReportData.ReportName = "GAS_BIFUR."+extension;
            ReportData.Query = "I_DT1=" + Convert.ToDateTime(tECHSCR18ViewModel.FromDate).ToString("dd/MMM/yyyy") + seprator +
                               "I_DT2=" + Convert.ToDateTime(tECHSCR18ViewModel.ToDate).ToString("dd/MMM/yyyy") + seprator + 
                               "TPE=" + tECHSCR18ViewModel.InputType;  
            return ReportData;
        }
    }
}
