﻿
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
    public class TECHCC01Controller : BaseController<TECHCC01ViewModel>
    {
        private readonly ModelContext _context;
        private readonly TechnicalCommonService suggestionCommonService = null;
        private readonly DropDownListBindWeb dropDownListBindWeb = null;
        private readonly ReportRepositoryWithParameters reportRepository = null;
        private readonly PrimaryKeyGen primaryKeyGen = null;
        CommonException<TECHCC01ViewModel> commonException = null;

        public TECHCC01Controller(ModelContext context)
        {
            _context = context;
            commonException = new CommonException<TECHCC01ViewModel>();
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
            int count = 0;
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "REP_DATE", OracleDbType = OracleDbType.VarChar, Value = FromDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "TO_DATE", OracleDbType = OracleDbType.VarChar, Value = ToDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "PERSONAL_NO", OracleDbType = OracleDbType.VarChar, Value = EMP_ID });
            if ((FromDate.ToString("yyyyMM") == DateTime.Now.AddMonths(-1).ToString("yyyyMM") && DateTime.Now.Day <= 13) || FromDate.ToString("yyyyMM") == DateTime.Now.AddMonths(0).ToString("yyyyMM"))
            {
                if (FromDate < new DateTime(2008, 04, 01))
                {
                    Alert alert = new Alert
                    {
                        name = "ALERT19",
                        message = "Previous FY Date please check !",
                        type = "warning"

                    };
                    return Json(alert);
                }
                else
                {
                    count = _context.GetScalerFromDB("SELECT COUNT(*) FROM DAILY_TECH_INPUT WHERE DATA_DATE IN('"+FromDate.Date()+"','"+ToDate.Date()+"') AND FREEZE = 'Y'AND REVISED = 'N' AND SHIFT = 'G'");
                    if (count>0)
                    {
                        try
                        {
                            int a = _context.ExecuteProcedure("DAILYFUNCTION", oracleParameterCollecion);
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
                               _context.ExecuteProcedure("DAILYFUNCTION_N", oracleParameterCollecion);
                                Alert alert = new Alert
                                {
                                    name = "ALERT17",
                                    message = "Compution have been completed successfully !",
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
                    else
                    {
                        Alert alert = new Alert
                        {
                            name = "ALERT1",
                            message = "Either Production Dept Or Techical Dept has not approved data ,Please Check it",
                            type = "warning"

                        };
                        return Json(alert);
                    }
                }
            }
            else
            {
                Alert alert = new Alert
                {
                    name = "ALERT21",
                    message = " Please Check Dates !",
                    type = "warning"

                };
                return Json(alert);
            }

            





        }
    }
}