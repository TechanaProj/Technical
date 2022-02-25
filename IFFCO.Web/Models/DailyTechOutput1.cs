using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailyTechOutput1
    {
        public DateTime DataDate { get; set; }
        public string OpCode { get; set; }
        public decimal? OpValue { get; set; }
        public DateTime FeedDateTime { get; set; }
        public string Freeze { get; set; }
        public string Revised { get; set; }
        public string CreatedBy { get; set; }

        public OutputMaster OpCodeNavigation { get; set; }
    }
}
