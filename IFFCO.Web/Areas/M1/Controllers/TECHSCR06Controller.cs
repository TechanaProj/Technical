
using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;


namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHSCR06Controller : BaseController<TECHSCR06ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR03ViewModel> commonException = null;

        public TECHSCR06Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR03ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            suggestionCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }




        public ActionResult GenerateReport(DateTime ReportDate,string ReportType)
        {
            string Report = "";
            string QueryString = String.Empty;
            Report reportobj = GenerateReportData(ReportDate,ReportType);
            string data = reportobj.ReportName + "+destype=cache+desformat=" + reportobj.ReportFormat;

            Report = reportRepository.GenerateReport(reportobj.Query, data, "NotEncode");
            CommonViewModel.AreaName = this.ControllerContext.RouteData.Values["area"].ToString();
            CommonViewModel.SelectedMenu = this.ControllerContext.RouteData.Values["controller"].ToString();
            CommonViewModel.Report = Report;
            return Json(CommonViewModel);
        }
        public Report GenerateReportData(DateTime ReportDate, string ReportType)
        {
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            Report ReportData = new Report();
            ReportData.ReportFormat = "PDF";
            ReportData.Query = "IDT=" + ReportDate.Date() + "+" + "P_I_DT=" + ReportDate.Date() + "+" + "P_IDT=" + ReportDate.Date();
            switch (ReportType)
            {
                case "T":
                   
                    ReportData.ReportName = "tptreportN.rep";
                    break;
                case "C":
                    
                    ReportData.ReportName = "REP_CONDENSATE.REP";
                    break;
                case "D":
                  
                    ReportData.ReportName = "DisplayBoard_rep.rep";
                    break;
                case "J":
                  
                    ReportData.ReportName = "Percentage_urea_desp.rep";
                    break;
                default:
                   
                    ReportData.ReportName = "bagreport.rep";
                    break;
                   
            }
            
            return ReportData;
        }


    }
}