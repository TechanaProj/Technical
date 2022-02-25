using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailyPurgeGasDtls
    {
        public DateTime DataDate { get; set; }
        public decimal? PurgeGasAmm1 { get; set; }
        public decimal? PurgeGasAmm2 { get; set; }
        public decimal? ProdAmm1Pg { get; set; }
        public decimal? ProdAmm2Pg { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
