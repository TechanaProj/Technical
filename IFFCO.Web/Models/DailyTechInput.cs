using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailyTechInput
    {
        public DateTime DataDate { get; set; }
        public DateTime FeedDtTime { get; set; }
        public string PrCode { get; set; }
        public decimal PrValue { get; set; }
        public string Freeze { get; set; }
        public string Revised { get; set; }
        public string Shift { get; set; }
        public int? CreatedBy { get; set; }
    }
}
