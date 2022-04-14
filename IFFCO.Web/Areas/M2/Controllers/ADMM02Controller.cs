using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace IFFCO.TECHPROD.Web.Areas.M2.Controllers
{
    [Area("M2")]
    public class ADMM02Controller : BaseController<ADMM01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<ADMM01ViewModel> commonException = null;
        readonly string proj = new AppConfiguration().ProjectId;
        public ADMM02Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<ADMM01ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            primaryKeyGen = new PrimaryKeyGen();
        }
        

        // GET: M2/ADMM02
        public async Task<IActionResult> Index()
        {
            var LOV = dropDownListBindWeb.AdmSubMenuModuleLOVBind(proj);
            CommonViewModel.SubLOV = LOV;
            CommonViewModel.SelectedSubModule = LOV.FirstOrDefault().Value;
            CommonViewModel.SubList = _context.AdmSubMenuMsts.Where(x => x.Moduleid.Equals(LOV.FirstOrDefault().Value) && x.Projectid == proj).ToList();
            ViewBag.ParentLOV = dropDownListBindWeb.AdmSubMenuParentLOVBind(LOV.FirstOrDefault().Value, proj);
            return View(CommonViewModel);
        }

        public async Task<IActionResult> ViewMsts(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Model = PopulateSub(id);
            CommonViewModel = Model;
            ViewBag.ParentLOV = dropDownListBindWeb.AdmSubMenuParentLOVBind(Model.SelectedSubModule, proj);
            return View("Index", CommonViewModel);
        }

        public ADMM01ViewModel PopulateSub(string id)
        {
            ADMM01ViewModel view = new ADMM01ViewModel();
            var LOV = dropDownListBindWeb.AdmSubMenuModuleLOVBind(proj);
            view.SubLOV = LOV;
            if (id == null) { id = LOV.FirstOrDefault().Value; }
            view.SelectedSubModule = id;
            view.SubList = _context.AdmSubMenuMsts.Where(x => x.Moduleid.Equals(id) && x.Projectid == proj).ToList();            
            return view;
        }
        // GET: M2/ADMM02/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admSubMenuMsts = await _context.AdmSubMenuMsts
                .FirstOrDefaultAsync(m => m.Moduleid == id);
            if (admSubMenuMsts == null)
            {
                return NotFound();
            }

            return View(admSubMenuMsts);
        }

        // GET: M2/ADMM02/Create
        public IActionResult Create(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Model = PopulateSub(id);
            CommonViewModel = Model;
            CommonViewModel.SubMsts = new AdmSubMenuMsts() { Moduleid = id };
            CommonViewModel.Status = "Create";
            ViewBag.ParentLOV = dropDownListBindWeb.AdmSubMenuParentLOVBind(Model.SelectedSubModule, proj);
            return View("Index", CommonViewModel);
        }

        // POST: M2/ADMM02/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.SubMsts.SubMenuId) && !string.IsNullOrWhiteSpace(aDMM01ViewModel.SubMsts.ParentMenuId) && !string.IsNullOrWhiteSpace(aDMM01ViewModel.SubMsts.SubMenuName))
                {
                    aDMM01ViewModel.SubMsts.Projectid = proj;
                    _context.Add(aDMM01ViewModel.SubMsts);
                    await _context.SaveChangesAsync();
                    CommonViewModel.Message = "Sub Menu ID " + aDMM01ViewModel.SubMsts.SubMenuId + " - " + aDMM01ViewModel.SubMsts.SubMenuName;
                    CommonViewModel.Alert = "Create";
                    CommonViewModel.Status = "Create";
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

        // GET: M2/ADMM02/Edit/5
        public async Task<IActionResult> Edit(string id, string mod)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Model = PopulateSub(mod);
            CommonViewModel = Model;
            CommonViewModel.SubMsts = new AdmSubMenuMsts();
            CommonViewModel.SubMsts = _context.AdmSubMenuMsts.FirstOrDefault(x => x.SubMenuId.Equals(id) && x.Moduleid.Equals(mod) && x.Projectid.Equals(proj));
            CommonViewModel.Status = "Edit";
            ViewBag.ParentLOV = dropDownListBindWeb.AdmSubMenuParentLOVBind(Model.SelectedSubModule, proj);
            return View("Index", CommonViewModel);
        }

        // POST: M2/ADMM02/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ADMM01ViewModel aDMM01ViewModel)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aDMM01ViewModel.SubMsts.SubMenuId))
                {
                    var unitaccess = _context.AdmPrgMaster.Where(x => x.SubMenuName == aDMM01ViewModel.PrevSubMenuID)?.ToList().Count;
                    if (unitaccess == 0)
                    {
                        var upd = _context.AdmSubMenuMsts.FirstOrDefault(x => x.Moduleid.Equals(aDMM01ViewModel.SubMsts.Moduleid) && x.SubMenuId.Equals(aDMM01ViewModel.PrevSubMenuID) && x.Projectid == proj);
                        upd.SubMenuId = aDMM01ViewModel.SubMsts.SubMenuId;
                        upd.SubMenuName = aDMM01ViewModel.SubMsts.SubMenuName;
                        upd.DisplayOrder = aDMM01ViewModel.SubMsts.DisplayOrder;
                        upd.ParentMenuId = aDMM01ViewModel.SubMsts.ParentMenuId;
                        upd.Moduleid = aDMM01ViewModel.SubMsts.Moduleid;
                        _context.Update(upd);
                        await _context.SaveChangesAsync();
                        CommonViewModel.Message = "Sub-Menu No " + aDMM01ViewModel.SubMsts.SubMenuId + " - " + aDMM01ViewModel.SubMsts.SubMenuName;
                        CommonViewModel.Alert = "Update";
                        CommonViewModel.Status = "Update";
                    }
                    else
                    {
                        CommonViewModel.ErrorMessage = "Sub-Menu cannot be updated as it contains Mapped-Programs";
                        CommonViewModel.Message = "Sub-Menu cannot be updated as it contains Mapped-Programs";
                        CommonViewModel.Alert = "Warning";
                        CommonViewModel.Status = "Warning";
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

        // GET: M2/ADMM02/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admSubMenuMsts = await _context.AdmSubMenuMsts
                .FirstOrDefaultAsync(m => m.Moduleid == id);
            if (admSubMenuMsts == null)
            {
                return NotFound();
            }

            return View(admSubMenuMsts);
        }

        
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(string id, string mod)
        {
            try
            {
                var SubmenuObj = _context.AdmSubMenuMsts.Where(x => x.Moduleid == mod && x.SubMenuId == id).FirstOrDefault();
                var unitaccess = _context.AdmPrgMaster.Where(x => x.Moduleid==mod && x.SubMenuName==id)?.Count();
                if (unitaccess == 0)
                {
                    _context.AdmSubMenuMsts.Remove(SubmenuObj);
                    await _context.SaveChangesAsync();
                    CommonViewModel.ErrorMessage = "";
                    CommonViewModel.Message = "Sub Menu Id- " + Convert.ToString(id);
                    CommonViewModel.Alert = "Delete";
                    CommonViewModel.Status = "Delete";
                }
                else
                {
                    CommonViewModel.ErrorMessage = "Sub-Menu cannot be deleted as it contains Programs";
                    CommonViewModel.Message = "Sub-Menu cannot be deleted as it contains Programs";
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


        private bool AdmSubMenuMstsExists(string id)
        {
            return _context.AdmSubMenuMsts.Any(e => e.Moduleid == id);
        }
    }
}
