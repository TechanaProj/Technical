
using Devart.Data.Oracle;
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
    public class EREMARKController : BaseController<EREMARKViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<EREMARKViewModel> commonException = null;

        public EREMARKController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<EREMARKViewModel>();
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
            List<CommonData> data = TechnicalCommonService.GetRecordsEREMARK(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-16), EMP_ID.ToString());
            ViewBag.records = data;
            return View(CommonViewModel);
        }
        public IActionResult Execute(string OperationType, string Shift, DateTime FromDate, DateTime ToDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            switch (OperationType)
            {
                case "query":
                    List<CommonData> data = TechnicalCommonService.GetRecordsEREMARK(FromDate, ToDate, EMP_ID.ToString());
                    ViewBag.reason = TechnicalCommonService.GetReason();
                    ViewBag.records = data;

                    break;
                case "save":
                    CommonViewModel.alert = "Data Saved";
                    return Json(CommonViewModel);
                default:
                    break;
            }
            return PartialView("_partialEREMARK");

        }
        public IActionResult PostData(DateTime FromDate, DateTime ToDate, string Input_Name, string Input_Value, string InputType)
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
                    break;//tech//
            }
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();


            CommonViewModel.alert = TechnicalCommonService.PostRecordsEREMARK(EMP_ID.ToString(), FromDate, ToDate,Input_Value, Input_Name);
            return Json(CommonViewModel);
        }
    }
}