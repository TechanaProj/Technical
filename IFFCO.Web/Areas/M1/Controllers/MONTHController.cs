﻿using Devart.Data.Oracle;
using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class MONTHController : BaseController<MONTHViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<MONTHViewModel> commonException = null;

        public MONTHController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<MONTHViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            TechnicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            List<CommonData> data = TechnicalCommonService.GetRecordsMONTH(controller,  EMP_ID.ToString(), DateTime.Now,DateTime.Now,"COMPOSITE");
            ViewBag.reason = TechnicalCommonService.GetReason();
            ViewBag.records = data;



            return View(CommonViewModel);
        }
        public IActionResult Execute(string OperationType, string Gas, DateTime FromDate, DateTime ToDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            switch (OperationType)
            {
                case "query":
                    List<CommonData> data = TechnicalCommonService.GetRecordsMONTH(controller, EMP_ID.ToString(), FromDate, ToDate, Gas);
                    ViewBag.reason = TechnicalCommonService.GetReason();
                    ViewBag.records = data;

                    break;
                case "save":
                    break;
                case "approve":
                    //TechnicalCommonService.ApproveRecordsMONTH(controller, Shift, EMP_ID.ToString(), FromDate);
                    //List<CommonData> data1 = TechnicalCommonService.GetRecordsMONTH(controller, Shift, EMP_ID.ToString(), FromDate);
                    //ViewBag.reason = TechnicalCommonService.GetReason();
                    //ViewBag.records = data1;

                    break;
                default:
                    break;
            }
            return PartialView("_partialMONTHLY");
        }
        public IActionResult PostData(DateTime FromDate, DateTime ToDate, string pno, string gastype, string Input_Value, string Input_Name, string InputType)
        {
            switch (InputType)
            {
                case "datetime-local":
                    Input_Value = Convert.ToDateTime(Input_Value.Replace("T", " ")).ToString("MM/dd/yyyy HH:mm:ss");
                    break;
                case "date":
                    Input_Value = Convert.ToDateTime(Input_Value).Date();
                    break;
                default:
                    break;
            }
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();


            string alert = TechnicalCommonService.PostRecordsMONTH( FromDate,  ToDate,  pno,  gastype,  Input_Value,  Input_Name);
            return Json(alert);
        }
        public IActionResult PostShutDownData(string Shift, DateTime DataDate, string Reason, string ReasonCode, string SD_PLANT, DateTime? FromDate, DateTime? ToDate, String InputType)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();


            string alert = TechnicalCommonService.PostShutdownMONTH(Shift, DataDate, Reason, ReasonCode, SD_PLANT, FromDate, ToDate, controller, EMP_ID.ToString(), InputType);
            return Json("");



        }
        public IActionResult PostTechRemarkData(string Shift, DateTime DataDate, string ReasonName, string RemarksValue)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();


            string alert = TechnicalCommonService.PostTechRemarkMONTH(Shift, DataDate, ReasonName, RemarksValue, EMP_ID.ToString(), controller);
            return Json("");



        }
    }
}