using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Service;
using IFFCO.HRMS.Shared.CommonFunction;
using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Components
{
    public class LeftMenu : ViewComponent
    {
        CommonService commonService = null;
        DropDownListBindWeb dropDownListBind = null;
        public AccountService accountService = null;
        public LeftMenu()
        {
            commonService = new CommonService();
            dropDownListBind = new DropDownListBindWeb();
            accountService = new AccountService();
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            LeftMenuViewModel leftMenuViewModel = null;
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            string moduleid = Convert.ToString(HttpContext.Session.GetString("ModuleID"));
            //var obj = accountService.GetModules(EMP_ID).FirstOrDefault(x => x.MODULEID == HttpContext.Session.GetString("ModuleID"));
            //if (obj != null) obj.SelectedValue = "True";
            LeftMenuViewModel leftMenuViewModelobj = new LeftMenuViewModel();

            List<ModulesMenu> Menues = commonService.GetModulesMenus(EMP_ID).Where(x => x.Menulevel == 1).ToList();
            List<SelectListItem> Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);
            if(Menues.Count>0 && Units.Count > 0)
            {
                HttpContext.Session.SetObject("GetModulesMenus", Menues);
                HttpContext.Session.SetObject("GetUnitDropDown", Units);
                HttpContext.Session.SetObject("GetFilterMenues", null);
                leftMenuViewModel = new LeftMenuViewModel
                {
                    Modules = commonService.GetModules(EMP_ID),
                    ModulesMenu = Menues,
                    Units = Units,
                    FilterModulesMenu = null//FilterMenues
                };
            }
            else
            {
                IntializeSessoin(moduleid, EMP_ID, 0, "M", HttpContext.Session.GetString("ProjectId"));

                 Menues = commonService.GetModulesMenus(EMP_ID).Where(x => x.Menulevel == 1).ToList();
                 Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);
                HttpContext.Session.SetObject("GetModulesMenus", Menues);
                HttpContext.Session.SetObject("GetUnitDropDown", Units);
                HttpContext.Session.SetObject("GetFilterMenues", null);
                leftMenuViewModel = new LeftMenuViewModel
                {
                    Modules = commonService.GetModules(EMP_ID),
                    ModulesMenu = Menues,
                    Units = Units,
                    FilterModulesMenu = null//FilterMenues
                };
            }
            //var FilterMenues = commonService.GetModulesMenus("MASTERS_MENU");
            

            //if (HttpContext.Session.GetObject<List<ModulesMenu>>("GetModulesMenus") != null && HttpContext.Session.GetObject<List<SelectListItem>>("GetUnitDropDown") != null)
            //{
            //    leftMenuViewModel = new LeftMenuViewModel
            //    {
            //        Modules = commonService.GetModules(EMP_ID),
            //        ModulesMenu = HttpContext.Session.GetObject<List<ModulesMenu>>("GetModulesMenus"),
            //        Units = HttpContext.Session.GetObject<List<SelectListItem>>("GetUnitDropDown"),
            //        //FilterModulesMenu = commonService.GetModulesMenus("MASTERS_MENU"),
            //        //FilterModulesMenu = HttpContext.Session.GetObject<List<ModulesMenu>>("GetFilterMenues"),
            //    };
            //}
            //else
            //{
            //    var Menues = commonService.GetModulesMenus().Where(x => x.Menulevel == 1).ToList();
            //    var Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), moduleid);
            //    //var FilterMenues = commonService.GetModulesMenus("MASTERS_MENU");
            //    HttpContext.Session.SetObject("GetModulesMenus", Menues);
            //    HttpContext.Session.SetObject("GetUnitDropDown", Units);
            //    HttpContext.Session.SetObject("GetFilterMenues", null);                
            //    leftMenuViewModel = new LeftMenuViewModel
            //    {
            //        Modules = commonService.GetModules(EMP_ID),
            //        ModulesMenu = Menues,
            //        Units = Units,
            //        FilterModulesMenu = null//FilterMenues
            //    };
            //}


            var ModuleData = leftMenuViewModel.Modules.FirstOrDefault(x => x.Value == HttpContext.Session.GetString("ModuleID"));
            if (ModuleData != null) ModuleData.Selected = true;
            var UnitData = leftMenuViewModel.Units.FirstOrDefault(x => x.Value == HttpContext.Session.GetString("UnitCode"));
            if (UnitData != null) UnitData.Selected = true;

            return View("~/Views/Menu/_LeftMenu.cshtml", leftMenuViewModel);
        }
        
        public IViewComponentResult GetMouldeResult()
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            var obj = commonService.GetModules(EMP_ID).FirstOrDefault(x => x.Value == HttpContext.Session.GetString("ModuleID"));
            if (obj != null) obj.Selected = true;
            LeftMenuViewModel leftMenuViewModel = new LeftMenuViewModel
            {
                Modules = commonService.GetModules(EMP_ID),
                ModulesMenu = commonService.GetModulesMenus(EMP_ID),
                Units = dropDownListBind.GetUnitWithSecurity(Convert.ToString(EMP_ID), HttpContext.Session.GetString("ModuleID"))//commonService.GetUnitDropDown()
            };

            //string Emp_ID2 = Request.Cookies["EmpID2"].ToString();
              return new Microsoft.AspNetCore.Mvc.ViewComponents.ContentViewComponentResult(JsonConvert.SerializeObject(leftMenuViewModel));
        }
        public void IntializeSessoin(string ModuleID, float EMPID, float UnitCode, string EVENT,string ProjectId)
        {
            UserModuleUnitModel UserModuleUnitModel1 = new UserModuleUnitModel
            {
                ModuleID = ModuleID,
                EMPID = EMPID,
                UnitCode = UnitCode,
                EVENT = EVENT,
                ProjectId=ProjectId
            };
            

            var Pcglobalvalues = accountService.Pcglobalvalues(UserModuleUnitModel1);

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
    }
}
