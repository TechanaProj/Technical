using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using IFFCO.HRMS.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class ENERGYLOSSController : BaseController<ENERGYLOSSViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ENERGYLOSSViewModel> commonException = null;
        public ENERGYLOSSController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ENERGYLOSSViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
        }



        // GET: ENERGYLOSSController
        public async Task<IActionResult> Index()
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            CommonViewModel.ListRefEnergy = new List<RefEnergy>();
            CommonViewModel.ListRefEnergy = _context.RefEnergy.ToList();
            //Jquery Based Sorting . Refer Index
            //CommonViewModel.ListBagVendorMsts = _context.BagVendorMsts.OrderByDescending(x => x.Pmonth);
            return View(CommonViewModel);
        }
        public IActionResult Create()
        {
            CommonViewModel.ListRefEnergy = _context.RefEnergy.ToList();
            CommonViewModel.ObjRefEnergy = new RefEnergy();
            CommonViewModel.Status = "Create";
            return View("Index", CommonViewModel);
        }

        // POST: M1/DWGM2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ENERGYLOSSViewModel energylossViewModel)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            string PersonnelNumber = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
            try
            {
                RefEnergy refEnergy = new RefEnergy();
                refEnergy = energylossViewModel.ObjRefEnergy;
                var xy = ModelState.IsValid && _context.RefEnergy.Where(y => y.PlantCode !=energylossViewModel.ObjRefEnergy.PlantCode).Any();
                if (ModelState.IsValid && _context.RefEnergy.Where(x => x.PlantCode != energylossViewModel.ObjRefEnergy.PlantCode).Any())
                {
                    refEnergy.CreationDatetime = DateTime.Now;
                    refEnergy.CreatedBy = Convert.ToDecimal(PersonnelNumber);
                    refEnergy.ModifiedBy = Convert.ToDecimal(PersonnelNumber);
                    _context.RefEnergy.Add(refEnergy);
                    await _context.SaveChangesAsync();

                    CommonViewModel.ListRefEnergy = new List<RefEnergy>();
                    CommonViewModel.ListRefEnergy = _context.RefEnergy.ToList();
                    CommonViewModel.Message = refEnergy.PlantCode;
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Plant code already  Exists";
                    CommonViewModel.ErrorMessage = "Plant Code alreadyRate Exists";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }
            //CommonViewModel.IsAlertBox = true;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: M1/DWGM2/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            CommonViewModel.ListRefEnergy = _context.RefEnergy.ToList();
            CommonViewModel.ObjRefEnergy = _context.RefEnergy.FirstOrDefault(x => x.PlantCode.Equals(id));
            CommonViewModel.Status = "Edit";
            return View("Index", CommonViewModel);
        }

        // POST: M1/DWGM2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ENERGYLOSSViewModel energylossViewModel)
        {
            RefEnergy refEnergy = new RefEnergy();
            refEnergy = energylossViewModel.ObjRefEnergy;
            decimal PersonnelNumber = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            try
            {
                var Obj = _context.RefEnergy.FirstOrDefault (x=>x.PlantCode.Equals(refEnergy.PlantCode));
                //if (_context.CasualEsiRateMsts.Any(x => x.UnitCode == unit && x.RateCode.Equals(casualEsiRateMsts.RateCode)))
                if (Obj != null)
                {
                    Obj.ModifiedBy = PersonnelNumber;
                    Obj.ModifiedDatetime = DateTime.Now;
                    // Obj is the data from databse while casEsi Rate is from View
                    Obj.PlantCode = refEnergy.PlantCode;
                    Obj.Plant= refEnergy.Plant;
                    Obj.ReEnergy = refEnergy.ReEnergy;
       
                    _context.RefEnergy.Update(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message =refEnergy.PlantCode;
                    //CommonViewModel.ErrorMessage = BagVendorMsts.Pmonth;
                    CommonViewModel.Alert = "Update";
                    CommonViewModel.Alert = "Update";
                    CommonViewModel.ErrorMessage = "";
                }
                else
                {
                    CommonViewModel.Message = "Update isnt  possible. Please check";
                    CommonViewModel.ErrorMessage = "Update isnt  possible. Please check";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }
            CommonViewModel.IsAlertBox = true;
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);

        }

        // GET: M1/DWGM2/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refenergy = await _context.RefEnergy
                .FirstOrDefaultAsync(m => m.PlantCode == id);
            if (refenergy == null)
            {
                return NotFound();
            }

            return View(refenergy);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            int unit = Convert.ToInt32(HttpContext.Session.GetString("UnitCode"));
            try
            {
                var Obj = _context.RefEnergy.FirstOrDefault(x =>  x.PlantCode.Equals(id));

                if (Obj != null)
                {
                    _context.RefEnergy.Remove(Obj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.ErrorMessage = "";
                    CommonViewModel.Message = Obj.PlantCode.ToString();
                    CommonViewModel.Alert = "Delete";
                    CommonViewModel.Status = "Delete";
                }
                else
                {
                    CommonViewModel.ErrorMessage = "Delete couldnt be initiated. Please check";
                    CommonViewModel.Message = "Delete couldnt be initiated. Please check";
                    CommonViewModel.Alert = "Warning";
                    CommonViewModel.Status = "Warning";
                }

                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();

            }
            return Json(CommonViewModel);
        }
    }
}