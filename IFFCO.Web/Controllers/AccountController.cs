using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFFCO.HRMS.Entities.Models;
using IFFCO.HRMS.Service;
using IFFCO.HRMS.Shared.Entities;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using IFFCO.TECHPROD.Web.Areas.M1.Controllers;
using IFFCO.HRMS.Entities.AppConfig;
using System.Net;

namespace IFFCO.TECHPROD.Web.Controllers
{
    public class AccountController : Controller
    {
        public CommonService commonService = null;
        private IHttpContextAccessor _accessor;
        private LoginModel CommonViewModel;
        private AccountService accountService;
        private BaseController<LoginModel> AccountBaseController;
        public AccountController(IHttpContextAccessor accessor)
        {
            this._accessor = accessor;
            accountService = new AccountService();
            commonService = new CommonService();
            CommonViewModel = new LoginModel();
        }
        public IActionResult Login()
        {
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            try
            {
                HttpContext.Session.SetString("ProjectId", new AppConfiguration().ProjectId);
                CommonViewModel.ModuleId = loginViewModel.ModuleId;
                CommonViewModel.Password = loginViewModel.Password;
                CommonViewModel.PersonalNo = loginViewModel.PersonalNo;
                CommonViewModel.ProjectId = HttpContext.Session.GetString("ProjectId");
                string clientIp = "";
                string fullClientIp = "";
                try
                {
                    var addlist = Dns.GetHostEntry(Dns.GetHostName());
                    clientIp = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                    fullClientIp = HttpContext.Request.Headers["X-Forwarded-For"].ToString() + ";"
                                          + addlist.AddressList[1].ToString() + ";"
                                          + _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                catch (Exception ex)
                {
                    clientIp = "";
                    fullClientIp = "";
                }

                var Validate = accountService.ValidateUser(CommonViewModel);
                if (Validate.Status == "0")
                {
                    try
                    {
                        var session = accountService.SessionLog(CommonViewModel, clientIp, fullClientIp);
                    }
                    catch (Exception ex) { }

                    AccountBaseController = new BaseController<LoginModel>(_accessor);
                    AccountBaseController.IntializeSessoin(loginViewModel.ModuleId, loginViewModel.PersonalNo, 0, "L", CommonViewModel.ProjectId);
                    if (Convert.ToString(HttpContext.Session.GetString("StatusCode")) == null || Convert.ToString(HttpContext.Session.GetString("StatusCode")) != "0")
                    {

                        ViewBag.Message = Convert.ToString(HttpContext.Session.GetString("ErrorMessage"));
                        return RedirectToAction("Login");
                    }

                    return RedirectToAction("Index", "Home", new { area = loginViewModel.ModuleId });

                }
                else
                {
                    ViewBag.Message = Validate.Message;
                    return View();

                }
            }
            catch (Exception ex)
            {
                //ViewBag.Message = ex.Message;
                Response.WriteAsync(ex.Message);
                return RedirectToAction("Login");
            }
        }

        public JsonResult GetMouldeResultForAccount(int PersonalNo)
        {
            return Json(commonService.GetModules(PersonalNo));
        }
        public JsonResult ChangePassword([FromBody]LoginModel loginViewModel)
        {
            var ObjChangePassword = accountService.ChangePassword(loginViewModel);
            CommonViewModel.Status = ObjChangePassword.Status;
            CommonViewModel.ErrorMessage = ObjChangePassword.Error;
            CommonViewModel.Message = ObjChangePassword.Message;
            CommonViewModel.PersonalNo = loginViewModel.PersonalNo;
            CommonViewModel.Password = loginViewModel.Password;
            return Json(CommonViewModel);
        }
        public JsonResult CallOTP(DateTime? Dob, int PersonalNo)
        {
            var ObjCallOTP = accountService.CallOTP(Dob, PersonalNo);
            CommonViewModel.Status = ObjCallOTP.Status;
            CommonViewModel.ErrorMessage = ObjCallOTP.Error;
            CommonViewModel.Message = ObjCallOTP.Message;

            return Json(CommonViewModel);
        }
        public JsonResult ValidateOTP(string otp, int PersonalNo)
        {
            var ObjValidateOTP = accountService.ValidateOTP(otp, PersonalNo);
            CommonViewModel.Status = ObjValidateOTP.Status;
            CommonViewModel.ErrorMessage = ObjValidateOTP.Error;
            CommonViewModel.Message = ObjValidateOTP.Message;

            return Json(CommonViewModel);
        }
        public JsonResult ValidatePasswd(string password)
        {
            var ObjChangePassword = accountService.ValidatePasswd(password);
            CommonViewModel.Status = ObjChangePassword.Status;
            CommonViewModel.ErrorMessage = ObjChangePassword.Error;
            CommonViewModel.Message = ObjChangePassword.Message;

            return Json(CommonViewModel);
        }
        public JsonResult SetNewPass(int PersonalNo, string Password, string Otp)
        {
            LoginModel loginModel = new LoginModel
            {
                PersonalNo = PersonalNo,
                Password = Password,
                Otp = Otp
            };
            var ObjChangePassword = accountService.SetNewPass(loginModel);
            CommonViewModel.Status = ObjChangePassword.Status;
            CommonViewModel.ErrorMessage = ObjChangePassword.Error;
            CommonViewModel.Message = ObjChangePassword.Message;
            CommonViewModel.PersonalNo = loginModel.PersonalNo;
            CommonViewModel.Password = loginModel.Password;
            return Json(CommonViewModel);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Account/Login");
        }

    }
}