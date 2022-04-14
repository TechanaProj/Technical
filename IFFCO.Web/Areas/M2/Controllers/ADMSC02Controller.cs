using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Service;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Areas.M2.Controllers
{

    [Area("M2")]
    public class ADMSC02Controller : BaseController<ADMSC02ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;//DespatchCommonService
        private readonly TechnicalAccessRightsFunctions technicalAccessRightsFunctions = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ADMSC02ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ADMSC02Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ADMSC02ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalAccessRightsFunctions = new TechnicalAccessRightsFunctions();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: M2/ADMSC02
        public async Task<IActionResult> Index()
        {

            return View(CommonViewModel);
        }

        // GET: M2/ADMSC02/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpUnitAccess = await _context.AdmEmpUnitAccess
                .FirstOrDefaultAsync(m => m.Empid == id);
            if (admEmpUnitAccess == null)
            {
                return NotFound();
            }

            return View(admEmpUnitAccess);
        }

        // GET: M2/ADMSC02/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: M2/ADMSC02/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Empid,Moduleid,UnitCode,DefaultUnit,HierYn,AllDeptAccess,AllSectionAccess,CreatedBy,DatetimeCreated,ModifiedBy,DatetimeModified,OnlyAreaAccess,Projectid")] AdmEmpUnitAccess admEmpUnitAccess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admEmpUnitAccess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admEmpUnitAccess);
        }

        // GET: M2/ADMSC02/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpUnitAccess = await _context.AdmEmpUnitAccess.FindAsync(id);
            if (admEmpUnitAccess == null)
            {
                return NotFound();
            }
            return View(admEmpUnitAccess);
        }

        // POST: M2/ADMSC02/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] ADMSC02ViewModel aDMSC02ViewModel)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(aDMSC02ViewModel.PersonnelNumber)))
                {
                    var Personnel = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                    //List<string> termsList = new List<string>();
                    //foreach (var item in aDMSC02ViewModel.ListPlant)
                    //{
                    //    if (item.Key == "AN0") { termsList.Add("3"); }
                    //    else if (item.Key == "PH0") { termsList.Add("5"); }
                    //    else if (item.Key == "KD0") { termsList.Add("6"); }
                    //    else if (item.Key == "KL0") { termsList.Add("4"); }
                    //    else if (item.Key == "PD0") { termsList.Add("835"); }
                    //}
                    //int PlantAcess = despatchCommonService.DespPlantAccess(aDMSC02ViewModel.PersonnelNumber.ToString(), string.Join(",", termsList), Personnel);

                    foreach (var value in aDMSC02ViewModel.ListModule)
                    {
                        if (value.Value)
                        foreach(var unit in aDMSC02ViewModel.ListObj?.Where(x=>x.DefaultUnit.Equals("Y")))
                        {
                            var UnitCheck = _context.AdmEmpUnitAccess.FirstOrDefault(x=>x.Empid.Equals(aDMSC02ViewModel.PersonnelNumber) && x.UnitCode.Equals(unit.UnitCode) && x.Moduleid.Equals(value.Key) && x.Projectid.Equals(proj));
                            if (UnitCheck != null)
                            {
                                UnitCheck.DefaultUnit = unit.DefaultUnit;
                                UnitCheck.HierYn = unit.HierYn;
                                UnitCheck.OnlyAreaAccess = unit.OnlyAreaAccess;
                                UnitCheck.AllDeptAccess = unit.AllDeptAccess;
                                UnitCheck.AllSectionAccess = unit.AllSectionAccess;
                                UnitCheck.ModifiedBy = Personnel;
                                UnitCheck.DatetimeModified = DateTime.Now;
                                _context.Update(UnitCheck);
                                CommonViewModel.Alert = "Update";
                                CommonViewModel.Status = "Update";
                                }
                            else
                            {
                                UnitCheck = new AdmEmpUnitAccess
                                {
                                    Empid = unit.Empid,
                                    UnitCode = unit.UnitCode,
                                    Moduleid = value.Key,
                                    DefaultUnit = unit.DefaultUnit,
                                    HierYn = unit.HierYn,
                                    OnlyAreaAccess = unit.OnlyAreaAccess,
                                    AllDeptAccess = unit.AllDeptAccess,
                                    AllSectionAccess = unit.AllSectionAccess,
                                    CreatedBy = Personnel,
                                    DatetimeCreated = DateTime.Now,
                                    Projectid = proj
                                };
                                _context.Add(UnitCheck);
                                    CommonViewModel.Alert = "Create";
                                    CommonViewModel.Status = "Create";

                                }
                                await _context.SaveChangesAsync();
                            }
                        else
                        {
                            var DelData = _context.AdmEmpUnitAccess.Where(x => x.Empid.Equals(aDMSC02ViewModel.PersonnelNumber) && x.Moduleid.Equals(value.Key) && x.Projectid.Equals(proj)).ToList();
                            foreach (var del in DelData)
                            {
                                _context.AdmEmpUnitAccess.Remove(del);
                            }                           
                            await _context.SaveChangesAsync();

                        }
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

            CommonViewModel.Message = "Unit & Plant Access for" + aDMSC02ViewModel.PersonnelNumber;
            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: M2/ADMSC02/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpUnitAccess = await _context.AdmEmpUnitAccess
                .FirstOrDefaultAsync(m => m.Empid == id);
            if (admEmpUnitAccess == null)
            {
                return NotFound();
            }

            return View(admEmpUnitAccess);
        }

        // POST: M2/ADMSC02/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admEmpUnitAccess = await _context.AdmEmpUnitAccess.FindAsync(id);
            _context.AdmEmpUnitAccess.Remove(admEmpUnitAccess);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdmEmpUnitAccessExists(int id)
        {
            return _context.AdmEmpUnitAccess.Any(e => e.Empid == id);
        }

        public List<SelectListItem> UnitLOVBind()
        {
            List<SelectListItem> unitLOV = new List<SelectListItem>();
            string ModuleId = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string PersonnelNumber = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
            unitLOV = dropDownListBindWeb.GetUnitWithDespatchSecurity(PersonnelNumber, ModuleId);
            return unitLOV;
        }

        public List<SelectListItem> EmpLOVBind(string Unit)
        {
            List<SelectListItem> EmpCDLOV = new List<SelectListItem>();
            EmpCDLOV = dropDownListBindWeb.GetEmpForSecurity(Unit);
            return EmpCDLOV;
        }

        public JsonResult EmpDetailBind(int Personnel)
        {
           // string PersonnelNumber = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
            var Empdetail = technicalAccessRightsFunctions.GetEmployeeDetailForBind(Personnel);
            ADMSC02ViewModel aDMM02ViewModel = new ADMSC02ViewModel()
            {
                PersonnelNumber = Personnel,
                EmployeeName = Empdetail.EmployeeName,
                Department = Empdetail.Department,
                Designation = Empdetail.Designation,
                Section = Empdetail.Section,
                Grade = Empdetail.Grade,
                Unit = Empdetail.Unit.ToString()
            };
           
            aDMM02ViewModel.ListModule = technicalAccessRightsFunctions.GetModuleKeyValue(Personnel.ToString());
            var obj = technicalAccessRightsFunctions.GetUnitAccessDetail(Personnel.ToString(), proj);
            obj = obj ?? new List<AdmEmpUnitAccess>();
            if (obj.Count == 0)
            {
                obj.Add(new AdmEmpUnitAccess()
                {
                    Empid = Personnel,
                    UnitCode = Convert.ToInt32(technicalAccessRightsFunctions.GetEmployeeDetailForBind(Personnel).Unit),
                    DefaultUnit = "Y",
                    HierYn = "N",
                    OnlyAreaAccess = "N",
                    AllDeptAccess = "Y",
                    AllSectionAccess = "Y",
                    Projectid = proj,
                    CreatedBy = "New"
                    
                });
            }
            aDMM02ViewModel.ListObj = obj;
            return Json(aDMM02ViewModel);
        }
    }
}
