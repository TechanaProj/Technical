using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class GasMaster
    {
        public GasMaster()
        {
            DailyGasAlloc = new HashSet<DailyGasAlloc>();
        }

        public string GasName { get; set; }
        public string GasCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationTime { get; set; }
        public int? PriorityLevel { get; set; }
        public string GasNameInDatabase { get; set; }
        public string RmCode { get; set; }

        public ICollection<DailyGasAlloc> DailyGasAlloc { get; set; }
    }
}
