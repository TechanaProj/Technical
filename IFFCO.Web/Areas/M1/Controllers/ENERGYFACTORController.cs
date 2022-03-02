
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
    public class ENERGYFACTORController : BaseController<ENERGYFACTORViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENERGYFACTORViewModel> commonException = null;

        public ENERGYFACTORController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENERGYFACTORViewModel>();
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
                DateTime dt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
                DateTime dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                List<EnergyFactor> data = TechnicalCommonService.GetRecordsENERGYFACTOR(dt1, dt2);               
                ViewBag.records =data;
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


        public IActionResult Save( DateTime FromDate, DateTime ToDate, DateTime FromEDate, DateTime ToEDate,string Unit,string PrCode,string PrValue)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                
              int i = TechnicalCommonService.SaveRecordsENERGYFACTOR(FromEDate, ToEDate,  Unit,  PrCode,  PrValue,EMP_ID.ToString());
                if (i>0)
                {
                    List<EnergyFactor> data = TechnicalCommonService.GetRecordsENERGYFACTOR(FromEDate, ToEDate);
                    ViewBag.records = data;
                }
                else if(i==-1)
                {
                    CommonViewModel.errorMessage = "Pr Code already added";
                    return Json(CommonViewModel);
                }
                     
            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialENERGYFACTOR");
        }
        public IActionResult Update(DateTime FromDate, DateTime ToDate, DateTime FromEDate, DateTime ToEDate, string Unit, string PrCode, string PrValue)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {

                //int i = TechnicalCommonService.UpdateRecordsENERGYFACTOR(FromEDate, ToEDate, Unit, PrCode, PrValue, EMP_ID.ToString());
                if (true)
                {
                    List<EnergyFactor> data = TechnicalCommonService.GetRecordsENERGYFACTOR(FromEDate, ToEDate);
                    ViewBag.records = data;
                }

            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialENERGYFACTOR");
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
                        List<EnergyFactor> data = TechnicalCommonService.GetRecordsENERGYFACTOR(FromDate, ToDate);
                        ViewBag.records = data;

                        break;
                    case "save":
                        CommonViewModel.alert = "Data Saved";
                        return Json(CommonViewModel);
                    case "approve":
                        CommonViewModel.alert = TechnicalCommonService.ApproveRecordsSPTARGET(FromDate, ToDate);
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




            return PartialView("_partialENERGYFACTOR");
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


                CommonViewModel.alert = TechnicalCommonService.PostRecordsSPTARGET(EMP_ID.ToString(), FromDate, ToDate, Input_Value, Input_Name, OperationType);
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