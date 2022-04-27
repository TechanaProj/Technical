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
    public class OSSC01Controller : BaseController<OSSC01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<OSSC01ViewModel> commonException = null;

        public OSSC01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<OSSC01ViewModel>();
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
                List<CommonData> data = TechnicalCommonService.GetRecordsOSSC01(controller, "G", EMP_ID.ToString(), DateTime.Now.AddDays(-1));
                ViewBag.reason = TechnicalCommonService.GetReason();
                ViewBag.records = data;
                ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, DateTime.Now.AddDays(-1));
                ViewBag.frValue = TechnicalCommonService.GetFrValue(DateTime.Now.AddDays(-1));
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
                        List<CommonData> data = TechnicalCommonService.GetRecordsOSSC01(controller, Shift, EMP_ID.ToString(), FromDate);
                        ViewBag.reason = TechnicalCommonService.GetReason();
                        ViewBag.records = data;
                        ViewBag.rights = TechnicalCommonService.GetScreenAccess(EMP_ID, controller, FromDate);
                        ViewBag.frValue = TechnicalCommonService.GetFrValue(FromDate);
                        
                        break;
                    case "save":
                        CommonViewModel.alert = "Data Saved";
                        return Json(CommonViewModel);
                       
                    case "approve":
                        CommonViewModel.alert = TechnicalCommonService.ApproveRecordsOSSC01(controller, Shift, EMP_ID.ToString(), FromDate);
                        //List<CommonData> data1 = TechnicalCommonService.GetRecordsOSSC01(controller, Shift, EMP_ID.ToString(), FromDate);
                        ViewBag.reason = TechnicalCommonService.GetReason();
                        //ViewBag.records = data1;
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
            return PartialView("_partialOSSC01");
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


                CommonViewModel.alert = TechnicalCommonService.PostRecordsOSSC01(controller, Shift, EMP_ID.ToString(), FromDate, Input_Value, Input_Name, OperationType).ToProperCase();
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