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
    public class TECHCC07Controller : BaseController<TECHCC07ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC07ViewModel> commonException = null;

        public TECHCC07Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHCC07ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            suggestionCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }

        public IActionResult Execute(DateTime FromDate, DateTime ToDate, string Gas)
        {
            if (FromDate > ToDate)
            {
                Alert alert = new Alert
                {
                    name = "ERROR",
                    message = "From date can not be greatter than ToDate",
                    type = "error"

                };
                return Json(alert);
            }
            if (FromDate > DateTime.Now.Date)
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
           int count = _context.GetScalerFromDB("SELECT COUNT(*) FROM MONTHLY_TECH_INPUT WHERE FROM_DATE BETWEEN '"+FromDate.Date()+"' AND '"+ToDate.Date()+ "' AND REVISED = 'N' AND AND TYPE_OF_GAS='"+Gas+"'");
           

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "ID1", OracleDbType = OracleDbType.VarChar, Value = FromDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "ID2", OracleDbType = OracleDbType.VarChar, Value = ToDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "PER_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "GAS", OracleDbType = OracleDbType.VarChar, Value = Gas });


            try
            {


                if (count > 0)
                {
                    int a = _context.ExecuteProcedure("CUMULATIVE_NORMS", oracleParameterCollecion);
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