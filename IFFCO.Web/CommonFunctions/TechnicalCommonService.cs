using Devart.Data.Oracle;
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
            return _context.GetCharScalerFromDB("SELECT Get_Time_Based_Screen_Acess(" + pno + ", '" + formName + "','" + dt.Date() + "') FROM DUAL ");
        }
        public double GetFrValue(DateTime dt)
        {
            return Convert.ToDouble(_context.GetCharScalerFromDB(@"SELECT FR_VALUE FROM FACTOR_MASTER WHERE FR_CODE = 'AMSTF' AND '" + dt.Date() + "' BETWEEN NVL(EFFECTIVE_FROM_DATE, '01/JAN/1900') AND NVL(EFFECTIVE_TO_DATE, SYSDATE) "));
        }

        public List<SelectListItem> GetPlantList()
        {
            List<SelectListItem> selectListItem = new List<SelectListItem>();
            selectListItem.Add(new SelectListItem() { Text = "Ammonia-I(B.L)", Value = "AM1_EN" });
            selectListItem.Add(new SelectListItem() { Text = "Ammonia-II(B.L)", Value = "AM2_EN" });
            selectListItem.Add(new SelectListItem() { Text = "Urea( Aonla-I)", Value = "UR1_EN" });
            selectListItem.Add(new SelectListItem() { Text = "Urea( Aonla-II)", Value = "UR2_EN" });
            selectListItem.Add(new SelectListItem() { Text = "Urea( Aonla-I&II)", Value = "UR12_EN" });


            return selectListItem;
        }
        public Employee GetEmployeeDetails(string pno)
        {
            Employee employee = new Employee();
                
            string query = "select personal_no,emp_name,designation from VW_EMPLOYEE_COMPLETE_DTLS_TBL where personal_no='" + pno + "'";
            var data = _context.GetSQLQuery(query);
            employee = data.AsEnumerable().Select(e => new Employee {
                Pno = e.Field<int>("personal_no").ToString(),
                Name = e.Field<string>("emp_name"),
                Designation = e.Field<string>("designation"),
            }).FirstOrDefault();


            return employee;
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
            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsAMMSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction=ParameterDirection.Output    });
            var data = _context.ExecuteProcedureForRefCursor("PHSC01_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsPHSC01(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("PHSC02_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsPHSC02(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction=ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("ELECTRICAL_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsELECTRICAL1(string formName, string shift, string pno, DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SHIFT", OracleDbType = OracleDbType.VarChar, Value = shift });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FORM_NAME", OracleDbType = OracleDbType.VarChar, Value = formName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
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
            bool isNHLV = (int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'") > 0;
            bool isTEMPNHLV = (int)_context.GetScalerFromDB("SELECT COUNT(DATA_DATE) FROM TEMP_NGLHV WHERE DATA_DATE = '" + dt.Date() + "' and input_type = 'D'") == 0;
            if (isNHLV && isTEMPNHLV)
            {
                query= "INSERT INTO TEMP_NGLHV SELECT* FROM NGLHV WHERE DATA_DATE =  '" + dt.Date() + "'";
                _context.insertUpdateToDB(query);

            }
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[9].Value.ToString();
            return alert;
        }

        public string PostTechRemarkPWRSC01(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("PWRSC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[5].Value.ToString();
            return alert;
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

        public string SUMRecordsGASCV(string pno, DateTime dt1, DateTime dt2)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("GASCV_SUM", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
            return alert;

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
            string query = "select count(*) from weekly_energy_factor where EFF_FROM_DATE = '" + FromDate.Date() + "' and pr_code='"+PrCode+"'";
            if (_context.GetScalerFromDB(query)>0)
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
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[9].Value.ToString();
            return alert;
        }
        public string PostTechRemarkUREASC01(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC01_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[5].Value.ToString();
            return alert;
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
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),

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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[7].Value.ToString();
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
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
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_PLANT_SHUTDOWN_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[9].Value.ToString();
            return alert;
        }
        public string PostTechRemarkUREASC02(string Shift, DateTime DataDate, string ReasonName, string RemarksValue, string pno, string FormName)
        {


            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = DataDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REASON_NAME", OracleDbType = OracleDbType.VarChar, Value = ReasonName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_SD_REMARKS", OracleDbType = OracleDbType.VarChar, Value = RemarksValue });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_NAME", OracleDbType = OracleDbType.VarChar, Value = FormName });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("UREASC02_TECH_REMARK_POST", oracleParameterCollecion);
            string alert = oracleParameterCollecion[5].Value.ToString();
            return alert;
        }


        /************************Month_DATA*********************/

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
        public string SumRecordsMONTH(DateTime dt1, DateTime dt2, string Gas)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_GAS", OracleDbType = OracleDbType.VarChar, Value = Gas });           
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("MONTH_SUM", oracleParameterCollecion);
            string alert = oracleParameterCollecion[3].Value.ToString();
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
        public List<CommonData> GetRecordsREMARK(DateTime dt1,string pno )
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATA_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() }); 
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
            query = "SELECT COUNT(*) FROM TECH_REMARKS WHERE DATA_DATE='" + dt1.Date() + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
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
        //---------------------------ENERGY RECORD-----------------------------------//
        public List<ENERGYRECORD> GetRecordsENERGYRECORD()
        {
            List<ENERGYRECORD> energy = new List<ENERGYRECORD>();
            string query = "select * from energy_record order by creation_datetime desc";
            var data = _context.GetSQLQuery(query);
            energy = data.AsEnumerable().Select(e => new ENERGYRECORD
            {
                Plant=e.Field<string>("OP_code"),
                ForMonth=Convert.ToString(e.Field<double?>("FOR_MONTH")),
                UptoMonth=Convert.ToString(e.Field<double?>("UPTO_MONTH")),
                Period=e.Field<string>("PERIOD"),

            }).ToList();
            return energy;
        }

        public int SaveRecordsENERGYRECORD(string Plant, string ForMonth, string UptoMonth, string Period,string pno)
        {
            string query = "select count(*) from energy_record where period = '" + Period + "' and op_code='"+Plant+"'";
            if (_context.GetScalerFromDB(query)>0)
            {
                return -1;
            }
           
            query = "insert into energy_record values('" + Plant + "','" + Period + "','" + ForMonth + "','" + UptoMonth + "','" + pno + "',SYSDATE)";
            var i = _context.insertUpdateToDB(query);
            return i;

        }


        //---------------------------SMS ENTRY-----------------------------------//
      



        public List<SMSENTRY> GetRecordsSMSENTRY(DateTime dt)
        {
            List<SMSENTRY> energy = new List<SMSENTRY>();
            string query = "select * from monthly_sms_overtime where till_date='"+dt.Date()+"' order by s_no desc";
            var data = _context.GetSQLQuery(query);
            energy = data.AsEnumerable().Select(e => new SMSENTRY
            {
                S_SNO = e.Field<int>("s_no"),
                P_NO = Convert.ToString(e.Field<int?>("p_no")),               
                NAME = e.Field<string>("ename"),
                DESIGNATION = e.Field<string>("designation"),
                SMS_AMOUNT = Convert.ToString(e.Field<double?>("sms_amount")),
                TILL_DATE = e.Field<DateTime?>("till_date"),

            }).ToList();
            return energy;
        }

        public int SaveRecordsSMSENTRY(string Sno, string Pno, string Name, string Desg, string Sms,DateTime d1,string createdby)
        {
            string query = "select count(*) from monthly_sms_overtime where till_date = '" + d1.Date() + "' and p_no='" + Pno + "'";
            if (_context.GetScalerFromDB(query) > 0)
            {
                return -1;
            }
         
            query = "insert into monthly_sms_overtime(S_no,P_no,EName,Designation,  Sms_amount,  till_date, created_by,creation_datetime) values('" + Sno + "','" + Pno + "','" + Name + "','" + Desg + "','" + Sms + "','" + d1.Date() + "','" + createdby + "',SYSDATE)";
            var i = _context.insertUpdateToDB(query);
            return i;

        }



        //---------------------------FACTOR MASTER-----------------------------------//
        public List<SelectListItem> GetFactorList()
        {
            var data = _context.GetSQLQuery("SELECT DISTINCT  FACTOR_MASTER.FR_CODE, FACTOR_MASTER.FR_NAME FROM FACTOR_MASTER WHERE fr_code in ('AMMSTEN', 'NGCOST', 'NAPCOST', 'HPSTCOST', 'POWCOST', 'STF', 'TOTAL_EN','NEEMCOST')");
            return data.AsEnumerable().Select(e => new SelectListItem
            {
                Text = e.Field<string>("FR_NAME"),
                Value = e.Field<string>("FR_CODE"),
            }).ToList();

        }
        public List<FactorMaster> GetRecordsFACTORMASTER(string FrCode)
        {
            List<FactorMaster> energy = new List<FactorMaster>();
            string query = "select * from factor_master where fr_code='"+FrCode+"' order by Effective_From_Date desc";
            var data = _context.GetSQLQuery(query);
            energy = data.AsEnumerable().Select(e => new FactorMaster
            {
                FrCode = e.Field<string>("Fr_Code"),
                FrName =e.Field<string>("Fr_Name"),
                FrUnit = e.Field<string>("Fr_Unit"),
                FrValue = e.Field<decimal?>("Fr_Value"),
                EffectiveFromDate = e.Field<DateTime>("Effective_From_Date"),
                EffectiveToDate = e.Field<DateTime?>("Effective_To_Date"),

            }).ToList();
            return energy;
        }

        public int SaveRecordsFACTORMASTER(string Code, string Unit, string Name, string Value, DateTime FromDate, DateTime? ToDate, string createdby)
        {
            string query = "select count(*) from factor_master where Effective_From_Date = '" + FromDate.Date() + "' and Fr_Code='" + Code + "'";
            if (_context.GetScalerFromDB(query) > 0)
            {
                return -1;
            }
            if (ToDate is null)
            {
                query = "insert into factor_master(Fr_Code,Fr_Name,Fr_Unit,Fr_Value,  Effective_From_Date,created_by,creation_datetime) values('" + Code + "','" + Name + "','" + Unit + "','" + Value + "','" + FromDate.Date() + "','" + createdby + "',SYSDATE)";

            }
            else
            {
                query = "insert into factor_master(Fr_Code,Fr_Name,Fr_Unit,Fr_Value,  Effective_From_Date,Effective_To_Date,created_by,creation_datetime) values('" + Code + "','" + Name + "','" + Unit + "','" + Value + "','" + FromDate.Date() + "','" + ToDate.Value.Date() + "','" + createdby + "',SYSDATE)";

            }
            var i = _context.insertUpdateToDB(query);
            return i;

        }
        public int UpdateRecordsFACTORMASTER(string Code, string Unit, string Name, string Value, DateTime FromDate, DateTime? ToDate, string createdby)
        {
            string query = "";
            if (ToDate is null)
            {
              query = "update factor_master set Fr_Unit='"+Unit+"',Fr_Value='"+Value+ "', created_by='" + createdby + "',creation_datetime=SYSDATE where Effective_From_Date = '" + FromDate.Date() + "' and Fr_Code='" + Code + "'";

            }
            else
            {
                 query = "update factor_master set Fr_Unit='" + Unit + "',Fr_Value='" + Value + "',Effective_To_Date='" + ToDate.Value.Date() + "', created_by='" + createdby + "',creation_datetime=SYSDATE where Effective_From_Date = '" + FromDate.Date() + "' and Fr_Code='" + Code + "'";

            }


            var i = _context.insertUpdateToDB(query);
            return i;

        }


        //---------------------------TOP 3-----------------------------------//
        public List<TOP3Data> GetRecordsTOP3(string fyear, string plant)
        {
            List<TOP3Data> energy = new List<TOP3Data>();
            string query = "select * from top3_data where fin_year='"+fyear+"' and plant_unit='"+plant+"' order by s_no desc";
            var data = _context.GetSQLQuery(query);
            energy = data.AsEnumerable().Select(e => new TOP3Data
            {
                S_NO = e.Field<int>("S_NO"),
                PLANT_CATALYST = e.Field<string>("PLANT_CATALYST"),
                TYPE = e.Field<string>("TYPE"),
                SUPPLIER = e.Field<string>("SUPPLIER"),
                QTY = e.Field<double?>("QTY"),
                DENSITY = e.Field<double?>("DENSITY"),
                LIFE_GURANTEED = e.Field<double?>("LIFE_GURANTEED"),
                CHARG_DATE = e.Field<string>("CHARG_DATE"),
                REPLACE_DATE = e.Field<string>("REPLACE_DATE"),
                EXPECTED_LIFE = e.Field<string>("EXPECTED_LIFE"),
                PRE_CHARGE_DATE = e.Field<string>("PRE_CHARGE_DATE"),
                PRE_REPLACE_DATE = e.Field<string>("PRE_REPLACE_DATE"),

            }).ToList();
            return energy;
        }

        public int SaveRecordsTOP3(string Sno, string Plant_cat, string Type, string Supplier,string Density, string Qty, string Life, string CDate, string RDate, string ELife, string PCDate, string PRDate,string finyear,string plant)
        {
            string query = "select count(*) from top3_data where s_no = '" + Sno + "' and Plant_unit='" + plant + "' and Fin_year='"+finyear+"'";
            if (_context.GetScalerFromDB(query) > 0)
            {
                return -1;
            }
            
            else
            {
                query = @"insert into top3_data(FIN_YEAR, PLANT_UNIT, S_NO, 
                          PLANT_CATALYST, TYPE, SUPPLIER, 
                          QTY, DENSITY, LIFE_GURANTEED, 
                          CHARG_DATE, REPLACE_DATE, EXPECTED_LIFE, 
                          PRE_CHARGE_DATE, PRE_REPLACE_DATE) 
                          values('" + finyear + "','" + plant + "','" + Sno + "','" + Plant_cat + "','" + Type + "','" + Supplier + "','" + Qty + "'," +
                          "'" + Density + "','" + Life + "','" + CDate + "','" + RDate + "','" + ELife + "','" + PCDate + "','"+PRDate+"')";

            }
            var i = _context.insertUpdateToDB(query);
            return i;

        }
        public int UpdateRecordsTOP3(string Sno, string Plant_cat, string Type, string Supplier, string Density, string Qty, string Life, string CDate, string RDate, string ELife, string PCDate, string PRDate, string finyear, string plant)
        {
            string query = "select count(*) from top3_data where s_no = '" + Sno + "' and Plant_unit='" + plant + "' and Fin_year='" + finyear + "'";
            
                query = @"update top3_data set PLANT_CATALYST='" + Plant_cat + "',TYPE='" + Type + "',SUPPLIER='" + Supplier + "'," +
                    "QTY='" + Qty + "',DENSITY='" + Density + "',LIFE_GURANTEED='" + Life + "'," +
                    "CHARG_DATE='" + CDate + "',REPLACE_DATE='" + RDate + "',EXPECTED_LIFE='" + ELife + "'," +
                    "PRE_CHARGE_DATE='" + PCDate + "'," +
                    "PRE_REPLACE_DATE='" + PRDate + "' where s_no = '" + Sno + "' and Plant_unit='" + plant + "' and Fin_year='" + finyear + "'";
            
            var i = _context.insertUpdateToDB(query);
            return i;

        }

        public string COPYRecordsTOP3(string refyear, string fyear)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FYEAR", OracleDbType = OracleDbType.VarChar, Value = fyear });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_REFYEAR", OracleDbType = OracleDbType.VarChar, Value = refyear});
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP3_COPY", oracleParameterCollecion);

            string alert = oracleParameterCollecion[2].Value.ToString();
            return alert;
        }

        //---------------------------TOP 18-----------------------------------//
        public List<TOP18Data> GetRecordsTOP18(string fyear, string plant)
        {
            List<TOP18Data> energy = new List<TOP18Data>();
            string query = "select * from CHEMICALS_DATA where FN_YR='" + fyear + "' and PLANT_UNIT='" + plant + "' order by S_NO desc";
            var data = _context.GetSQLQuery(query);
            energy = data.AsEnumerable().Select(e => new TOP18Data
            {
                S_NO = e.Field<int>("S_NO"),
                CATALYST = e.Field<string>("CATALYST"),
                UNITS = e.Field<string>("UNITS"),
                APR = e.Field<double?>("APR"),
                MAY = e.Field<double?>("MAY"),
                JUN = e.Field<double?>("JUN"),
                JUL = e.Field<double?>("JUL"),
                AUG = e.Field<double?>("AUG"),
                SEP = e.Field<double?>("SEP"),
                OCT = e.Field<double?>("OCT"),
                NOV = e.Field<double?>("NOV"),
                DEC = e.Field<double?>("DEC"),
                JAN = e.Field<double?>("JAN"),
                FEB = e.Field<double?>("FEB"),
                MAR = e.Field<double?>("MAR"),


            }).ToList();
            return energy;
        }

        public int SaveRecordsTOP18(string Sno, string Plant_cat, string Uom, string Pno, string APR, string MAY, string JUN, string JUL, string AUG, string SEP, string OCT, string NOV, string DEC, string JAN, string FEB, string MAR, string finyear, string plant)
        {
            string query = "select count(*) from CHEMICALS_DATA where S_NO = '" + Sno + "' and PLANT_UNIT='" + plant + "' and FN_YR='" + finyear + "'";
            if (_context.GetScalerFromDB(query) > 0)
            {
                return -1;
            }

            else
            {
                query = @"insert into CHEMICALS_DATA(FN_YR, S_NO, CATALYST, UNITS, CREATED_BY, CREATION_TIME, PLANT_UNIT, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC, JAN, FEB, MAR) 
                          values('" + finyear + "','" + Sno + "','" + Plant_cat + "','" + Uom + "','" + Pno + "',SYSDATE,'" + plant + "'," +
                          "'" + APR + "','" + MAY + "','" + JUN + "','" + JUL + "','" + AUG + "','" + SEP + "','" + OCT + "','" + NOV + "','" + DEC + "','" + JAN + "','" + FEB + "','" + MAR + "')";

            }

            var i = _context.insertUpdateToDB(query);
            return i;

        }

        public int UpdateRecordsTOP18(string Sno, string Plant_cat, string Uom, string Pno, string APR, string MAY, string JUN, string JUL, string AUG, string SEP, string OCT, string NOV, string DEC, string JAN, string FEB, string MAR, string finyear, string plant)
        {
            string query = "select count(*) from CHEMICALS_DATA where S_NO = '" + Sno + "' and PLANT_UNIT='" + plant + "' and FN_YR='" + finyear + "'";

            query = @"update CHEMICALS_DATA set CATALYST='" + Plant_cat + "',UNITS='" + Uom + "',APR='" + APR + "'," +
                "MAY='" + MAY + "',JUN='" + JUN + "',JUL='" + JUL + "'," +
                "AUG='" + AUG + "',SEP='" + SEP + "',OCT='" + OCT + "'," +
                "NOV='" + NOV + "'," +
                "DEC='" + DEC + "',JAN='" + JAN + "',FEB='" + FEB + "',MAR='" + MAR + "' where s_no = '" + Sno + "' and Plant_unit='" + plant + "' and FN_YR='" + finyear + "'";

            var i = _context.insertUpdateToDB(query);
            return i;

        }

        //---------------------------EREMARK-----------------------------------//
        public List<CommonData> GetRecordsEREMARK(DateTime dt1, DateTime dt2, string pno)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_FR_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.VarChar, Value = dt2.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("EREMARK_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[2].Value).GetDataReader();

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
        public string PostRecordsEREMARK(string pno, DateTime dt1, DateTime dt2, string Input_Value, string Input_Name)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(" + Input_Name + ") FROM ENERGY_REMARKS WHERE FR_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO ENERGY_REMARKS (FR_DATE, TO_DATE," + Input_Name + " ,CREATED_BY,creation_datetime) values('" + dt1.Date() + "','" + dt2.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE)";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update ENERGY_REMARKS SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_datetime=SYSDATE WHERE FR_DATE='" + dt1.Date() + "' AND TO_DATE='" + dt2.Date() + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                } 
            }
            return alert;

        }
        //---------------------------TOP9AMM-----------------------------------//
        public List<CommonData> GetRecordsTOP9AMM(DateTime dt1, string plant)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date()});
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PLANT", OracleDbType = OracleDbType.VarChar, Value = plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP9_AMM_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[2].Value).GetDataReader();

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

        public string PostRecordsTOP9AMM(DateTime dt1, string plant,  string Input_Name, string Input_Value, string pno)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(*) FROM top9_data WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO top9_data (data_date," + Input_Name + " ,CREATED_BY,creation_time,PLANT) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'"+plant+"')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update top9_data SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }

        //---------------------------TOP9UREA-----------------------------------//
        public List<CommonData> GetRecordsTOP9UREA(DateTime dt1, string plant)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PLANT", OracleDbType = OracleDbType.VarChar, Value = plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP9_UREA_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[2].Value).GetDataReader();

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

        public string PostRecordsTOP9UREA(DateTime dt1, string plant, string Input_Name, string Input_Value, string pno)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(*) FROM top9_data WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO top9_data (data_date," + Input_Name + " ,CREATED_BY,creation_time,PLANT) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'" + plant + "')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update top9_data SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }

        //---------------------------TOP9STEAM-----------------------------------//
        public List<CommonData> GetRecordsTOP9STEAM(DateTime dt1, string plant)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            //oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PLANT", OracleDbType = OracleDbType.VarChar, Value = plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP9_SG_QUERY", oracleParameterCollecion);

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

        public string PostRecordsTOP9STEAM(DateTime dt1, string plant, string Input_Name, string Input_Value, string pno)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(*) FROM top9_data WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO top9_data (data_date," + Input_Name + " ,CREATED_BY,creation_time,PLANT) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'" + plant + "')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted"; 
                }
            }
            else
            {
                query = "update top9_data SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }




        //---------------------------TOP9POWER-----------------------------------//

        public List<CommonData> GetRecordsTOP9CPP(DateTime dt1, string plant)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP9_CPP_QUERY", oracleParameterCollecion);

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

        public string PostRecordsTOP9CPP(DateTime dt1, string plant, string Input_Name, string Input_Value, string pno)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(*) FROM top9_data WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO top9_data (data_date," + Input_Name + " ,CREATED_BY,creation_time,PLANT) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'" + plant + "')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update top9_data SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }



        //---------------------------TOP9LAB-----------------------------------//

        public List<CommonData> GetRecordsTOP9LAB(DateTime dt1, string plant)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt1.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PLANT", OracleDbType = OracleDbType.VarChar, Value = plant });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("TOP9_LAB_QUERY", oracleParameterCollecion);

            OracleDataReader reader = ((OracleCursor)oracleParameterCollecion[2].Value).GetDataReader();

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

        public string PostRecordsTOP9LAB(DateTime dt1, string plant, string Input_Name, string Input_Value, string pno)
        {

            string query = "";
            string alert = "";
            query = "SELECT COUNT(*) FROM top9_data WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
            int RES = _context.GetScalerFromDB(query);
            if (RES == 0)
            {
                query = "INSERT INTO top9_data (data_date," + Input_Name + " ,CREATED_BY,creation_time,PLANT) values('" + dt1.Date() + "','" + Input_Value + "','" + pno + "',SYSDATE,'" + plant + "')";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Inserted";
                }
            }
            else
            {
                query = "update top9_data SET " + Input_Name + "='" + Input_Value + "' ,CREATED_BY='" + pno + "',creation_time=SYSDATE WHERE data_date='" + dt1.Date() + "' AND plant='" + plant + "'";
                var i = _context.insertUpdateToDB(query);
                if (i > 0)
                {
                    alert = "Updated";
                }
            }
            return alert;

        }


        //-----------NAPHTA-------------------//

        public List<CommonData> GetRecordsNAPHTA(DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });

            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_RESPONSE_CUR", OracleDbType = OracleDbType.Cursor, Direction = ParameterDirection.Output });

            var data = _context.ExecuteProcedureForRefCursor("NAPHTHA_QUERY", oracleParameterCollecion);



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
                    Readonly = reader.GetString(reader.GetOrdinal("READONLY")),
                    Layout = reader.GetString(reader.GetOrdinal("LAYOUT")),

                });
            }

            return cd;

        }

        public string PostRecordsNAPHTA(string Cat, string Label, string Unit, DateTime FromDate, string Input_Name, string Input_Value, string InputType, string pno)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = FromDate.Date() });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_PNO", OracleDbType = OracleDbType.VarChar, Value = pno });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_CAT", OracleDbType = OracleDbType.VarChar, Value = Cat });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_LABEL", OracleDbType = OracleDbType.VarChar, Value = Label });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_UNIT", OracleDbType = OracleDbType.VarChar, Value = Unit });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_NAME", OracleDbType = OracleDbType.VarChar, Value = Input_Name });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_INPUT_VALUE", OracleDbType = OracleDbType.VarChar, Value = Input_Value });
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("NAPHTHA_POST", oracleParameterCollecion);

            string alert = oracleParameterCollecion[7].Value.ToString();
            return alert;

        }
        public string ApproveRecordsNAPHTA(DateTime dt)
        {
            List<OracleParameter> oracleParameterCollecion = new List<OracleParameter>();
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_DATE", OracleDbType = OracleDbType.VarChar, Value = dt.Date() });           
            oracleParameterCollecion.Add(new OracleParameter() { ParameterName = "P_OUTPUT_MESSAGE", OracleDbType = OracleDbType.VarChar, Direction = ParameterDirection.Output });
            var data = _context.ExecuteProcedureForRefCursor("NAPHTHA_APPROVE", oracleParameterCollecion);
            string alert = oracleParameterCollecion[1].Value.ToString();
            return alert;

        }





    }
}
