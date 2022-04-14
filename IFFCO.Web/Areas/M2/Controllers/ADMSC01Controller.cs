using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.HRMS.Service;
using IFFCO.HRMS.Entities.AppConfig;


namespace IFFCO.TECHPROD.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class ADMSC01Controller : BaseController<ADMSC01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;//DespatchCommonService
        TechnicalAccessRightsFunctions technicalAccessRightsFunctions = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ADMM01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ADMSC01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ADMM01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalAccessRightsFunctions = new TechnicalAccessRightsFunctions();
            primaryKeyGen = new PrimaryKeyGen();
        }

        // GET: M2/ADMSC01
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

        // GET: M2/ADMSC01/Details/5
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

        // GET: M2/ADMSC01/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: M2/ADMSC01/Create
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

        // GET: M2/ADMSC01/Edit/5
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

        // POST: M2/ADMSC01/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] ADMSC01ViewModel aDMSC01ViewModel)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(aDMSC01ViewModel.PersonnelNumber)))
                {
                    foreach (var value in aDMSC01ViewModel.ListObj)
                    {
                        var Load = _context.AdmEmpprgAccess.FirstOrDefault(x => x.Empid == aDMSC01ViewModel.PersonnelNumber.ToString() && x.Projectid == proj && x.Programid == value.Programid);                       
                        Load = Load ?? new AdmEmpprgAccess()
                        {
                            Projectid = proj,
                            Moduleid = _context.AdmPrgMaster.FirstOrDefault(x => x.Projectid == proj && x.Programid == value.Programid).Moduleid,
                            Empid = value.Empid,
                            Programid = value.Programid,
                            Programtype = _context.AdmPrgMaster.FirstOrDefault(x => x.Projectid == proj && x.Programid == value.Programid).Programtype,
                        };

                        if ((bool.Parse(value.PrivSelect) && aDMSC01ViewModel.Select == "DisplayBlock")) { Load.PrivSelect = "Y"; } else { Load.PrivSelect = "N"; }
                        if ((bool.Parse(value.PrivInsert) && aDMSC01ViewModel.Insert == "DisplayBlock")) { Load.PrivInsert = "Y"; } else { Load.PrivInsert = "N"; }
                        if ((bool.Parse(value.PrivUpdate) && aDMSC01ViewModel.Edit == "DisplayBlock")) { Load.PrivUpdate = "Y"; } else { Load.PrivUpdate = "N"; }
                        if ((bool.Parse(value.PrivDelete) && aDMSC01ViewModel.Delete == "DisplayBlock")) { Load.PrivDelete = "Y"; } else { Load.PrivDelete = "N"; }

                        if (Load.CreatedBy == null)
                        {
                            Load.CreatedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                            Load.CreatedDate = DateTime.Now;
                            _context.Add(Load);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "Rights Granted to  " + Convert.ToString(aDMSC01ViewModel.PersonnelNumber);
                            CommonViewModel.Alert = "Create";
                            CommonViewModel.Status = "Create";
                        }
                        else{
                            Load.ModifiedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                            Load.ModifiedDate = DateTime.Now;
                            _context.Update(Load);
                            await _context.SaveChangesAsync();
                            CommonViewModel.Message = "Rights Updated for " + Convert.ToString(aDMSC01ViewModel.PersonnelNumber);
                            CommonViewModel.Alert = "Update";
                            CommonViewModel.Status = "Update";
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

            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: M2/ADMSC01/Delete/5
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

        // POST: M2/ADMSC01/Delete/5
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

        public List<SelectListItem> EmpLOVBind(string Unit)
        {
            List<SelectListItem> EmpCDLOV = new List<SelectListItem>();
            EmpCDLOV = dropDownListBindWeb.GetEmpForSecurity(Unit);
            return EmpCDLOV;
        }

        public JsonResult EmpDetailBind(int Personnel)
        {           
            ADMSC01ViewModel aDMM01ViewModel = technicalAccessRightsFunctions.GetEmployeeDetailForBind(Personnel);
            return Json(aDMM01ViewModel);
        }

        public JsonResult ProgramListAccessBind(int Personnel,string ModuleID,string ProgramType,string QueryType)
        {
            var EmpAccessDetail = technicalAccessRightsFunctions.GetEmployeeAccessDetails(Personnel.ToString(), ModuleID, ProgramType, proj);
            ADMSC01ViewModel aDMM01ViewModel = new ADMSC01ViewModel()
            {
                ListObj = EmpAccessDetail,
                QueryType = QueryType

            };
            return Json(aDMM01ViewModel);
        }//ProgramListAccessBind
    }
}
