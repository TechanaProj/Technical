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
    public class FACTORMASTERController : BaseController<FACTORMASTERViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<FACTORMASTERViewModel> commonException = null;

        public FACTORMASTERController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<FACTORMASTERViewModel>();
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
                List<FactorMaster> data = TechnicalCommonService.GetRecordsFACTORMASTER("");
                ViewBag.ListItem = TechnicalCommonService.GetFactorList();
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



        public IActionResult Save(string op,string Code, string Unit, string Name, string Value, DateTime FromDate, DateTime? ToDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));

            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {


                if (op == "update")
                {
                    int i = TechnicalCommonService.UpdateRecordsFACTORMASTER(Code, Unit, Name, Value, FromDate, ToDate, EMP_ID.ToString());
                    if (i > 0)
                    {
                        List<FactorMaster> data = TechnicalCommonService.GetRecordsFACTORMASTER(Code);
                        ViewBag.records = data;
                    }
                    else if (i == -1)
                    {
                        CommonViewModel.errorMessage = "From Date With Same FR Code already exists";
                        return Json(CommonViewModel);
                    }
                }
                else
                {
                    int i = TechnicalCommonService.SaveRecordsFACTORMASTER(Code, Unit, Name, Value, FromDate, ToDate, EMP_ID.ToString());
                    if (i > 0)
                    {
                        List<FactorMaster> data = TechnicalCommonService.GetRecordsFACTORMASTER(Code);
                        ViewBag.records = data;
                    }
                    else if (i == -1)
                    {
                        CommonViewModel.errorMessage = "From Date With Same FR Code already exists";
                        return Json(CommonViewModel);
                    }
                }
            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialFACTORMASTER");
        }


   

        public IActionResult Execute(string FrName)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                List<FactorMaster> data = TechnicalCommonService.GetRecordsFACTORMASTER(FrName);
                ViewBag.records = data;


            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialFACTORMASTER");
        }

    }
}