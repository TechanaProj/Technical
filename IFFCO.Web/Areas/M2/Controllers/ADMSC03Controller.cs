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

namespace IFFCO.DAILYWG.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class ADMSC03Controller : BaseController<ADMSC03ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;//DespatchCommonService
        private readonly TechnicalAccessRightsFunctions technicalAccessRightsFunctions = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ADMSC03ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ADMSC03Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ADMSC03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalAccessRightsFunctions = new TechnicalAccessRightsFunctions();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: M2/ADMSC03
        public async Task<IActionResult> Index()
        {
            string UnitCode = Convert.ToString(HttpContext.Session.GetString("UnitCode"));
            string ModuleId = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            string PersonnelNumber = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
            var unitLOV = dropDownListBindWeb.GetUnitWithDespatchSecurity(PersonnelNumber, ModuleId);
            CommonViewModel.UnitLOV = unitLOV;
            CommonViewModel.ModuleLOV = dropDownListBindWeb.AdmSubMenuModuleLOVBind(proj);
            return View(CommonViewModel);
        }

        // GET: M2/ADMSC03/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpprgAccess = await _context.AdmEmpprgAccess
                .FirstOrDefaultAsync(m => m.Empid == id);
            if (admEmpprgAccess == null)
            {
                return NotFound();
            }

            return View(admEmpprgAccess);
        }

        // GET: M2/ADMSC03/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: M2/ADMSC03/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Empid,Projectid,Moduleid,Programid,PrivSelect,PrivInsert,PrivUpdate,PrivDelete,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Programtype")] AdmEmpprgAccess admEmpprgAccess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admEmpprgAccess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admEmpprgAccess);
        }

        // GET: M2/ADMSC03/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpprgAccess = await _context.AdmEmpprgAccess.FindAsync(id);
            if (admEmpprgAccess == null)
            {
                return NotFound();
            }
            return View(admEmpprgAccess);
        }

        // POST: M2/ADMSC03/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Empid,Projectid,Moduleid,Programid,PrivSelect,PrivInsert,PrivUpdate,PrivDelete,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Programtype")] AdmEmpprgAccess admEmpprgAccess)
        {
            if (id != admEmpprgAccess.Empid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admEmpprgAccess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdmEmpprgAccessExists(admEmpprgAccess.Empid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admEmpprgAccess);
        }

        // GET: M2/ADMSC03/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admEmpprgAccess = await _context.AdmEmpprgAccess
                .FirstOrDefaultAsync(m => m.Empid == id);
            if (admEmpprgAccess == null)
            {
                return NotFound();
            }

            return View(admEmpprgAccess);
        }

        // POST: M2/ADMSC03/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admEmpprgAccess = await _context.AdmEmpprgAccess.FindAsync(id);
            _context.AdmEmpprgAccess.Remove(admEmpprgAccess);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdmEmpprgAccessExists(string id)
        {
            return _context.AdmEmpprgAccess.Any(e => e.Empid == id);
        }

        public JsonResult ProgramUnitPlantListBind(int Unit, string ModuleID, string ProgramID)
        {
            var EmpAccessDetail = technicalAccessRightsFunctions.GetAllEmployeeAccessDetails(Unit,ModuleID, ProgramID, proj);
            var EmpPlantAccessDetail = new List<PlantTableForBind>();
            var EmpUnitAccessDetail = new List<AdmEmpUnitAccess>();
            if (EmpAccessDetail.Any())
            {             
                EmpUnitAccessDetail  = technicalAccessRightsFunctions.GetUnitAccessDetail(EmpAccessDetail.OrderBy(c => c.PersonnelNo).FirstOrDefault().PersonnelNo.ToString(), proj);
            }

            ADMSC03ViewModel aDMM03ViewModel = new ADMSC03ViewModel()
            {
                ListObj = EmpAccessDetail,
                ObjDetail = EmpUnitAccessDetail,
                ObjPlantDetail = EmpPlantAccessDetail,
                SelectedModule = ModuleID,
                SelectedProgram = ProgramID,
                UnitCD = Unit.ToString()
            };
            return Json(aDMM03ViewModel);
        }//ProgramListAccessBind       

        public List<SelectListItem> ProgramLOVBind(string Module)
        {
            List<SelectListItem> SubMenuCDLOV = new List<SelectListItem>();
            SubMenuCDLOV = _context.AdmPrgMaster.Where(x => x.Projectid == proj && x.Moduleid.Equals(Module)).Select(x => new SelectListItem
            {
                Text = string.Concat(x.Programid, " - ", x.Programname),
                Value = x.Programid.ToString()
            }).ToList();
            return SubMenuCDLOV;
        }

        public JsonResult UnitPlantListBind(string PersonnelNo)
        {          
            var EmpUnitAccessDetail = new List<AdmEmpUnitAccess>();
            if (PersonnelNo.Length==6)
            {
               
                EmpUnitAccessDetail = technicalAccessRightsFunctions.GetUnitAccessDetail(PersonnelNo, proj);
            }

            ADMSC03ViewModel aDMM03ViewModel = new ADMSC03ViewModel()
            {
                ObjDetail = EmpUnitAccessDetail            
            };
            return Json(aDMM03ViewModel);
        }//ProgramListAccessBind 

        


    }
}
