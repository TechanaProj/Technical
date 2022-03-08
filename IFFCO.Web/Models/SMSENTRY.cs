using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IFFCO.TECHPROD.Web.Models
{
    public class SMSENTRY
    {

        public string P_NO { get; set; }
        public string NAME { get; set; }
        public string DESIGNATION { get; set; }
        public string SMS_AMOUNT { get; set; }
        public DateTime? TILL_DATE { get; set; }
        public int? S_SNO { get; set; }
    }
}
