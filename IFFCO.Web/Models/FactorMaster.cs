using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class FactorMaster
    {
        public string FrCode { get; set; }
        public string FrName { get; set; }
        public string FrUnit { get; set; }
        public decimal? FrValue { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDatetime { get; set; }
    }
}
