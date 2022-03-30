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
    public class TECHCC08Controller : BaseController<TECHCC04ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC04ViewModel> commonException = null;

        public TECHCC08Controller(ModelContext context)
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

        public IActionResult Execute(DateTime FROM_DATE, DateTime TO_DATE, string report)
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
            //try
            //{
                int EMP_ID = Convert.ToInt32(HttpContext.Session.GetInt32("EmpID"));
                if (report == "D")
                {
                    List<OracleParameter> oracleParameterCollecion1 = new List<OracleParameter>();
                    oracleParameterCollecion1.Add(new OracleParameter() { ParameterName = "I_DT1", OracleDbType = OracleDbType.VarChar, Value = FROM_DATE.Date() });
                    oracleParameterCollecion1.Add(new OracleParameter() { ParameterName = "I_DT2", OracleDbType = OracleDbType.VarChar, Value = TO_DATE.Date() });
                    oracleParameterCollecion1.Add(new OracleParameter() { ParameterName = "PERSONAL_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });



                    int a1 = _context.ExecuteProcedure("NG_ANALYSIS", oracleParameterCollecion1);
                    //if (a1 == -1)
                    //{
                        Alert alert = new Alert
                        {
                            name = "SUCCESS",
                            message = "Compution have been completed successfully !",
                            type = "success"

                        };
                        return Json(alert);
                    //}


                }
                else
                {
                    List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
                    oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "FRM_DATE", OracleDbType = OracleDbType.VarChar, Value = FROM_DATE.Date() });
                    oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "T_DATE", OracleDbType = OracleDbType.VarChar, Value = TO_DATE.Date() });
                    oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "PERS_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });
                    oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "INPUT", OracleDbType = OracleDbType.VarChar, Value = report });
                    int a2 = _context.ExecuteProcedure("NG_ANALYSIS1", oracleParameterCollecion);
                    //if (a2 == -1)
                    //{
                        Alert alert = new Alert
                        {
                            name = "SUCCESS",
                            message = "Compution have been completed successfully !",
                            type = "success"

                        };
                        return Json(alert);
                //    }
                }
            //}
            //catch (Exception)
            //{
            //    Alert alert = new Alert
            //    {
            //        name = "Error",
            //        message = "Internal Server error",
            //        type = "error"
            //    };
            //    return Json(alert);

            //}
            //Alert a = new Alert
            //{
            //    name = "Error",
            //    message = "Internal Server error",
            //    type = "error"
            //};
            //return Json(a);








        }





    }
}