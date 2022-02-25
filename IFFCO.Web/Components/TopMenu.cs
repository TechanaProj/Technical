using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Components
{
    public class TopMenu : ViewComponent
    {
        //AccountService accountService = null;
        CommonService commonService = null;
        public TopMenu()
        {
            //accountService = new AccountService();
            commonService = new CommonService();
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            TopMenuViewModel topMenuViewModel = null;
            try
            {
                int? EmpId = HttpContext.Session.GetInt32("EmpID");        
                var UserInfo = commonService.GetUserInfo(EmpId);
               
                topMenuViewModel = new TopMenuViewModel
                {
                    UserName = HttpContext.Session.GetString("EmployeeName"),
                    Designation = UserInfo.Designation,
                    Image =commonService.GetProfileImage(EmpId),
                    PersonalNo = EmpId.ToString(),
                    Unit = UserInfo.Unit,
                    UnitCode = UserInfo.UnitCode,
                    WorkDepartment = UserInfo.WorkDepartment,
                    WorkSection = UserInfo.WorkSection,
                    WorkUnit = UserInfo.WorkUnit
                };

             
            }
            catch (Exception ex)
            {
                
            }


            return View("~/Views/Menu/_TopMenu.cshtml", topMenuViewModel);
        }
    }
}
