
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
using System.Data;

namespace IFFCO.TECHPROD.Web.Areas.M1.Controllers
{
    [Area("M1")]
    public class TECHSCR09BController : BaseController<TECHSCR07BViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService technicalCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHSCR07BViewModel> commonException = null;

        public TECHSCR09BController(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHSCR07BViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            technicalCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }




        public ActionResult SendData(DateTime dt, string ReportType, string Operation)
        {



            if (Operation.ToLower() == "send-data")
            {

                if (dt > DateTime.Now.Date)
                {
                    Alert alert = new Alert
                    {
                        name = "ERROR",
                        message = "Date can not be greatter than Today's Date",
                        type = "error"

                    };
                    return Json(alert);
                }


                List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
                oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "I_DT", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
                oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

                try
                {
                    int a = _context.ExecuteProcedure("PMIS_MAIN", oracleParameterCollecion);

                    if (a == -1)
                    {
                        Alert alert = new Alert
                        {
                            name = "SUCCESS",
                            message = oracleParameterCollecion[4].Value.ToString(),
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
                catch (Exception ex)
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

            return Json(CommonViewModel);
        }
    }
}