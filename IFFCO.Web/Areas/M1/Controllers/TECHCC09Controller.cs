
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
    public class TECHCC09Controller : BaseController<TECHCC09ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC09ViewModel> commonException = null;

        public TECHCC09Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHCC09ViewModel>();
            dropDownListBindWeb = new DropDownListBindWeb();
            suggestionCommonService = new TechnicalCommonService();
            reportRepository = new ReportRepositoryWithParameters();
            primaryKeyGen = new PrimaryKeyGen();
        }
        public IActionResult Index()
        {

            return View(CommonViewModel);
        }

        public IActionResult Execute(DateTime FromDate, DateTime ToDate)
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
            

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "frm_date", OracleDbType = OracleDbType.VarChar, Value = FromDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "t_date", OracleDbType = OracleDbType.VarChar, Value = ToDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "PERSONAL_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });

            try
            {
                int a = _context.ExecuteProcedure("amm_variation", oracleParameterCollecion);
                if (a == -1)
                {
                    Alert alert = new Alert
                    {
                        name = "ALERT17",
                        message = "Compution have been completed successfully !",
                        type = "success"

                    };
                    return Json(alert);
                }
                else
                {
                    Alert alert = new Alert
                    {
                        name = "ALERT1",
                        message = "Either Production Dept Or Techical Dept has not approved data ,Please Check it",
                        type = "success"

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