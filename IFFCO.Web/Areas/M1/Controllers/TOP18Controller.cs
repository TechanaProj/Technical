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
    public class TOP18Controller : BaseController<TOP3ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService TechnicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TOP3ViewModel> commonException = null;

        public TOP18Controller(ModelContext context)
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
                List<TOP18Data> data = TechnicalCommonService.GetRecordsTOP18("2021-22", "");
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



        public IActionResult Save(string op,string Sno, string Plant_cat, string Uom, string Pno, string APR, string MAY, string JUN, string JUL, string AUG, string SEP, string OCT, string NOV, string DEC, string JAN, string FEB, string MAR, string finyear, string plant)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));

            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            //try
            //{
            //    if (op == "update")
            //    {
            //        int i = TechnicalCommonService.UpdateRecordsTOP18(Sno, Plant_cat, Uom, Pno, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR, finyear, plant);
            //        if (i > 0)
            //        {
            //            List<TOP3Data> data = TechnicalCommonService.GetRecordsTOP3(finyear, plant);
            //            ViewBag.records = data;
            //        }
            //        else if (i == -1)
            //        {
            //            CommonViewModel.errorMessage = "Sno,Fin Year and Plant must be unique";
            //            return Json(CommonViewModel);
            //        }
            //    }
            //    int i = TechnicalCommonService.SaveRecordsTOP18(Sno, Plant_cat, Uom, Pno, APR, MAY,  JUN, JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR, finyear, plant);
            //    if (i > 0)
            //    {
            //        List<TOP18Data> data = TechnicalCommonService.GetRecordsTOP18(finyear, plant);
            //        ViewBag.records = data;
            //    }
            //    else if (i == -1)
            //    {
            //        CommonViewModel.errorMessage = "Personel No already addedd";
            //        return Json(CommonViewModel);
            //    }

            //}
            //catch (Exception ex)
            //{

            //    commonException.GetCommonExcepton(CommonViewModel, ex);
            //    CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            //    CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            //    return Json(CommonViewModel);

            //}

            try
            {

                if (op == "update")
                {
                    int i = TechnicalCommonService.UpdateRecordsTOP18(Sno, Plant_cat, Uom, Pno, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR, finyear, plant);
                    if (i > 0)
                    {
                        List<TOP18Data> data = TechnicalCommonService.GetRecordsTOP18(finyear, plant);
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

                    int i = TechnicalCommonService.SaveRecordsTOP18(Sno, Plant_cat, Uom, Pno, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR, finyear, plant);
                    if (i > 0)
                    {
                        List<TOP18Data> data = TechnicalCommonService.GetRecordsTOP18(finyear, plant);
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


            return PartialView("_partialTOP18");
        }




        public IActionResult Execute(string plant, string fyear)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                List<TOP18Data> data = TechnicalCommonService.GetRecordsTOP18(fyear, plant);
                ViewBag.records = data;


            }
            catch (Exception ex)
            {

                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);

            }




            return PartialView("_partialTOP18");
        }

    }
}
