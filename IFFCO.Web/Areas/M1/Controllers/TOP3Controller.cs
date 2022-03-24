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
    public class TOP3Controller : BaseController<TOP3ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TOP3ViewModel> commonException = null;

        public TOP3Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TOP3ViewModel>();
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
                List<TOP3Data> data = TechnicalCommonService.GetRecordsTOP3("2021-22","");
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



        public IActionResult Save(string op,string plant,string fyear,string Sno, string Plant_cat, string Type, string Supplier, string Qty,string Density, string Life, string CDate, string RDate, string ELife, string PCDate, string PRDate)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));

            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {

                if (op == "update")
                {
                    int i = TechnicalCommonService.UpdateRecordsTOP3(Sno, Plant_cat, Type, Supplier, Qty, Density, Life, CDate, RDate, ELife, PCDate, PRDate, fyear, plant);
                    if (i > 0)
                    {
                        List<TOP3Data> data = TechnicalCommonService.GetRecordsTOP3(fyear, plant);
                        ViewBag.records = data;
                    }
                    else if (i == -1)
                    {
                        CommonViewModel.errorMessage = "Sno,Fin Year and Plant must be unique";
                        return Json(CommonViewModel);
                    }
                }
                else
                {

                    int i = TechnicalCommonService.SaveRecordsTOP3(Sno, Plant_cat, Type, Supplier, Qty, Density, Life, CDate, RDate, ELife, PCDate, PRDate, fyear, plant);
                    if (i > 0)
                    {
                        List<TOP3Data> data = TechnicalCommonService.GetRecordsTOP3(fyear, plant);
                        ViewBag.records = data;
                    }
                    else if (i == -1)
                    {
                        CommonViewModel.errorMessage = "Sno,Fin Year and Plant must be unique ";
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




            return PartialView("_partialTOP3");
        }




        public IActionResult Execute(string plant, string fyear)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                List<TOP3Data> data = TechnicalCommonService.GetRecordsTOP3(fyear,plant);
                ViewBag.records = data;


            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialTOP3");
        }


        public IActionResult Copy(string refyear, string fyear)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
               CommonViewModel.alert = TechnicalCommonService.COPYRecordsTOP3(refyear,fyear);
               


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