using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailyPlantInput
    {
        public DateTime DateTime { get; set; }
        public string Shift { get; set; }
        public DateTime FeedDateTime { get; set; }
        public string PrCode { get; set; }
        public string Freeze { get; set; }
        public string Revised { get; set; }
        public decimal PrValue { get; set; }
        public int? CreatedBy { get; set; }

        public ParameterMaster PrCodeNavigation { get; set; }
    }
}
