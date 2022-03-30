using Devart.Data.Oracle;
using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.CommonFunctions;
using IFFCO.TECHPROD.Web.Controllers;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHCC15Controller : BaseController<TECHCC04ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC04ViewModel> commonException = null;

        public TECHCC15Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHCC04ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            suggestionCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }

        public IActionResult Execute(DateTime FROM_DATE, DateTime TO_DATE)
        {
            //if (FROM_DATE > TO_DATE)
            //{
            //    Alert alert = new Alert
            //    {
            //        name = "ERROR",
            //        message = "From date can not be greatter than ToDate",
            //        type = "error"

            //    };
            //    return Json(alert);
            //}
            //if (FROM_DATE > DateTime.Now.Date)
            //{
            //    Alert alert = new Alert
            //    {
            //        name = "ERROR",
            //        message = "From date can not be greatter than Today's Date",
            //        type = "error"

            //    };
            //    return Json(alert);
            //}
            //if (TO_DATE > DateTime.Now)
            //{
            //    Alert alert = new Alert
            //    {
            //        name = "ERROR",
            //        message = "ToDate can not be greatter than Today's Date",
            //        type = "error"

            //    };
            //    return Json(alert);
            //}
            //int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            //if ((TO_DATE.ToString("DD/MM/YYYY") == new DateTime(FROM_DATE.Year,FROM_DATE.Month,DateTime.DaysInMonth(FROM_DATE.Year,FROM_DATE.Month)).ToString("DD/MM/YYYY")))
            //{
                List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
                oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "fr_date", OracleDbType = OracleDbType.VarChar, Value = FROM_DATE.Date() });
                oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "t_date", OracleDbType = OracleDbType.VarChar, Value = TO_DATE.Date() });
                
                //try
                //{
                    int a = _context.ExecuteProcedure("XIIAB_PROC", oracleParameterCollecion);
            //        if (a == -1)
            //        {
                        Alert alert = new Alert
                        {
                         name = "ALERT17",
                         message = "Compution have been completed successfully !",
                         type = "success"

                        };
                         return Json(alert);
            //        }
            //    }
            //    catch (Exception)
            //    {

            //        Alert alert = new Alert
            //        {
            //            name = "Error",
            //            message = "Internal Server error",
            //            type = "error"
            //        };
            //        return Json(alert);
            //    }

            //    Alert ale = new Alert
            //    {
            //        name = "Error",
            //        message = "Internal Server error",
            //        type = "error"
            //    };
            //    return Json(ale);

            //}
            //else
            //{
            //    Alert alert = new Alert
            //    {
            //        name = "Error",
            //        message = "From Date and To Date Should not exceed One Month Please check!",
            //        type = "error"
            //    };
            //    return Json(alert);
            //}






        }





    }
}