
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
    public class ENERGYRECORDController : BaseController<ENERGYRECORDViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENERGYRECORDViewModel> commonException = null;

        public ENERGYRECORDController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENERGYRECORDViewModel>();
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
                List<ENERGYRECORD> data = TechnicalCommonService.GetRecordsENERGYRECORD();
                ViewBag.ListItem = TechnicalCommonService.GetPlantList();
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


        public IActionResult Save(string Plant, string ForMonth, string UptoMonth,string Period)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {

                int i = TechnicalCommonService.SaveRecordsENERGYRECORD(Plant, ForMonth,UptoMonth,  Period, EMP_ID.ToString());
                if (i > 0)
                {
                    List<ENERGYRECORD> data = TechnicalCommonService.GetRecordsENERGYRECORD();
                    ViewBag.records = data;
                }
                else if (i == -1)
                {
                    CommonViewModel.errorMessage = "Plant already added";
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




            return PartialView("_partialENERGYRECORD");
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
                        List<ENERGYRECORD> data = TechnicalCommonService.GetRecordsENERGYRECORD();
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




            return PartialView("_partialENERGYRECORD");
        }
      
    }
}