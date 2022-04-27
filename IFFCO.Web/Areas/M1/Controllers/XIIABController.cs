
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
    public class XIIABController : BaseController<XIIABViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<XIIABViewModel> commonException = null;

        public XIIABController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<XIIABViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            TechnicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {
            try
            {
                int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                List<CommonData> data = TechnicalCommonService.GetRecordsXIIAB(controller, "G", EMP_ID.ToString(), DateTime.Now.AddDays(-1), DateTime.Now);
                ViewBag.reason = TechnicalCommonService.GetReason();
                ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, DateTime.Now.AddDays(-1));
                ViewBag.records = data;
            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }



            return View(CommonViewModel);
        }
        public IActionResult Execute(string OperationType, string Shift, DateTime FromDate, DateTime ToDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                switch (OperationType)
                {
                    case "query":
                        List<CommonData> data = TechnicalCommonService.GetRecordsXIIAB(controller, Shift, EMP_ID.ToString(), FromDate, ToDate);
                        ViewBag.reason = TechnicalCommonService.GetReason();
                        ViewBag.records = data;
                        ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, FromDate);
                        break;
                    case "save":
                        CommonViewModel.alert = "Data Saved";
                        return Json(CommonViewModel);
                    case "approve":
                        //TechnicalCommonService.ApproveRecordsXIIAB(controller, Shift, EMP_ID.ToString(), FromDate);
                        //List<CommonData> data1 = TechnicalCommonService.GetRecordsXIIAB(controller, Shift, EMP_ID.ToString(), FromDate);
                        //ViewBag.reason = TechnicalCommonService.GetReason();
                        //ViewBag.records = data1;

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialXIIAB");
        }
        public IActionResult PostData(string OperationType, string Shift, DateTime FromDate, DateTime ToDate, string Input_Name, string Input_Value, string InputType)
        {
            try
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


                CommonViewModel.alert = TechnicalCommonService.PostRecordsXIIAB(controller, Shift, EMP_ID.ToString(), FromDate, ToDate, Input_Value, Input_Name, OperationType);
                return Json(CommonViewModel);
            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




          
        }
    }
}