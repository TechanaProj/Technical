using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using IFFCO.HRMS.Shared.CommonFunction;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class DropDownListBindWeb : DropDownListBind
    {
        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        //IDataContextAsync context;
        private readonly ModelContext _context;
        DataTable _dt = new DataTable();
        public DropDownListBindWeb()
        {
            _context = new ModelContext();
            // context = modelContext;
            //_unitOfWork = new UnitOfWork(_context, _repositoryProvider);
        }

        public List<SelectListItem> GetSuggestionUnitWithSecurity(string empid, string moduleid)
        {
            StringBuilder sqlquery = new StringBuilder();
            sqlquery.Append(" select distinct x.unit_code,y.description from (select a.unit_code from adm_emp_unit_access a ");
            sqlquery.Append(" where a.empid= " + empid + " ");
            sqlquery.Append(" and   a.moduleid = '" + moduleid + "' ");
            sqlquery.Append(" and   a.hier_yn = 'N' union Select b.unit_code from  eb_unit_msts b where b.unit_code = b.process_unit_code  ");
            sqlquery.Append(" start with b.unit_code = (select min(c.unit_code) from adm_emp_unit_access c where  c.empid= " + empid + "   ");
            sqlquery.Append("  and   c.moduleid =  '" + moduleid + "'  ");
            sqlquery.Append("  and   c.hier_yn = 'Y' )  ");
            sqlquery.Append("  connect by prior b.unit_code=b.unit_parent_code union SELECT B.UNIT_CODE FROM V_EB_EMPLOYEE_COMPLETE_DTLS B WHERE B.PERSONAL_NO = " + empid + " ) x, eb_unit_msts y  ");
            sqlquery.Append("  where y.unit_code = x.unit_code order by 1 ");
            DataTable dt = _context.GetSQLQuery(sqlquery.ToString());
            var appUnitList = (from DataRow dr in dt.Rows
                               select new SelectListItem
                               {
                                   Text = Convert.ToString(Convert.ToString(dr["UNIT_CODE"] + "-" + dr["DESCRIPTION"])),
                                   Value = Convert.ToString(dr["unit_code"])
                               }).ToList();

            return appUnitList;
        }
        public DataTable GetSuggestionNoListForSUGSCR01(string empid,string unitCode,string fromDate, string ToDate)
        {
            StringBuilder sqlquery = new StringBuilder();
            sqlquery.Append("SELECT ALL  A.SUGG_NO, A.SUGG_DT, A.SUGG_BY_PNO1,B.EMP_NAME, ");
            sqlquery.Append("A.SUGG_HEADING,C.CAT_DESC,A.UNIT_CD,B.UNIT_NAME ");
            sqlquery.Append(" 	FROM GBL_SUGG_ENTRY A, VW_GBL_SUGG_USER B,GBL_SUGG_CAT_MST C");
            sqlquery.Append(" 	WHERE (A.SUGG_BY_PNO1 = B.PERSONAL_NO)  ");
            sqlquery.Append(" 	 AND A.SUGG_BY_PNO1='" + empid + "' ");
            sqlquery.Append(" 	and a.sugg_dt between '"+fromDate+"' and '"+ToDate+"' ");
            sqlquery.Append(" 	and B.UNIT_CODE="+unitCode+" ");
            sqlquery.Append(" 	AND A.CAT_CD=C.CAT_CD ");
            sqlquery.Append(" union all ");
            sqlquery.Append(" SELECT  distinct A.SUGG_NO, A.SUGG_DT, A.SUGG_BY_PNO1,B.EMP_NAME, ");
            sqlquery.Append(" A.SUGG_HEADING,C.CAT_DESC,A.UNIT_CD,B.UNIT_NAME  ");
            sqlquery.Append(" FROM GBL_SUGG_ENTRY A, VW_GBL_SUGG_USER B,GBL_SUGG_CAT_MST C,GBL_SUGG_PRG_ACCESS_DTLS d ");
            sqlquery.Append(" WHERE (A.SUGG_BY_PNO1 = B.PERSONAL_NO)  ");
            sqlquery.Append(" and d.PERSONAL_NO='"+empid+"' ");
            sqlquery.Append(" and a.UNIT_CD=b.UNIT_CODE ");
            sqlquery.Append(" 	and a.sugg_dt between '" + fromDate + "' and '" + ToDate + "' ");
            sqlquery.Append(" 	and B.UNIT_CODE=" + unitCode + " ");
            sqlquery.Append(" AND A.CAT_CD=C.CAT_CD");
            sqlquery.Append(" and a.UNIT_CD=d.UNIT_CODE ");
            sqlquery.Append(" and d.PROGRAM_ID='SUGSC09'");
            sqlquery.Append(" order by 1 desc");
            DataTable dt = _context.GetSQLQuery(sqlquery.ToString());
            var SuggestionNoList = (from DataRow dr in dt.Rows
                                    select new SelectListItem()
                                    {
                                        Text = Convert.ToString(dr["SUGG_NO"]+"-"+ dr["SUGG_DT"] +"-"+ dr["SUGG_HEADING"]),
                                        Value = Convert.ToString(dr["SUGG_NO"])
                                    }).ToList();

            return dt;
        }
        public List<SelectListItem> AdmPrgParentLOVBind(string Module, string Projid)
        {
            string sqlquery = "SELECT DISTINCT SUB_MENU_ID SUB_MENU_ID FROM ADM_SUB_MENU_MSTS WHERE PROJECTID = '" + Projid + "' AND MODULEID = '" + Module + "'  ";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new SelectListItem()
                         {
                             Text = Convert.ToString(dr["SUB_MENU_ID"]),
                             Value = Convert.ToString(dr["SUB_MENU_ID"])
                         }).ToList();

            return DRP_VALUE;

        }

       

        public List<SelectListItem> AdmSubMenuParentLOVBind(string Module, string Projid)
        {
            string sqlquery = " select '" + Module + "' SUB_MENU_ID FROM DUAL UNION ALL SELECT DISTINCT SUB_MENU_ID SUB_MENU_ID FROM ADM_SUB_MENU_MSTS WHERE PROJECTID = '" + Projid + "' AND MODULEID = '" + Module + "'  ";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new SelectListItem()
                         {
                             Text = Convert.ToString(dr["SUB_MENU_ID"]),
                             Value = Convert.ToString(dr["SUB_MENU_ID"])
                         }).ToList();

            return DRP_VALUE;

        }

        public List<SelectListItem> GetEmpForSecurity(string unit)
        {
            StringBuilder sqlquery = new StringBuilder();
            sqlquery.Append(" select EMP_NAME name, personal_no  from VW_GBL_SUGG_USER where unit_code = '" + unit + "' order by grade_code asc  ");
            DataTable dt = _context.GetSQLQuery(sqlquery.ToString());
            var appUnitList = (from DataRow dr in dt.Rows
                               select new SelectListItem
                               {
                                   Text = Convert.ToString(Convert.ToString(dr["personal_no"] + " - " + dr["name"])),
                                   Value = Convert.ToString(dr["personal_no"])
                               }).ToList();

            return appUnitList;
        }

        public List<SelectListItem> GetReportServers()
        {
            string sqlquery = "SELECT SERV_ID, SERV_DESC, IP FROM M_REP_SERVER WHERE STATUS = 'A' ";
            DataTable dtDRP_VALUE = _context.GetSQLQuery(sqlquery);
            List<SelectListItem> DRP_VALUE = new List<SelectListItem>();
            DRP_VALUE = (from DataRow dr in dtDRP_VALUE.Rows
                         select new SelectListItem()
                         {
                             Text = Convert.ToString(dr["SERV_DESC"]),
                             Value = Convert.ToString(dr["IP"])
                         }).ToList();

            return DRP_VALUE;

        }

       
    }
}
