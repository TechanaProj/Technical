using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class DailyGasAlloc
    {
        public string GasCode { get; set; }
        public DateTime DataDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationTime { get; set; }
        public double? AllocatedQty { get; set; }
        public int? PriorityLevel { get; set; }
        public string Basis { get; set; }
        public double? LcvEnergy { get; set; }
        public double? GcvEnergy { get; set; }
        public int? DrawnQty { get; set; }

        public GasMaster GasCodeNavigation { get; set; }
    }
}
