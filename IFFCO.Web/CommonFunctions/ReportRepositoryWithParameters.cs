using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using Microsoft.AspNetCore.Http;
using System;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class ReportRepositoryWithParameters
    {
        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        private readonly ModelContext _context;

        //private readonly string reportURL = new AppConfiguration().ReportURL;
        //private readonly string reportURL = "http://140.238.253.16/reports/rwservlet?cmdkey=techprod_rep";
        //private readonly string reportURL = "https://wlsan.iffco.coop/reports/rwservlet?cmdkey=techprod_rep";
        private readonly string reportURL = "https://wlsaonla.iffco.coop/reports/rwservlet?cmdkey=techprod_rep";

        //private readonly string reportURLFrame = "http://";
        private readonly string reportURLFrame2 = "reports/rwservlet?cmdkey=techprod_rep";

        
        public ReportRepositoryWithParameters()
        {
            _context = new ModelContext();
        }

        public string GenerateSalaryCardReport(string model, string format)
        {

            string report = "";
            //string connstr = "hrmsadm_new";
            string connstr = "gess_rep";
            report = "http://10.12.1.132/reports/rwservlet?module=" + format + "+" + "cmdkey=" + connstr + "+" + model; //PRODCUTION           
            // report = "http://10.12.1.57/reports/rwservlet?module=" + format + "+" + "cmdkey=" + connstr + "+" + model; // DEVELOPEMENT 
            return report;
        }       

        public string GenerateReport(string querystring, string reportname)
        {
           
            string report = "";
            report = reportURL + "+module=" + reportname + "+" + querystring;
            report = "data:application/pdf;base64," + ReportService.EncodeServerName(report);
            return report;
        }

        public string GenerateReport(string querystring, string reportname, string NotEncode)
        {
            string report = "";
            if (NotEncode == "NotEncode")
            {
                report = reportURL + "+module=" + reportname + "+" + querystring;
            }
            else
            {
                report = NotEncode + reportURLFrame2 + "+module=" + reportname + "+" + querystring;
            }                      
            return report;
        }

        public string GenerateReportExcel(string querystring, string reportname)
        {
            string report = "";
            report = reportURL + "+module=" + reportname + "+" + querystring;
            report = "data:application/vnd.ms-excel;base64," + ReportService.EncodeServerName(report);
            return report;
        }

        public string GenerateReportExcelWithoutEncode(string querystring, string reportname)
        {
            string report = "";
            report = reportURL + "+module=" + reportname + "+" + querystring;
            //report = "data:application/vnd.ms-excel;base64," + report;
            return report;
        }
        public string GenerateReportRdlc(string reportrdlcUrl, string querystring, string reportname, string module, string name, string personalNo, string fullClientIp, string clientIp)
        {
            string report = "";

            //report = reportrdlcUrl + "/" + module + "/" + reportname + "?" + Encclass.GetEncryptedQueryString(querystring.Replace("''", ""));
            report = reportrdlcUrl + "/" + reportname + "?" + Encclass.GetEncryptedQueryString(querystring.Replace("''", ""));
            //Encclass.ReportLog(module, reportname, name, querystring, personalNo, fullClientIp, clientIp);
            return report;
        }



    }
}
