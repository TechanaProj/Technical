using System;
using System.Collections.Generic;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class ParameterMaster
    {
        public ParameterMaster()
        {
            DailyPlantInput = new HashSet<DailyPlantInput>();
            DailyPlantOutput = new HashSet<DailyPlantOutput>();
        }

        public string PrCode { get; set; }
        public string PrName { get; set; }
        public string PrUnit { get; set; }
        public string PlCode { get; set; }
        public string PlInput { get; set; }

        public PlantMaster PlCodeNavigation { get; set; }
        public ICollection<DailyPlantInput> DailyPlantInput { get; set; }
        public ICollection<DailyPlantOutput> DailyPlantOutput { get; set; }
    }
}
