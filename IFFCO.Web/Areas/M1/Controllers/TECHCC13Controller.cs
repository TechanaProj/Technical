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
    public class TECHCC13Controller : BaseController<TECHCC13ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC13ViewModel> commonException = null;

        public TECHCC13Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHCC13ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            suggestionCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }

        public IActionResult Execute(DateTime REP_DATE, DateTime ToDate)
        {
            if (REP_DATE > ToDate)
            {
                Alert alert = new Alert
                {
                    name = "ERROR",
                    message = "From date can not be greatter than ToDate",
                    type = "error"

                };
                return Json(alert);
            }
            if (REP_DATE > DateTime.Now.Date)
            {
                Alert alert = new Alert
                {
                    name = "ERROR",
                    message = "From date can not be greatter than Today's Date",
                    type = "error"

                };
                return Json(alert);
            }
            if (ToDate > DateTime.Now)
            {
                Alert alert = new Alert
                {
                    name = "ERROR",
                    message = "ToDate can not be greatter than Today's Date",
                    type = "error"

                };
                return Json(alert);
            }
            int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
            if ((REP_DATE.ToString("yyyyMM") == DateTime.Now.AddMonths(-1).ToString("yyyyMM") && DateTime.Now.Day <= 10) || REP_DATE.ToString("yyyyMM") == DateTime.Now.AddMonths(0).ToString("yyyyMM"))
            {

                Alert alert = new Alert
                {
                    name = "ALERT19",
                    message = "Previous FY Date please check !",
                    type = "warning"

                };
                return Json(alert);

            }

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "FRM_DATE", OracleDbType = OracleDbType.VarChar, Value = REP_DATE.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "T_DATE", OracleDbType = OracleDbType.VarChar, Value = ToDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "PERSONAL_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });
             //
              
            try
            {
                int a = _context.ExecuteProcedure("WATER_NONPLANT", oracleParameterCollecion);
                if (a == -1)
                {
                    Alert alert = new Alert
                    {
                        name = "SUCCESS",
                        message = "Compution have been completed successfully !",
                        type = "success"

                    };
                    return Json(alert);
                }
                else
                {
                    Alert alert = new Alert
                    {
                        name = "NODATA",
                        message = "No Data Found For This Period",
                        type = "warning"
                    };
                    return Json(alert);
                }
            }
            catch (Exception)
            {
                Alert alert = new Alert
                {
                    name = "Error",
                    message = "Internal Server error",
                    type = "error"
                };
                return Json(alert);

            }



        }





    }
}