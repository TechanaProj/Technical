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
    public class ADMM01Controller : BaseController<ADMM01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly TechnicalAccessRightsFunctions technicalAccessRightsFunctions = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ADMM01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ADMM01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ADMM01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
            technicalAccessRightsFunctions = new TechnicalAccessRightsFunctions();
        }

        

        // GET: M2/ADMM01
        public async Task<IActionResult> Index()
        {
            CommonViewModel = PopulateCommonViewModel(null, proj);
            if (Convert.ToString(TempData["EditChild"]) != "")
            {
                if (Convert.ToString(TempData["moduleid"]) != "" && Convert.ToString(TempData["ProjId"]) != "")
                {
                    CommonViewModel.Status = "ViewMsts";
                    ViewBag.Message = "";
                    var modulid = Convert.ToString(TempData["moduleid"]);
                    var projid = Convert.ToString(TempData["ProjId"]);
                    CommonViewModel.ListItem = _context.AdmPrgMaster.Where(x => x.Moduleid.Equals(modulid) && x.Projectid.Equals(projid)).ToList();
                }

            }

            return View(CommonViewModel);
        }

        public async Task<IActionResult> ViewMsts(string id, string ProjId)
        {
            if (id == null)
            {
                return NotFound();
            }
            //DropDownListBind drp = new DropDownListBind();
            string ModuleId = HttpContext.Session.GetString("ModuleID");
            CommonViewModel = PopulateCommonViewModel(id, ProjId);
            ViewBag.ModuleID = id;
            TempData["moduleid"] = id;
            TempData["ProjId"] = ProjId;

            CommonViewModel.Status = "ViewMsts";

            //if (CommonViewModel.admProjmodMaster == null)
            //{
            //    return NotFound();
            //}
            return View("Index", CommonViewModel);
        }

        // GET: M2/ADMM01/Details/5
        public async Task<IActionResult> Create(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommonViewModel = PopulateCommonViewModel(id, proj);
            CommonViewModel.Status = "Create";
            return View("Index",CommonViewModel);
        }

        // GET: M2/ADMM01/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admProjmodMaster = await _context.AdmProjmodMaster
                .FirstOrDefaultAsync(m => m.Projectid == id);
            if (admProjmodMaster == null)
            {
                return NotFound();
            }

            return View(admProjmodMaster);
        }

        // GET: M2/ADMM01/Create
        public ADMM01ViewModel PopulateCommonViewModel(string id, string ProjId)
        {
            ADMM01ViewModel view = new ADMM01ViewModel();
            view.List = _context.AdmProjmodMaster.Where(x => x.Projectid == ProjId).ToList();
            if (id == null)
            {
                id = _context.AdmProjmodMaster.FirstOrDefault(x => x.Projectid == ProjId).Moduleid;
            }
            view.SelectedMod = id;
            view.ListItem = _context.AdmPrgMaster.Where(x => x.Projectid == ProjId && x.Moduleid == id)?.ToList();
            return view;
        }

        // POST: M2/ADMM01/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.Msts.Moduleid))
                {
                    AdmProjmodMaster Obj = new AdmProjmodMaster
                    {
                        Projectid = proj,
                        Moduleid = aDMM01ViewModel.Msts.Moduleid,
                        Modulename = aDMM01ViewModel.Msts.Modulename,
                        CreatedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID")),
                        CreatedDate = DateTime.Now,
                        GstCompliant = "N",
                        Migrated = "Y"
                    };
                    _context.Add(Obj);
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }


            await _context.SaveChangesAsync();
            CommonViewModel.Message = "Module ID " + aDMM01ViewModel.Msts.Moduleid + " - " + aDMM01ViewModel.Msts.Modulename;
            CommonViewModel.Alert = "Create";
            CommonViewModel.Status = "Create";

            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMsts(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.MstsItem.Programid))
                {
                    AdmPrgMaster Obj = new AdmPrgMaster
                    {
                        Projectid = proj,
                        Moduleid = aDMM01ViewModel.MstsItem.Moduleid,
                        Programid = aDMM01ViewModel.MstsItem.Programid,
                        Programname = aDMM01ViewModel.MstsItem.Programname,
                        Programtype = aDMM01ViewModel.MstsItem.Programtype,
                        CreatedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID")),
                        CreatedDate = DateTime.Now,
                        SubMenuName = aDMM01ViewModel.MstsItem.SubMenuName,
                        DisplayOrder = aDMM01ViewModel.MstsItem.DisplayOrder,
                        ActiveInactive = aDMM01ViewModel.MstsItem.ActiveInactive
                    };
                    _context.Add(Obj);
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }


            await _context.SaveChangesAsync();
            CommonViewModel.Message = "Program ID " + aDMM01ViewModel.MstsItem.Programid + " - " + aDMM01ViewModel.MstsItem.Programname;
            CommonViewModel.Alert = "Create";
            CommonViewModel.Status = "Create";

            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: M2/ADMM01/Edit/5
        public async Task<IActionResult> Edit(string id, string ProjId)
        {
            if (id == null)
            {
                return NotFound();
            }

            CommonViewModel = PopulateCommonViewModel(id, proj);
            CommonViewModel.Status = "edit";
            CommonViewModel.Msts = _context.AdmProjmodMaster.FirstOrDefault(x => x.Moduleid.Equals(id) && x.Projectid.Equals(ProjId));
            return View("Index", CommonViewModel);

        }

        // POST: M2/ADMM01/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.Msts.Moduleid))
                {
                    var upd = _context.AdmProjmodMaster.FirstOrDefault(x => x.Moduleid.Equals(aDMM01ViewModel.Msts.Moduleid) && x.Projectid == proj);
                    upd.Modulename = aDMM01ViewModel.Msts.Modulename;
                    upd.ModifiedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                    upd.ModifiedDate = DateTime.Now;
                    _context.Update(upd);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }


            await _context.SaveChangesAsync();
            CommonViewModel.Message = "Module No " + aDMM01ViewModel.Msts.Moduleid + " - "+ aDMM01ViewModel.Msts.Modulename;
            CommonViewModel.Alert = "Update";
            CommonViewModel.Status = "Update";

            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMsts(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.MstsItem.Moduleid))
                {
                    var upd = _context.AdmPrgMaster.FirstOrDefault(x => x.Moduleid.Equals(aDMM01ViewModel.MstsItem.Moduleid) && x.Projectid == proj && x.Programid.Equals(aDMM01ViewModel.MstsItem.Programid));
                    upd.Programname = aDMM01ViewModel.MstsItem.Programname;
                    upd.Programtype = aDMM01ViewModel.MstsItem.Programtype;
                    upd.DisplayOrder = aDMM01ViewModel.MstsItem.DisplayOrder;
                    upd.ActiveInactive = aDMM01ViewModel.MstsItem.ActiveInactive;
                    upd.SubMenuName = aDMM01ViewModel.MstsItem.SubMenuName;
                    upd.ModifiedBy = Convert.ToString(HttpContext.Session.GetInt32("EmpID"));
                    upd.ModifiedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                commonException.GetCommonExcepton(CommonViewModel, ex);
                CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
                CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
                return Json(CommonViewModel);
            }


            await _context.SaveChangesAsync();
            CommonViewModel.Message = "Program ID " + aDMM01ViewModel.MstsItem.Programid + " - " + aDMM01ViewModel.MstsItem.Programname;
            CommonViewModel.Alert = "Update";
            CommonViewModel.Status = "Update";

            CommonViewModel.ErrorMessage = "";
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            return Json(CommonViewModel);
        }

        // GET: M2/ADMM01/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admProjmodMaster = await _context.AdmProjmodMaster
                .FirstOrDefaultAsync(m => m.Projectid == id);
            if (admProjmodMaster == null)
            {
                return NotFound();
            }

            return View(admProjmodMaster);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(string id, string ProjId)
        {
            try
            {
                var admPrgMaster = _context.AdmProjmodMaster.Where(x => x.Moduleid.Equals(id) && x.Projectid.Equals(ProjId)).FirstOrDefault();
                var unitaccess = _context.AdmEmpUnitAccess.Where(x => x.Moduleid.Equals(id) && x.Projectid.Equals(ProjId))?.Count();
                if (unitaccess == 0)
                {
                    _context.AdmProjmodMaster.Remove(admPrgMaster);
                    await _context.SaveChangesAsync();
                    CommonViewModel.ErrorMessage = "";
                    CommonViewModel.Message = "Module Id- " + Convert.ToString(id);
                    CommonViewModel.Alert = "Delete";
                    CommonViewModel.Status = "Delete";
                }
                else
                {
                    CommonViewModel.ErrorMessage = "Module cannot be deleted as it contains Access Grants";
                    CommonViewModel.Message = "Module cannot be deleted as it contains Access Grants";
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

        public async Task<IActionResult> DeleteConfirmedChild(string id, string ProjId, string PrjType, string ProgId)
        {
            try
            {
                
                var admPrgMaster = _context.AdmPrgMaster.Where(x => x.Moduleid.Equals(id) && x.Projectid.Equals(ProjId) && x.Programtype.Equals(PrjType) && x.Programid.Equals(ProgId)).FirstOrDefault();
                var prgaccess = _context.AdmEmpprgAccess.Where(x => x.Moduleid.Equals(id) && x.Projectid.Equals(ProjId) && x.Programid.Equals(ProgId))?.Count();

                if (prgaccess == 0)
                {
                    _context.AdmPrgMaster.Remove(admPrgMaster);
                    await _context.SaveChangesAsync();
                    CommonViewModel.ErrorMessage = "";
                    CommonViewModel.Message = "Program Id- " + Convert.ToString(ProgId);
                    CommonViewModel.Alert = "Delete";
                    CommonViewModel.Status = "Delete";
                }
                else
                {
                    CommonViewModel.ErrorMessage = "Program cannot be deleted as it contains Access Grants";
                    CommonViewModel.Message = "Program cannot be deleted as it contains Access Grants";
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

        private bool AdmProjmodMasterExists(string id)
        {
            return _context.AdmProjmodMaster.Any(e => e.Projectid == id);
        }

        //SubMenuBind
        public List<SelectListItem> SubMenuBind(string SelectedMod)
        {
            var results = dropDownListBindWeb.AdmPrgParentLOVBind(SelectedMod, proj);
            List<SelectListItem> productCDLOV = new List<SelectListItem>();
            productCDLOV = results;
            return productCDLOV;
        }

        public JsonResult EditListBind(string ProgramId, string SelectedMod)
        {
            var results = _context.AdmPrgMaster.FirstOrDefault(x => x.Moduleid.Equals(SelectedMod) && x.Programid.Equals(ProgramId) && x.Projectid == proj);
            return Json(results);
        }
    }
}
