using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailySpEnergy
    {
        public DateTime DataDate { get; set; }
        public string OpCode { get; set; }
        public decimal? OpValue { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? FeedDateTime { get; set; }
    }
}
