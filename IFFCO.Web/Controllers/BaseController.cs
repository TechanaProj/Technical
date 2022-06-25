using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Service;
using IFFCO.HRMS.Shared.CommonFunction;
using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace IFFCO.TECHPROD.Web.Controllers
{
    public class BaseController<T> : Controller
    {
        CommonService commonService = null;
        public AccountService accountService = null;
        DropDownListBindWeb dropDownListBind = null;
        public dynamic CommonViewModel = default(T);
        string controllerName = string.Empty;
        private int EMP_ID = 0;
        private IHttpContextAccessor _accessor;
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {

                CommonViewModel.Delete = "DisplayNone";
                CommonViewModel.Insert = "DisplayNone";
                CommonViewModel.Edit = "DisplayNone";
                CommonViewModel.Select = "DisplayNone";

                EMP_ID = Convert.ToInt32(context.HttpContext.Session.GetInt32("EmpID"));
                var url = this.HttpContext.Request.GetDisplayUrl();
                var index = url.IndexOf(this.HttpContext.Request.Path.Value);
                var newurl = url.Substring(0, index);

                if (EMP_ID == 0)
                {
                    HttpContext.Session.Clear();
                    
                    context.Result = new RedirectResult(newurl);
                    return;
                }
                else if (EMP_ID != 0)// && controllerName!="")
                {
                    var AuthorizedData = new CommonService().GetModulesMenus(EMP_ID).Where(x => x.MenuId == controllerName).FirstOrDefault();
                    if (AuthorizedData != null)
                    {
                        CommonViewModel.Delete = AuthorizedData.PrivDelete != "N" ? "DisplayBlock" : "DisplayNone";
                        CommonViewModel.Insert = AuthorizedData.PrivInsert != "N" ? "DisplayBlock" : "DisplayNone";
                        CommonViewModel.Edit = AuthorizedData.PrivUpdate != "N" ? "DisplayBlock" : "DisplayNone";
                        CommonViewModel.Select = AuthorizedData.PrivSelect != "N" ? "DisplayBlock" : "DisplayNone";
                        CommonViewModel.SelectedMenu = AuthorizedData.MenuId;
                        CommonViewModel.MenuName = AuthorizedData.MenuName;
                    }
                }
            }
            catch (Exception ex)
            {
                EMP_ID = 0;
            }
            // Do something before the action executes.
        }

        public BaseController()
        {
            controllerName = this.GetType().Name.Replace("Controller", "").Trim();
            CommonViewModel = (dynamic)Activator.CreateInstance(typeof(T));
            accountService = new AccountService();
            commonService = new CommonService();
            dropDownListBind = new DropDownListBindWeb();
        }

        public BaseController(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
            controllerName = this.GetType().Name.Replace("Controller", "").Trim();            
            accountService = new AccountService();
            commonService = new CommonService();            
        }

        public async Task<IActionResult> GetMouldeResult(string ModuleId)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            //string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            IntializeSessoin(ModuleId, EMP_ID, 0, "M", HttpContext.Session.GetString("ProjectId"));
            var obj = commonService.GetModules(EMP_ID).FirstOrDefault(x => x.Value == HttpContext.Session.GetString("ModuleID"));
            if (obj != null) obj.Selected = true;
            var Menues = commonService.GetModulesMenus(EMP_ID).Where(x => x.Menulevel == 1).ToList();
            var Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), ModuleId);
            //var FilterMenues= commonService.GetModulesMenus("MASTERS_MENU");
            HttpContext.Session.SetObject("GetModulesMenus", Menues);
            HttpContext.Session.SetObject("GetUnitDropDown", Units);
            HttpContext.Session.SetObject("GetFilterMenues", null);
            LeftMenuViewModel leftMenuViewModel = new LeftMenuViewModel
            {
                Modules = commonService.GetModules(EMP_ID),
                ModulesMenu = Menues,
                FilterModulesMenu = null,//FilterMenues,
                Units = Units
            };

            //string Emp_ID2 = Request.Cookies["EmpID2"].ToString();
            //return Json(leftMenuViewModel);
            return RedirectToAction("Index", "Home", new { area = ModuleId });
        }
        public string GetUnitResult(int UnitId)
        {
            string Status = string.Empty;
            try
            {
                int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                string ModuleId = HttpContext.Session.GetString("ModuleID");
                IntializeSessoin(ModuleId, EMP_ID, UnitId, "U", HttpContext.Session.GetString("ProjectId"));
                Status = "Success";
            }
            catch (Exception ex)
            {
                Status = "Fail";
            }

            //string Emp_ID2 = Request.Cookies["EmpID2"].ToString();
            return Status;
        }

        public void IntializeSessoin(string ModuleID, float EMPID, float UnitCode, string EVENT, string ProjectId)
        {
            UserModuleUnitModel UserModuleUnitModel1 = new UserModuleUnitModel
            {
                ModuleID = ModuleID,
                EMPID = EMPID,
                UnitCode = UnitCode,
                EVENT = EVENT,
                ProjectId = ProjectId
            };

            var Pcglobalvalues = accountService.Pcglobalvalues(UserModuleUnitModel1);
            if (_accessor == null)
            {
                HttpContext.Session.SetInt32("EmpID", Convert.ToInt32(EMPID));
                HttpContext.Session.SetString("ModuleID", ModuleID);
                HttpContext.Session.SetString("UnitCode", Pcglobalvalues.UnitCode.ToString());
                HttpContext.Session.SetString("UnitDescription", Pcglobalvalues.UnitDescription.ToString());
                HttpContext.Session.SetString("UnitType", Pcglobalvalues.UnitType.ToString());
                HttpContext.Session.SetString("ProcessUnitCode", Pcglobalvalues.ProcessUnitCode.ToString());
                HttpContext.Session.SetString("OrcUnitcode", Pcglobalvalues.OrcUnitcode.ToString());
                HttpContext.Session.SetString("AreaUnitCode", Pcglobalvalues.AreaUnitCode.ToString());
                HttpContext.Session.SetString("EmployeeName", Pcglobalvalues.EmployeeName.ToString());
                HttpContext.Session.SetString("WorkUnit", Pcglobalvalues.WorkUnit.ToString());
                HttpContext.Session.SetString("AllDeptAccess", Pcglobalvalues.AllDeptAccess.ToString());
                HttpContext.Session.SetString("AllSecAccess", Pcglobalvalues.AllSecAccess.ToString());
                HttpContext.Session.SetString("HierYn", Pcglobalvalues.HierYn.ToString());
                HttpContext.Session.SetString("ModuleName", Pcglobalvalues.ModuleName.ToString());
                HttpContext.Session.SetString("StatusCode", Pcglobalvalues.StatusCode.ToString());
                HttpContext.Session.SetString("ErrorCode", Pcglobalvalues.ErrorCode.ToString());
                HttpContext.Session.SetString("ErrorMessage", Pcglobalvalues.ErrorMessage.ToString());
            }
            else
            {
                _accessor.HttpContext.Session.SetInt32("EmpID", Convert.ToInt32(EMPID));
                _accessor.HttpContext.Session.SetString("ModuleID", ModuleID);
                _accessor.HttpContext.Session.SetString("UnitCode", Pcglobalvalues.UnitCode.ToString());
                _accessor.HttpContext.Session.SetString("UnitDescription", Pcglobalvalues.UnitDescription.ToString());
                _accessor.HttpContext.Session.SetString("UnitType", Pcglobalvalues.UnitType.ToString());
                _accessor.HttpContext.Session.SetString("ProcessUnitCode", Pcglobalvalues.ProcessUnitCode.ToString());
                _accessor.HttpContext.Session.SetString("OrcUnitcode", Pcglobalvalues.OrcUnitcode.ToString());
                _accessor.HttpContext.Session.SetString("AreaUnitCode", Pcglobalvalues.AreaUnitCode.ToString());
                _accessor.HttpContext.Session.SetString("EmployeeName", Pcglobalvalues.EmployeeName.ToString());
                _accessor.HttpContext.Session.SetString("WorkUnit", Pcglobalvalues.WorkUnit.ToString());
                _accessor.HttpContext.Session.SetString("AllDeptAccess", Pcglobalvalues.AllDeptAccess.ToString());
                _accessor.HttpContext.Session.SetString("AllSecAccess", Pcglobalvalues.AllSecAccess.ToString());
                _accessor.HttpContext.Session.SetString("HierYn", Pcglobalvalues.HierYn.ToString());
                _accessor.HttpContext.Session.SetString("ModuleName", Pcglobalvalues.ModuleName.ToString());
                _accessor.HttpContext.Session.SetString("StatusCode", Pcglobalvalues.StatusCode.ToString());
                _accessor.HttpContext.Session.SetString("ErrorCode", Pcglobalvalues.ErrorCode.ToString());
                _accessor.HttpContext.Session.SetString("ErrorMessage", Pcglobalvalues.ErrorMessage.ToString());
            }

        }
        public async Task<IActionResult> GetMenuResult(string MenuId)
        {
            LeftMenuViewModel leftMenuViewModel = null;
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            var obj = commonService.GetModules(EMP_ID).FirstOrDefault(x => x.Value == HttpContext.Session.GetString("ModuleID"));
            if (obj != null) obj.Selected = true;
            List<ModulesMenu> Menues = commonService.GetModulesMenus(EMP_ID).Where(x => x.Menulevel == 1).ToList();
            List<ModulesMenu> FilterMenues = commonService.GetModulesMenus(MenuId, EMP_ID);
            List<SelectListItem> Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);
            if (Menues.Count > 0 && Units.Count > 0 && FilterMenues.Count > 0)
            {
                HttpContext.Session.SetObject("GetModulesMenus", Menues);
                HttpContext.Session.SetObject("GetUnitDropDown", Units);
                HttpContext.Session.SetObject("GetFilterMenues", FilterMenues);
                leftMenuViewModel = new LeftMenuViewModel
                {
                    Modules = commonService.GetModules(EMP_ID),
                    ModulesMenu = Menues,
                    FilterModulesMenu = FilterMenues,
                    Units = Units
                };
            }
            else
            {
                IntializeSessoin(moduleid, EMP_ID, 0, "M", HttpContext.Session.GetString("ProjectId"));

                Menues = commonService.GetModulesMenus(EMP_ID).Where(x => x.Menulevel == 1).ToList();
                FilterMenues = commonService.GetModulesMenus(MenuId, EMP_ID);
                Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);

                HttpContext.Session.SetObject("GetModulesMenus", Menues);
                HttpContext.Session.SetObject("GetUnitDropDown", Units);
                HttpContext.Session.SetObject("GetFilterMenues", FilterMenues);
                leftMenuViewModel = new LeftMenuViewModel
                {
                    Modules = commonService.GetModules(EMP_ID),
                    ModulesMenu = Menues,
                    FilterModulesMenu = FilterMenues,
                    Units = Units
                };
            }

            //string Emp_ID2 = Request.Cookies["EmpID2"].ToString();
            return Json(leftMenuViewModel);
        }

        public string SetReporServer(string value)
        {
            string Status = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    HttpContext.Session.SetString("ReportServer", value);
                }
            }
            catch (Exception ex)
            {
                Status = string.Empty;
            }

            //string Emp_ID2 = Request.Cookies["EmpID2"].ToString();
            return Status;
        }


        //[HttpGet]
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear();
        //    return Redirect("/Account/Login");
        //}
    }
}