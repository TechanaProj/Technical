﻿using Devart.Data.Oracle;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using IFFCO.HRMS.Service;
using IFFCO.TECHPROD.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class TechnicalCommonService : CommonService
    {
        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        //IDataContextAsync context;
        private readonly ModelContext _context;
        private readonly string ProjectId = string.Empty;

        DataTable _dt = new DataTable();

        public TechnicalCommonService()
        {
            _context = new ModelContext();
            ProjectId = new AppConfiguration().ProjectId;
        }

        /// <summary>
        /// /SELECT COUNT(*) FROM DAILY_PLANT_INPUT WHERE TRUNC(DATE_TIME)='18-OCT-2017' AND SHIFT='G' AND FREEZE='Y' AND PR_CODE LIKE 'A1%'
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetReason()
        {
            var data = _context.GetSQLQuery("SELECT ALL SDN_REASON_MASTER.SD_CODE, SDN_REASON_MASTER.REASON FROM SDN_REASON_MASTER");
            return data.AsEnumerable().Select(e => new SelectListItem
            {
                Text = e.Field<string>("REASON"),
                Value = e.Field<string>("SD_CODE"),
            }).ToList();

        }
        public string GetScreenAccess(int pno, string formName, DateTime dt)
        {
            return _context.GetCharScalerFromDB("SELECT TECHANA.Get_Time_Based_Screen_Acess(" + pno + ", '" + formName + "','" + dt.Date() + "') FROM DUAL ");
        }
        public double GetFrValue(DateTime dt)
        {
            return Convert.ToDouble(_context.GetCharScalerFromDB(@"SELECT FR_VALUE FROM FACTOR_MASTER WHERE FR_CODE = 'AMSTF' AND '" + dt.Date() + "' BETWEEN NVL(EFFECTIVE_FROM_DATE, '01/JAN/1900') AND NVL(EFFECTIVE_TO_DATE, SYSDATE) "));
        }
        //---------AMMSC01----------------------//
        public List<CommonData> GetRecordsAMMSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsAMMSC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_POST", oracleParameterCollecion);

            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsAMMSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string SaveRecordsAMMSC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_54", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_58", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_62", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_66", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string PostShutdownAMMSC01(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[9].Value.ToString();
            return alert;
        }
        public string PostTechRemarkAMMSC01(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[5].Value.ToString();
            return alert;
        }


        //---------AMMSC02----------------------//


        public List<CommonData> GetRecordsAMMSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsAMMSC02(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsAMMSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string SaveRecordsAMMSC02(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_54", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_58", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_62", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_66", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string PostShutdownAMMSC02(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return "";
        }
        public string PostTechRemarkAMMSC02(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("AMMSC02_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return "";
        }





        //---------PHSC01----------------------//
        public List<CommonData> GetRecordsPHSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("PHSC01_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsPHSC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            var data = _context.ExecuteProcedureForRefCursor("PHSC01_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsPHSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("PHSC01_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }



        //---------PHSC02----------------------//
        public List<CommonData> GetRecordsPHSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("PHSC02_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsPHSC02(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            var data = _context.ExecuteProcedureForRefCursor("PHSC02_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsPHSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("PHSC02_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }



        //------------------ELECTRICAL1---------------------------//
        public List<CommonData> GetRecordsELECTRICAL1(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("ELECTRICAL_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }





        public string PostRecordsELECTRICAL1(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            var data = _context.ExecuteProcedureForRefCursor("ELECTRICAL_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsELECTRICAL1(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("ELECTRICAL_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }




        //-----------OSSC01-------------------//

        public List<CommonData> GetRecordsOSSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("OSSC01_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),

                });
            }

            return cd;

        }
        public string PostRecordsOSSC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("OSSC01_POST", oracleParameterCollecion);

            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsOSSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("OSSC01_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }


        //-----------OSSC11-------------------//

        public List<CommonData> GetRecordsOSSC11(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("OSSC11_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                   

                });
            }

            return cd;

        }
        public string PostRecordsOSSC11(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("OSSC11_POST", oracleParameterCollecion);

            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsOSSC11(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date()});
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = int.Parse(pno) });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("TECHANA.OSSC11_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }


        //----------------------GASALL----------------//
        public List<CommonData> GetRecordsGASALL(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("GASALL_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),

                });
            }

            return cd;

        }
        public string PostRecordsGASALL(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name.Substring(0, 3) != "A2-" ? Input_Name.Split('-')[0] : Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME1", OracleDbType = OracleDbType.VarChar, Value = Input_Name.Substring(0, 3) != "A2-" ? Input_Name.Split('-')[1] : null });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("GASALL_POST", oracleParameterCollecion);

            string query = "";
            if (Input_Name.Substring(0, 3) != "A2-")
            {
                if ((int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM DAILYTECH_OUTPUT WHERE DATA_DATE='" + dt.Date() + "'") == 0)
                {
                    query = "INSERT INTO DAILYTECH_OUTPUT(DATA_DATE," + Input_Name.Split('-')[1] + " ,CREATED_BY,CREATION_DATETIME) values('" + dt.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE)";
                    var i = _context.insertUpdateToDB(query);
                }
                else
                {
                    query = "update DAILYTECH_OUTPUT SET " + Input_Name.Split('-')[1] + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',CREATION_DATETIME=SYSDATE where Data_Date='" + dt.Date() + "'";
                    var i = _context.insertUpdateToDB(query);
                }
            }
            string alert = oracleParameterCollecion[8].Value.ToString();
            return alert;

        }
        public string ApproveRecordsGASALL(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("GASALL_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }



        //----------------NG ANALYSIS----------------//

        public List<CommonData> GetRecordsNGANALYSIS(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("NGANALYSIS_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),

                });
            }

            return cd;

        }
        public string PostRecordsNGANALYSIS(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {

            string query = "";
            string alert = "";

            if ((int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM TEMP_NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'") == 0)
            {
                query = "INSERT INTO TEMP_NGLHV(DATA_DATE," + Input_Name + " ,CREATED_BY,CREATION_DATE,INPUT_TYPE) values('" + dt.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'D')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update TEMP_NGLHV SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',CREATION_DATE=SYSDATE where Data_Date='" + dt.Date() + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }


            return alert;

        }


        public string SaveRecordsNGANALYSIS(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("NGANALYSIS_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[1].Value.ToString();
            return alert;

        }

        public bool ISRecordsSavedToNHLHV(string formName, string shift, string pno, DateTime dt)
        {
            if ((int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM TEMP_NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'") == 0)
            {
                return false;

            }
            else if ((int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'") == 0)
            {
                return false;
            }
            else if (Convert.ToDateTime(_context.GetCharScalerFromDB("SELECT CREATION_DATE FROM NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'")) != Convert.ToDateTime(_context.GetCharScalerFromDB("SELECT CREATION_DATE FROM TEMP_NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'")))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //----------------------POWER ----------------//
        public TotalSDPower GetRecordsPWRSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR1", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });


            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();
            OracleDataReader reader1 = ((OracleCursor)oracleParameterCollecion[5].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            List<ShutDownPower> cd1 = new List<ShutDownPower>();
            while (reader1.Read())
            {
                cd1.Add(new ShutDownPower()
                {

                    InputLabel = reader1.GetString(reader1.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader1.GetString(reader1.GetOrdinal("INPUT_VALUE")),
                    Heading = reader1.GetString(reader1.GetOrdinal("HEADING")),
                    InputText = reader1.GetString(reader1.GetOrdinal("INPUT_TEXT")),
                    InputType = reader1.GetString(reader1.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader1.GetString(reader1.GetOrdinal("READONLY")),
                    Category = reader1.GetString(reader1.GetOrdinal("CATEGORY")),
                    Readonly = reader1.GetString(reader1.GetOrdinal("READONLY")),
                    OperationType = reader1.GetString(reader1.GetOrdinal("OPERATION_TYPE")),

                });
            }

            TotalSDPower totalSDPower = new TotalSDPower();
            totalSDPower.CommonData = cd;
            totalSDPower.ShutDownPower = cd1;
            return totalSDPower;

        }
        public string PostRecordsPWRSC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsPWRSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }

        public string PostShutdownPWRSC01(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return "";
        }

        public string PostTechRemarkPWRSC01(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return "";
        }




        //----------------------SPECIFIC ENERGY ----------------//
        public List<CommonData> GetRecordsSPENERGY(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("SPENERGY_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY"))

                });
            }

            return cd;

        }
        public string PostRecordsSPENERGY(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("SPENERGY_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[6].Value.ToString();
            return alert;

        }

        //----------------------XIIAB ----------------//
        public List<CommonData> GetRecordsXIIAB(string formName, string shift, string pno, DateTime dt1, DateTime dt2)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("XII_AB_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[5].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    SubLabel = reader.GetString(reader.GetOrdinal("SUB_LABEL")),

                });
            }

            return cd;

        }


        public string PostRecordsXIIAB(string formName, string shift, string pno, DateTime dt1, DateTime dt2, string Input_Value, string Input_Name, string op)
        {

            string query = "";
            string alert = "";

            if ((int)_context.GetScalerFromDB("SELECT COUNT(FROM_DATE) FROM XIIAB_DATA WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'") == 0)
            {
                query = "INSERT INTO XIIAB_DATA(FROM_DATE,TO_DATE," + Input_Name + " ,CREATED_BY,CREATION_TIME) values('" + dt1.Date() + "','" + dt2.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE)";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update XIIAB_DATA SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',CREATION_TIME=SYSDATE  WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }


            return alert;

        }

        //----------------------NONPLANT ----------------//
        public List<CommonData> GetRecordsNONPLANT(string formName, string shift, string pno, DateTime dt1, DateTime dt2)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("NONPLANT_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[5].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),


                });
            }

            return cd;

        }


        public string PostRecordsNONPLANT(string formName, string shift, string pno, DateTime dt1, DateTime dt2, string Input_Value, string Input_Name, string op)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(FROM_DATE) FROM MONTHLY_TECH_INPUT WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'  AND REVISED = 'N' and type_of_gas = 'COMPOSITE' AND INPUT_TYPE = 'M'";
            if ((int)_context.GetScalerFromDB(query) == 0)
            {
                query = "INSERT INTO MONTHLY_TECH_INPUT(FROM_DATE,TO_DATE," + Input_Name + " ,CREATED_BY,creation_date,INPUT_TYPE) values('" + dt1.Date() + "','" + dt2.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'M')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update MONTHLY_TECH_INPUT SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_date=SYSDATE  WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "' AND REVISED = 'N' and type_of_gas = 'COMPOSITE' AND INPUT_TYPE = 'M'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    query = "update MONTHLY_TECH_INPUT SET UR1_NPPP = 0, UR2_NPPP = 0 WHERE TYPE_OF_GAS not IN('COMPOSITE','NG') AND FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
                    i = _context.insertUpdateToDB(query);
                    alert = "Updated";
                }
            }


            return alert;

        }
        public string ApproveRecordsNONPLANT(string formName, string shift, string pno, DateTime dt1, DateTime dt2)
        {
           string query = "UPDATE MONTHLY_TECH_INPUT SET FREEZE='Y' WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "' and revised = 'N'";
            var i = _context.insertUpdateToDB(query);
            if (i>0)
            {
                return "Data Freezed";
            }
            return "Something went worng";
        }


        //----------------------SPTARGET MASTER ----------------//
        public List<CommonData> GetRecordsSPTARGET(string formName, string shift, string pno, DateTime dt1, DateTime dt2)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("SPECIFIC_EN_TARGET_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[5].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    SubLabel= reader.GetString(reader.GetOrdinal("SUB_LABEL")),

                });
            }

            return cd;

        }
        public string PostRecordsSPTARGET(string pno, DateTime dt1, DateTime dt2, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("SPECIFIC_EN_TARGET_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[5].Value.ToString();
            return alert;

        }
        public string ApproveRecordsSPTARGET( DateTime dt1, DateTime dt2)
        {
            string query = "UPDATE specific_en_TARGET_MASTER SET FREEZE='Y' WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "' and revised = 'N' AND FREEZE='N'";
            var i = _context.insertUpdateToDB(query);
            if (i > 0)
            {
                return "Data Freezed";
            }
            return "Something went worng";
        }


        //----------------------TARGET MASTER ----------------//
        public List<CommonData> GetRecordsTARGET(string pno, string MonthYear, string FYear)
        {
            string moyear = MonthYear.Split('-')[1] + MonthYear.Split('-')[0];
            string fy = FYear.Split('-')[0] + "-" + FYear.Split('-')[1].Substring(2);
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FYEAR", OracleDbType = OracleDbType.VarChar, Value = fy });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_MONTHYEAR", OracleDbType = OracleDbType.VarChar, Value = moyear });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TARGET_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[3].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    SubLabel = reader.GetString(reader.GetOrdinal("SUB_LABEL")),
                    DataType = reader.GetString(reader.GetOrdinal("DATA_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsTARGET(string pno, string MonthYear, string FYear,string DataType, string Input_Value, string Input_Name)
        {
            string moyear = MonthYear.Split('-')[1] + MonthYear.Split('-')[0];
            string fy = FYear.Split('-')[0] + "-" + FYear.Split('-')[1].Substring(2);
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FYEAR", OracleDbType = OracleDbType.VarChar, Value = fy });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_MONTHYEAR", OracleDbType = OracleDbType.VarChar, Value = moyear });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATA_TYPE", OracleDbType = OracleDbType.VarChar, Value = DataType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("TARGET_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[6].Value.ToString();
            return alert;

        }
        public string ApproveRecordsTARGET(string MonthYear, string FYear)
        {
            string moyear = MonthYear.Split('-')[1] + MonthYear.Split('-')[0];
            string fy = FYear.Split('-')[0] + "-" + FYear.Split('-')[1].Substring(2);
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FYEAR", OracleDbType = OracleDbType.VarChar, Value = fy });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_MONTHYEAR", OracleDbType = OracleDbType.VarChar, Value = moyear });            
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("TARGET_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[2].Value.ToString();
            return alert;
        }




        //----------------------Gas CV ----------------//
        public List<CommonData> GetRecordsGASCV(string pno, DateTime dt1, DateTime dt2)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("GASCV_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[3].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY"))

                });
            }

            return cd;

        }
        public string PostRecordsGASCV(string pno, DateTime dt1, DateTime dt2, string Input_Value, string Input_Name)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(FROM_DATE) FROM GAS_CV WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
            if ((int)_context.GetScalerFromDB(query) == 0)
            {
                query = "INSERT INTO GAS_CV (FROM_DATE,TO_DATE," + Input_Name + " ,CREATED_BY,creation_time,INPUT_TYPE) values('" + dt1.Date() + "','" + dt2.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'M')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update GAS_CV SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE  WHERE FROM_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }
        //----------------------Energy Factor ----------------//

        public List<EnergyFactor> GetRecordsENERGYFACTOR(DateTime dt1, DateTime dt2)
        {
            string query = "select * from weekly_energy_factor where EFF_FROM_DATE='" + dt1.Date() + "' and EFF_TO_DATE='" + dt2.Date() + "' ORDER BY CREATION_DATETIME DESC";
            var data = _context.GetSQLQuery(query);

            List<EnergyFactor> cd = new List<EnergyFactor>();
            cd = data.AsEnumerable().Select(e => new EnergyFactor
            {

                FromDate = Convert.ToDateTime(e.Field<DateTime>("EFF_FROM_DATE")).ToString("dd/MM/yyyy"),
                ToDate = Convert.ToDateTime(e.Field<DateTime>("EFF_TO_DATE")).ToString("dd/MM/yyyy"),
                PrCode = e.Field<string>("PR_CODE"),
                PrValue = e.Field<double>("PR_VALUE"),
                EFFUnit = e.Field<string>("EFF_UNIT"),

            }).ToList();
           

            return cd;

        }
        public int SaveRecordsENERGYFACTOR(DateTime FromDate, DateTime ToDate, string Unit, string PrCode, string PrValue,string pno)
        {
            string query = "select pr_code from weekly_energy_factor where EFF_FROM_DATE = '" + FromDate.Date() + "'";
            if (_context.GetCharScalerFromDB(query).ToLower() == PrCode.ToLower())
            {
                return -1;
            }
            List<EnergyFactor> cd = new List<EnergyFactor>();
            query = "insert into weekly_energy_factor values('"+ FromDate.Date() + "','" + ToDate.Date() + "','" + PrCode + "','" + Unit + "','" + PrValue + "','" + pno + "',SYSDATE)";
            var i = _context.insertUpdateToDB(query);
            return i;

        }

        public string UpdateRecordsENERGYFACTOR(DateTime FromDate, DateTime ToDate, string Unit, string PrCode, string PrValue, string pno)
        {
            
            List<EnergyFactor> cd = new List<EnergyFactor>();
            string query = "update  weekly_energy_factor set EFF_FROM_DATE='" + FromDate.Date() + "',EFF_TO_DATE='" + ToDate.Date() + "',PR_CODE='" + PrCode + "',PR_UNIT='" + Unit + "',PR_VALUE='" + PrValue + "' WHERE EFF_FROM_DATE='" + FromDate.Date() + "'AND PR_CODE='" + PrCode + "'";
            var i = _context.insertUpdateToDB(query);
            return i.ToString();

        }
        //----------------------Energy Analysis ----------------//

        public List<CommonData> GetRecordsENERGYANALYSIS(DateTime dt1)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("ENERGY_ANALYSIS_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[1].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY"))

                });
            }

            return cd;

        }
        public string PostRecordsENERGYANALYSIS(DateTime dt1, string Input_Value, string Input_Name,string pno)
        {
            string alert = "";
            string query = "select count(*) from energy_analysis where ENG_DATA_DATE = '" + dt1.Date() + "'";
            if (_context.GetScalerFromDB(query) >0)
            {
                query = "update energy_analysis SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_DATETIME=SYSDATE  WHERE ENG_DATA_DATE='" + dt1.Date() +"'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            else
            {
                query = "INSERT INTO energy_analysis (ENG_DATA_DATE," + Input_Name + " ,CREATED_BY,creation_DATETIME) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE)";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }

            return alert;
        }

       
      


        /************************UREASC01*********************/
        public List<CommonData> GetRecordsUREASC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("UREASC01_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsUREASC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string SaveRecordsUREASC01(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_54", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_58", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_62", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_66", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsUREASC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string PostShutdownUREASC01(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return "";
        }
        public string PostTechRemarkUREASC01(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return "";
        }


        /************************UREASC02*********************/
        public List<CommonData> GetRecordsUREASC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("UREASC02_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[4].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string PostRecordsUREASC02(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OPERATION_TYPE", OracleDbType = OracleDbType.VarChar, Value = op });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string SaveRecordsUREASC02(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_54", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_58", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_62", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_66", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsUREASC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string PostShutdownUREASC02(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return "";
        }
        public string PostTechRemarkUREASC02(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return "";
        }


        /************************Month*********************/

        public List<CommonData> GetRecordsMONTH(string formName, string pno, DateTime dt1,DateTime dt2, String Gas)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_GAS", OracleDbType = OracleDbType.VarChar, Value = Gas });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("MONTH_QUERY", oracleParameterCollecion);



            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[5].Value).GetDataReader();


            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),
                    OperationType = reader.GetString(reader.GetOrdinal("OPERATION_TYPE")),

                });
            }

            return cd;

        }
        public string GetInputTYPE(DateTime dt1, DateTime dt2)
        {
            string inputtype="";

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "V_TYPE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedure("MONTH_SAVE_INPUT_TYPE", oracleParameterCollecion);
            inputtype = oracleParameterCollecion[2].Value.ToString();
            return inputtype;

        }
        public string PostRecordsMONTH(DateTime FromDate, DateTime ToDate, string pno, string gastype, string Input_Value, string Input_Name)
        {
            string alert = "";
            string query = @"SELECT Count(*) FROM MONTHLY_TECH_INPUT 
                            WHERE FROM_DATE ='"+ FromDate.Date() + "' AND" +
                            " TO_DATE ='" + ToDate.Date() + "' AND REVISED = 'N' and type_of_gas ='" + gastype + "' ";
            int i = _context.GetScalerFromDB(query);
            if (i>0)
            {
                query= "update MONTHLY_TECH_INPUT set "+ Input_Name + "='"+ Input_Value + "' ,CREATION_DATE=sysdate, CREATED_BY=" + pno + " WHERE FROM_DATE ='" + FromDate.Date() + "' AND" +
                            " TO_DATE ='" + ToDate.Date() + "' AND REVISED = 'N' and type_of_gas ='" + gastype + "' ";
                i = _context.insertUpdateToDB(query);

                if (i>0)
                {
                    alert = "updated";
                }
            }
            return alert;


        }
        public string SaveRecordsMONTH(string formName, string shift, string pno, DateTime dt, string Input_Value, string Input_Name, string op)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_54", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_58", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_62", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_A1_66", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_SAVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string ApproveRecordsMONTH(string formName, string pno, DateTime dt1, DateTime dt2, string Gas)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_GAS", OracleDbType = OracleDbType.VarChar, Value = Gas });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("MONTH_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[4].Value.ToString();
            return alert;

        }
        public string PostShutdownMONTH(string Shift, DateTime DataDate, string Reason, string ReasonCode, string sd_plant, DateTime? FromDate, DateTime? ToDate, string FormName, string Pno, string InputType)
        {

            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_TYPE", OracleDbType = OracleDbType.VarChar, Value = InputType });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = Pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARK_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonCode });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_PLANT", OracleDbType = OracleDbType.VarChar, Value = sd_plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = Reason });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_FROM", OracleDbType = OracleDbType.VarChar, Value = FromDate != null ? FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "NULL" });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_TO", OracleDbType = OracleDbType.VarChar, Value = ToDate != null ? ToDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : FromDate.Value.ToString("MM/dd/yyyy HH:mm:ss") });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return "";
        }
        public string PostTechRemarkMONTH(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return "";
            //tech///
        }

        //---------------------------REMARK-----------------------------------//
        public List<CommonData> GetRecordsREMARK(DateTime dt1)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("REMARK_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[1].Value).GetDataReader();

            List<CommonData> cd = new List<CommonData>();
            while (reader.Read())
            {
                cd.Add(new CommonData()
                {
                    InputLabel = reader.GetString(reader.GetOrdinal("INPUT_LABEL")),
                    InputValue = reader.GetString(reader.GetOrdinal("INPUT_VALUE")),
                    InputText = reader.GetString(reader.GetOrdinal("INPUT_TEXT")),
                    InputType = reader.GetString(reader.GetOrdinal("INPUT_TYPE")),
                    IsReadonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Category = reader.GetString(reader.GetOrdinal("CATEGORY")),
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY"))

                });
            }

            return cd;

        }
        public string PostRecordsREMARK(string pno, DateTime dt1, string Input_Value, string Input_Name)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(DATA_DATE) FROM GAS_CV WHERE DATA_DATE='" + dt1.Date() + "'";
            if ((int)_context.GetScalerFromDB(query) == 0)
            {
                query = "INSERT INTO TECH_REMARKS (DATA_DATE," + Input_Name + " ,CREATED_BY,creation_datetime) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE)";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update TECH_REMARKS SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_datetime=SYSDATE  WHERE DATA_DATE='" + dt1.Date() + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }

    }
}
