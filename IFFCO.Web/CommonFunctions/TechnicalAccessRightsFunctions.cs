using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using IFFCO.TECHPROD.Web.Models;
using IFFCO.TECHPROD.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class TechnicalAccessRightsFunctions
    {

        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        //IDataContextAsync context;
        private readonly ModelContext _context;
        private readonly string ProjectId = string.Empty;

        DataTable _dt = new DataTable();

        public TechnicalAccessRightsFunctions()
        {
            _context = new ModelContext();
            ProjectId = new AppConfiguration().ProjectId;
        }

        public List<AdmEmpUnitAccess> GetUnitAccessDetail(string empid, string proj)
        {
            //Returns warehouse codes from distinct V_M_WAREHOUSE district code and V_M_DISTRICT
            // Used in - DISC01Controller.cs

            string sqlquery = " SELECT DISTINCT UNIT_CODE,ALL_DEPT_ACCESS, ALL_SECTION_ACCESS, DEFAULT_UNIT, EMPID, HIER_YN, ONLY_AREA_ACCESS, PROJECTID FROM ADM_EMP_UNIT_ACCESS WHERE EMPID = '" + empid + "' AND PROJECTID = '" + proj + "'  ";


            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<AdmEmpUnitAccess> DRP_VALUE = new List<AdmEmpUnitAccess>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new AdmEmpUnitAccess()
                         {
                             UnitCode = Convert.ToInt32(dr["UNIT_CODE"]),
                             AllDeptAccess = Convert.ToString(dr["ALL_DEPT_ACCESS"]),
                             AllSectionAccess = Convert.ToString(dr["ALL_SECTION_ACCESS"]),
                             CreatedBy = "Old",
                             DefaultUnit = Convert.ToString(dr["DEFAULT_UNIT"]),
                             Empid = Convert.ToInt32(dr["EMPID"]),
                             HierYn = Convert.ToString(dr["HIER_YN"]),
                             OnlyAreaAccess = Convert.ToString(dr["ONLY_AREA_ACCESS"]),
                         }).ToList();

            return DRP_VALUE;

        }

        public ADMSC01ViewModel GetEmployeeDetailForBind(int empid)
        {
            string sqlquery = " SELECT UNIT_CODE UNIT, PERSONAL_NO, EMP_NAME, GRADE_CODE, DESIGNATION, DEPARTMENT, SECTION FROM V_EB_EMPLOYEE_COMPLETE_DTLS  ";
            sqlquery += " WHERE PERSONAL_NO = '" + empid + "' ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<ADMSC01ViewModel> DRP_VALUE = new List<ADMSC01ViewModel>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new ADMSC01ViewModel()
                         {
                             PersonnelNumber = Convert.ToInt32(dr["PERSONAL_NO"]),
                             EmployeeName = Convert.ToString(dr["EMP_NAME"]),
                             Department = Convert.ToString(dr["DEPARTMENT"]),
                             Designation = Convert.ToString(dr["DESIGNATION"]),
                             Section = Convert.ToString(dr["SECTION"]),
                             Grade = Convert.ToString(dr["GRADE_CODE"]),
                             Unit = Convert.ToString(dr["UNIT"])
                         }).ToList();

            return DRP_VALUE.FirstOrDefault();

        }

        public List<ModuleTableForBind> GetModuleKeyValue(string empid)
        {
            //Returns warehouse codes from distinct V_M_WAREHOUSE district code and V_M_DISTRICT
            // Used in - DISC01Controller.cs

            string sqlquery = " SELECT DISTINCT KEY, MAX(PNO), MAX(NAME) Name, MAX(VALUE) val FROM ( SELECT DISTINCT MODULEID KEY, MODULENAME NAME, " + empid + " PNO, 'N' VALUE   ";
            sqlquery += " FROM ADM_PROJMOD_MASTER  WHERE PROJECTID = 'DAILYWG' UNION ";
            sqlquery += " SELECT DISTINCT MODULEID KEY,'' NAME, EMPID PNO, 'Y' VALUE FROM ADM_EMP_UNIT_ACCESS WHERE EMPID =  " + empid + " and PROJECTID = 'DAILYWG') GROUP BY KEY ORDER BY KEY  ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<ModuleTableForBind> DRP_VALUE = new List<ModuleTableForBind>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new ModuleTableForBind()
                         {
                             Key = Convert.ToString(dr["KEY"]),
                             Name = Convert.ToString(dr["Name"]),
                             Value = ((Convert.ToString(dr["val"]) == "Y") ? true : false)
                         }).ToList();

            return DRP_VALUE;

        }

        public List<EmpProgramAccess> GetAllEmployeeAccessDetails(int unit, string mod, string program, string proj)
        {
            //Returns warehouse codes from distinct V_M_WAREHOUSE district code and V_M_DISTRICT
            // Used in - DISC01Controller.cs

            string sqlquery = " SELECT A.EMPID,B.EMP_NAME NAME,A.MODULEID, D.MODULENAME, A.PROGRAMID,C.PROGRAMNAME, A.PRIV_SELECT, A.PRIV_INSERT, A.PRIV_UPDATE, A.PRIV_DELETE,B.UNIT_CODE    ";
            sqlquery += " FROM ADM_EMPPRG_ACCESS A,V_EB_EMPLOYEE_COMPLETE_DTLS B, ADM_PRG_MASTER C, ADM_PROJMOD_MASTER D ";
            sqlquery += " WHERE A.EMPID = B.PERSONAL_NO AND A.PROGRAMID = C.PROGRAMID AND A.MODULEID = D.MODULEID AND A.PROJECTID = C.PROJECTID AND B.UNIT_CODE LIKE '" + unit + "'  ";
            sqlquery += " AND A.MODULEID LIKE '%" + mod + "%' AND A.PROGRAMID LIKE '%" + program + "%' AND A.PROJECTID LIKE '%" + proj + "%' ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<EmpProgramAccess> DRP_VALUE = new List<EmpProgramAccess>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new EmpProgramAccess()
                         {
                             PersonnelNo = Convert.ToString(dr["EMPID"]),
                             Name = Convert.ToString(dr["NAME"]),
                             ModuleID = Convert.ToString(dr["MODULEID"]),
                             ModuleName = Convert.ToString(dr["MODULENAME"]),
                             ProgramID = Convert.ToString(dr["PROGRAMID"]),
                             ProgramName = Convert.ToString(dr["PROGRAMNAME"]),
                             SelectAccess = Convert.ToString(dr["PRIV_SELECT"]),
                             InsertAccess = Convert.ToString(dr["PRIV_INSERT"]),
                             UpdateAccess = Convert.ToString(dr["PRIV_UPDATE"]),
                             DeleteAccess = Convert.ToString(dr["PRIV_DELETE"]),
                             UnitCode = Convert.ToInt32(dr["UNIT_CODE"]),
                         }).ToList();

            return DRP_VALUE;

        }

        public List<AdmEmpprgAccess> GetEmployeeAccessDetails(string empid, string mod, string progtype, string proj)
        {
            //Returns warehouse codes from distinct V_M_WAREHOUSE district code and V_M_DISTRICT
            // Used in - DISC01Controller.cs

            string sqlquery = " select distinct MODULEID,PROGRAMID,PROGRAMNAME, max(EMPID) EMPID,max(PRIV_DELETE) PRIV_DELETE, max(PRIV_INSERT) PRIV_INSERT, max(PRIV_SELECT) PRIV_SELECT, max(PRIV_UPDATE) PRIV_UPDATE, projectid,PROGRAMTYPE   ";
            sqlquery += " from (SELECT A.MODULEID, A.PROGRAMID,a.PROGRAMNAME, B.EMPID, B.PRIV_DELETE, B.PRIV_INSERT, B.PRIV_SELECT, B.PRIV_UPDATE, a.projectid, A.PROGRAMTYPE FROM ADM_PRG_MASTER A, ADM_EMPPRG_ACCESS B WHERE A.MODULEID = B.MODULEID(+) ";
            sqlquery += " AND A.PROGRAMID = B.PROGRAMID(+) AND B.EMPID = '" + empid + "' AND A.MODULEID = '" + mod + "' AND A.PROJECTID = '" + proj + "' and a.PROGRAMTYPE = '" + progtype + "'  ";
            sqlquery += " union select moduleid,programid,PROGRAMNAME,'" + empid + "' empid, '' PRIV_DELETE, '' PRIV_INSERT, '' PRIV_SELECT, '' PRIV_UPDATE,projectid, PROGRAMTYPE  frOM ADM_PRG_MASTER WHERE MODULEID = '" + mod + "' AND PROJECTID = '" + proj + "' and PROGRAMTYPE = '" + progtype + "' ) group by projectid,MODULEID,PROGRAMTYPE,PROGRAMID,PROGRAMNAME order by projectid,moduleid,PROGRAMTYPE, programid ";

            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<AdmEmpprgAccess> DRP_VALUE = new List<AdmEmpprgAccess>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new AdmEmpprgAccess()
                         {
                             Empid = Convert.ToString(dr["EMPID"]),
                             Projectid = Convert.ToString(dr["PROGRAMNAME"]),
                             Moduleid = Convert.ToString(dr["MODULEID"]),
                             Programid = Convert.ToString(dr["PROGRAMID"]),
                             PrivSelect = Convert.ToString(dr["PRIV_SELECT"]),
                             PrivInsert = Convert.ToString(dr["PRIV_INSERT"]),
                             PrivUpdate = Convert.ToString(dr["PRIV_UPDATE"]),
                             PrivDelete = Convert.ToString(dr["PRIV_DELETE"]),
                             Programtype = Convert.ToString(dr["PROGRAMTYPE"])
                         }).ToList();

            return DRP_VALUE;

        }

    }
}
