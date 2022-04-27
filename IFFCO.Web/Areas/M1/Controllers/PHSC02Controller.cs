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
    public class PHSC02Controller : BaseController<PHSC02ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<PHSC02ViewModel> commonException = null;

        public PHSC02Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<PHSC02ViewModel>();
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
                List<CommonData> data = TechnicalCommonService.GetRecordsPHSC02(controller, "G", EMP_ID.ToString(), DateTime.Now.AddDays(-1));
                ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, DateTime.Now.AddDays(-1));
                ViewBag.reason = TechnicalCommonService.GetReason();

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
        public IActionResult Execute(string OperationType, string Shift, DateTime FromDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                switch (OperationType)
                {
                    case "query":
                        List<CommonData> data = TechnicalCommonService.GetRecordsPHSC02(controller, Shift, EMP_ID.ToString(), FromDate);
                        ViewBag.reason = TechnicalCommonService.GetReason();
                        ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, FromDate);
                        ViewBag.records = data;

                        break;
                    case "save":

                        CommonViewModel.alert = "Data Saved";
                        return Json(CommonViewModel);
                    case "approve":
                        CommonViewModel.alert = TechnicalCommonService.ApproveRecordsPHSC02(controller, Shift, EMP_ID.ToString(), FromDate);
                        return Json(CommonViewModel);

                      
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
            return PartialView("_partialPHSC02");
        }
        public IActionResult PostData(string OperationType, string Shift, DateTime FromDate, string Input_Name, string Input_Value, string InputType)
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


                CommonViewModel.alert = TechnicalCommonService.PostRecordsPHSC02(controller, Shift, EMP_ID.ToString(), FromDate, Input_Value, Input_Name, OperationType);

            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }
            return Json(CommonViewModel);
        }

    }
}